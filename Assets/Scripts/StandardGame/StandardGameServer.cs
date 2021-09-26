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
    [SerializeField] private int currentTeam = -1;
    [SerializeField] private int playersTurn = 0;
    [SerializeField] private Schnoz game;
    public Schnoz Game
    {
      get => this.game;
    }
    [SerializeField] private GameSettings gameSettings;
    public Dictionary<Guid, Card> OpenCardsDict
    {
      get => this.game.OpenCards.ToDictionary(card => card.Id);
    }
    public Dictionary<Guid, Tile> TileDict
    {
      get => this.game.Map.Tiles.ToDictionary(tile => tile.Id);
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
              sg.netMapString = this.Game.Map.Serialize();
              Server.Instance.Broadcast(sg);
              break;
            }
          default:
            {
              // Start the game immediately
              Debug.Log("Starts the game as host");
              this.InitGame();
              Debug.Log(this.Game.Deck.OpenCards.Count);
              NetStartGame sg = new NetStartGame();
              sg.netMapString = this.Game.Map.Serialize();
              NetOpenCards oc = new NetOpenCards();
              sg.netOpenCardsString = this.Game.Deck.SerializeOpenCards();
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

      if (cnn.InternalId != playersTurn)
      {
        return;
      }

      UnitFormation unitFormation = new UnitFormation(UnitFormation.unitFormationIdToTypeDict[mm.unitFormationId]);
      unitFormation.rotation = mm.rotation;
      unitFormation.mirrorHorizontal = mm.mirrorHorizontal == 1 ? true : false;
      unitFormation.mirrorVertical = mm.mirrorVertical == 1 ? true : false;
      Coordinate coordinate = new Coordinate(mm.row, mm.col);
      this.Game.PlaceUnitFormation(cnn.InternalId, coordinate, unitFormation);
      NetUpdateMap um = new NetUpdateMap();
      um.netMapString = this.Game.Map.Serialize();
      Server.Instance.Broadcast(um);
      this.playersTurn = this.playersTurn == 0 ? 1 : 0;
    }
    #endregion
    private void InitGame()
    {
      this.gameSettings = new GameSettings(9, 9, 3, 0, 6, 30, new List<Player>() { new Player(0), new Player(1) });
      this.game = new Schnoz(this.gameSettings);
      this.game.CreateMap();
      this.game.CreateDeck();
      this.game.ShuffleDeck();
      this.game.DrawCards();
    }
    private void StartGame()
    {
      // List<RuleLogic> ruleLogics = new List<RuleLogic>();
      // ruleLogics.Add(RuleLogicMethods.DiagonalToTopRight);
    }
  }
}
