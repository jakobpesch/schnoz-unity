using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using TypeAliases;
using Unity.Networking.Transport;
namespace Schnoz {
  public class StandardGameServer : MonoBehaviour {
    // Multiplayer logic
    private bool isLocalGame {
      get => NetworkManager.Instance.NI == NetworkManager.NetworkIdentity.LOCAL;
    }
    [SerializeField] private int playerCount = -1;
    public Schnoz GameServer { get; private set; }
    [SerializeField] private GameSettings gameSettings;
    public Dictionary<Guid, Card> OpenCardsDict {
      get => this.GameServer.OpenCards.ToDictionary(card => card.Id);
    }
    public Dictionary<Guid, Tile> TileDict {
      get => this.GameServer.Map.Tiles.ToDictionary(tile => tile.Id);
    }

    #region Networking
    // Setup and cleanup
    private void Awake() {
      this.RegisterEvents();
    }
    private void OnDestroy() {
      this.UnregisterEvents();
    }
    private void RegisterEvents() {
      NetUtility.S_WELCOME += this.OnWelcome;
      NetUtility.S_MAKE_MOVE += this.OnMakeMove;
    }
    private void UnregisterEvents() {
      NetUtility.S_WELCOME -= this.OnWelcome;
      NetUtility.S_MAKE_MOVE -= this.OnMakeMove;
    }
    private void OnWelcome(NetMessage msg, NetworkConnection cnn) {
      NetWelcome nw = msg as NetWelcome;

      nw.AssinedTeam = ++this.playerCount;

      Server.Instance.SendToClient(cnn, nw);

      if (isLocalGame || this.playerCount == 1) {
        Debug.Log("All players present");
        switch (NetworkManager.Instance.NI) {
          case NetworkManager.NetworkIdentity.DEDICATED_SERVER: {
              // Start the game immediately
              Debug.Log("Starts the game as dedicated server");
              this.InitGame();
              NetStartGame sg = new NetStartGame();
              sg.nRows = this.GameServer.GameSettings.NRows;
              sg.nCols = this.GameServer.GameSettings.NCols;
              sg.units = this.GameServer.Map.Units;
              sg.terrains = this.GameServer.Map.Terrains;
              sg.cards = this.GameServer.Deck.OpenCards;

              Server.Instance.Broadcast(sg);
              break;
            }
          case NetworkManager.NetworkIdentity.HOST: {
              // Start the game immediately
              Debug.Log("Starts the game as host");
              this.InitGame();
              NetStartGame sg = new NetStartGame();
              sg.nRows = this.GameServer.GameSettings.NRows;
              sg.nCols = this.GameServer.GameSettings.NCols;
              sg.units = this.GameServer.Map.Units;
              sg.terrains = this.GameServer.Map.Terrains;
              sg.cards = this.GameServer.Deck.OpenCards;

              Server.Instance.Broadcast(sg);
              break;
            }
          default: {
              // Start the local game immediately
              Debug.Log("Starts local game");
              this.InitGame();
              NetStartGame sg = new NetStartGame();
              sg.nRows = this.GameServer.GameSettings.NRows;
              sg.nCols = this.GameServer.GameSettings.NCols;
              sg.units = this.GameServer.Map.Units;
              sg.terrains = this.GameServer.Map.Terrains;
              sg.cards = this.GameServer.Deck.OpenCards;

              Server.Instance.Broadcast(sg);
              break;
            }
        }
      } else {
        Debug.Log($"Waiting for {1 - this.playerCount} more players.");
      }
    }
    private void OnMakeMove(NetMessage msg, NetworkConnection cnn) {
      int ownerId = isLocalGame ? this.GameServer.ActivePlayerId : cnn.InternalId;
      if (ownerId != this.GameServer.ActivePlayerId) {
        Debug.Log("Cannot place unit because it is not your turn.");
        return;
      }
      NetMakeMove mm = msg as NetMakeMove;

      UnitFormation unitFormation = new UnitFormation(UnitFormation.unitFormationIdToTypeDict[mm.unitFormationId]);
      unitFormation.rotation = mm.rotation;
      unitFormation.mirrorHorizontal = mm.mirrorHorizontal == 1 ? true : false;
      unitFormation.mirrorVertical = mm.mirrorVertical == 1 ? true : false;
      Coordinate coordinate = new Coordinate(mm.row, mm.col);
      bool canPlaceUnitFormation = this.GameServer.CanPlaceUnitFormation(ownerId, coordinate, unitFormation);
      if (!canPlaceUnitFormation) {
        Debug.Log("Cannot place unit because at least one tile is not placeable.");
        return;
      }
      this.GameServer.PlaceUnitFormation(ownerId, coordinate, unitFormation);

      NetUpdateMap um = new NetUpdateMap();
      um.units = this.GameServer.Map.Units;
      um.terrains = this.GameServer.Map.Terrains;
      Debug.Log($"Units count SERVER {um.units.Count}");
      Debug.Log($"Terrains count SERVER {um.terrains.Count}");
      Server.Instance.Broadcast(um);

      bool endOfStage = this.GameServer.Turn % GameServer.GameSettings.NumberOfTurnsPerStage == 0;
      Debug.Log($"TURN: {this.GameServer.Turn}, endOfStage: {endOfStage}");
      if (endOfStage) {
        this.GameServer.GameSettings.Rules.ForEach(rule => {
          Player ruleWinner = this.GameServer.DetermineRuleWinner(rule.RuleName);
          if (ruleWinner != null) {
            ruleWinner.SetScore(ruleWinner.Score + 1);
          }
        });
        NetUpdateScore us = new NetUpdateScore();
        us.ScorePlayer1 = this.GameServer.GameSettings.IdToPlayerDict[0].Score;
        us.ScorePlayer2 = this.GameServer.GameSettings.IdToPlayerDict[1].Score;
        Server.Instance.Broadcast(us);
      }

      bool oddTurn = this.GameServer.Turn % 2 != 0;
      if (oddTurn) {
        this.GameServer.DrawCards();
        NetUpdateCards uc = new NetUpdateCards();
        uc.netOpenCardsString = this.GameServer.Deck.SerializeOpenCards();
        Server.Instance.Broadcast(uc);
      }

      this.GameServer.EndTurn();
    }
    #endregion
    private void InitGame() {
      List<RuleNames> ruleNames = new List<RuleNames>();
      ruleNames.Add(RuleNames.DiagonalToTopRight);
      ruleNames.Add(RuleNames.Water);

      this.gameSettings = new GameSettings(Constants.mapSize, Constants.mapSize, 3, 0, 6, 60, ruleNames);
      this.GameServer = new Schnoz(this.gameSettings);



      this.GameServer.CreateMap();
      this.GameServer.CreateDeck();
      this.GameServer.ShuffleDeck();
      this.GameServer.DrawCards();
    }
  }
}
