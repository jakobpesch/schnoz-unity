using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using TypeAliases;
using Unity.Networking.Transport;
namespace Schnoz
{
  public class StandardGameServer : MonoBehaviour
  {
    // Multiplayer logic
    private bool isLocalGame
    {
      get => NetworkManager.Instance.NI == NetworkManager.NetworkIdentity.LOCAL;
    }
    [SerializeField] private int playerCount = -1;
    public Schnoz GameServer { get; private set; }
    [SerializeField] private GameSettings gameSettings;
    public Dictionary<Guid, Card> OpenCardsDict
    {
      get => this.GameServer.OpenCards.ToDictionary(card => card.Id);
    }
    public Dictionary<Guid, Tile> TileDict
    {
      get => this.GameServer.Map.Tiles.ToDictionary(tile => tile.Id);
    }

    #region Networking
    // Setup and cleanup
    private void Awake()
    {
      this.RegisterEvents();
    }
    private void OnDestroy()
    {
      this.UnregisterEvents();
    }
    private void RegisterEvents()
    {
      NetUtility.S_WELCOME += this.OnWelcome;
      NetUtility.S_MAKE_MOVE += this.OnMakeMove;
    }
    private void UnregisterEvents()
    {
      NetUtility.S_WELCOME -= this.OnWelcome;
      NetUtility.S_MAKE_MOVE -= this.OnMakeMove;
    }
    private void OnWelcome(NetMessage msg, NetworkConnection cnn)
    {
      NetWelcome nw = msg as NetWelcome;

      nw.AssinedTeam = ++this.playerCount;

      Server.Instance.SendToClient(cnn, nw);

      if (isLocalGame || this.playerCount == 1)
      {
        Debug.Log("All players present");
        switch (NetworkManager.Instance.NI)
        {
          case NetworkManager.NetworkIdentity.DEDICATED_SERVER:
            {
              // Start the game immediately
              Debug.Log("Starts the game as dedicated server");
              this.InitGame();
              NetStartGame sg = new NetStartGame();
              sg.netMapString = this.GameServer.Map.Serialize();
              Debug.Log(sg.netMapString);
              Server.Instance.Broadcast(sg);
              break;
            }
          case NetworkManager.NetworkIdentity.HOST:
            {
              // Start the game immediately
              Debug.Log("Starts the game as host");
              this.InitGame();
              NetStartGame sg = new NetStartGame();
              sg.netMapString = this.GameServer.Map.Serialize();
              NetOpenCards oc = new NetOpenCards();
              sg.netOpenCardsString = this.GameServer.Deck.SerializeOpenCards();
              Server.Instance.Broadcast(sg);
              break;
            }
          default:
            {
              // Start the local game immediately
              Debug.Log("Starts local game");
              this.InitGame();
              NetStartGame sg = new NetStartGame();
              sg.netMapString = this.GameServer.Map.Serialize();
              NetOpenCards oc = new NetOpenCards();
              sg.netOpenCardsString = this.GameServer.Deck.SerializeOpenCards();
              Server.Instance.Broadcast(sg);
              break;
            }
        }
      }
      else
      {
        Debug.Log($"Waiting for {1 - this.playerCount} more players.");
      }
    }
    private void OnMakeMove(NetMessage msg, NetworkConnection cnn)
    {
      NetMakeMove mm = msg as NetMakeMove;
      int ownerId = isLocalGame ? this.GameServer.ActivePlayerId : cnn.InternalId;
      if (ownerId != this.GameServer.ActivePlayerId)
      {
        return;
      }
      UnitFormation unitFormation = new UnitFormation(UnitFormation.unitFormationIdToTypeDict[mm.unitFormationId]);
      unitFormation.rotation = mm.rotation;
      unitFormation.mirrorHorizontal = mm.mirrorHorizontal == 1 ? true : false;
      unitFormation.mirrorVertical = mm.mirrorVertical == 1 ? true : false;
      Coordinate coordinate = new Coordinate(mm.row, mm.col);
      bool canPlaceUnitFormation = this.GameServer.CanPlaceUnitFormation(ownerId, coordinate, unitFormation);
      if (!canPlaceUnitFormation)
      {
        return;
      }
      this.GameServer.PlaceUnitFormation(ownerId, coordinate, unitFormation);
      NetUpdateMap um = new NetUpdateMap();
      um.netMapString = this.GameServer.Map.Serialize();
      Server.Instance.Broadcast(um);

      if (this.GameServer.Turn % 2 != 0)
      {
        NetUpdateCards uc = new NetUpdateCards();
        this.GameServer.DrawCards();
        uc.netOpenCardsString = this.GameServer.Deck.SerializeOpenCards();
        Server.Instance.Broadcast(uc);
      }

      this.GameServer.EndTurn();

    }
    #endregion
    private void InitGame()
    {
      List<RuleLogic> ruleLogics = new List<RuleLogic>();
      ruleLogics.Add(RuleLogicMethods.DiagonalToTopRight);
      ruleLogics.Add(RuleLogicMethods.Water);

      this.gameSettings = new GameSettings(5, 5, 3, 0, 6, 30, ruleLogics);
      this.GameServer = new Schnoz(this.gameSettings);



      this.GameServer.CreateMap();
      this.GameServer.CreateDeck();
      this.GameServer.ShuffleDeck();
      this.GameServer.DrawCards();
    }
  }
}
