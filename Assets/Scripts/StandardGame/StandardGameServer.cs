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

      if (this.playerCount == 1)
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
          default:
            {
              // Start the game immediately
              Debug.Log("Starts the game as host");
              this.InitGame();
              Debug.Log(this.GameServer.Deck.OpenCards.Count);
              NetStartGame sg = new NetStartGame();
              sg.netMapString = this.GameServer.Map.Serialize();
              NetOpenCards oc = new NetOpenCards();
              sg.netOpenCardsString = this.GameServer.Deck.SerializeOpenCards();
              Debug.Log(sg.netMapString);
              Debug.Log(sg.netOpenCardsString);
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
      if (cnn.InternalId != this.GameServer.ActivePlayerId)
      {
        return;
      }
      UnitFormation unitFormation = new UnitFormation(UnitFormation.unitFormationIdToTypeDict[mm.unitFormationId]);
      unitFormation.rotation = mm.rotation;
      unitFormation.mirrorHorizontal = mm.mirrorHorizontal == 1 ? true : false;
      unitFormation.mirrorVertical = mm.mirrorVertical == 1 ? true : false;
      Coordinate coordinate = new Coordinate(mm.row, mm.col);
      this.GameServer.PlaceUnitFormation(cnn.InternalId, coordinate, unitFormation);
      NetUpdateMap um = new NetUpdateMap();
      um.netMapString = this.GameServer.Map.Serialize();
      Server.Instance.Broadcast(um);
      this.GameServer.EndTurn();
    }
    #endregion
    private void InitGame()
    {
      this.gameSettings = new GameSettings(9, 9, 3, 0, 6, 30, new List<int>() { 0, 1 });
      this.GameServer = new Schnoz(this.gameSettings);
      this.GameServer.CreateMap();
      this.GameServer.CreateDeck();
      this.GameServer.ShuffleDeck();
      this.GameServer.DrawCards();
    }
    private void StartGame()
    {
      // List<RuleLogic> ruleLogics = new List<RuleLogic>();
      // ruleLogics.Add(RuleLogicMethods.DiagonalToTopRight);
    }
  }
}
