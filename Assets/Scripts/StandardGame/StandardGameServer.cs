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
    private void InitGame() {

      List<RuleNames> ruleNames = new List<RuleNames>();
      ruleNames.Add(RuleNames.DiagonalToTopRight);
      ruleNames.Add(RuleNames.Water);

      this.gameSettings = new GameSettings(Constants.mapSize, Constants.mapSize, 3, 0, 6, 60, ruleNames);
      this.GameServer = new Schnoz(this.gameSettings);

      this.GameServer.InitialiseMap();
      this.GameServer.CreateDeck();
      this.GameServer.ShuffleDeck();
      this.GameServer.DrawCards();

      NetInitialiseMap im = new NetInitialiseMap();
      im.nRows = this.GameServer.GameSettings.NRows;
      im.nCols = this.GameServer.GameSettings.NCols;
      im.ruleNames = this.GameServer.GameSettings.Rules.Select(rule => rule.RuleName).ToList();

      NetUpdateTerrains ut = new NetUpdateTerrains();
      ut.terrains = this.GameServer.Map.Terrains;

      NetUpdateUnits uu = new NetUpdateUnits();
      uu.addedUnits = this.GameServer.Map.Units;

      NetUpdateCards uc = new NetUpdateCards();
      uc.cards = this.GameServer.Deck.OpenCards;

      NetRender r = new NetRender();
      r.renderTypes = new List<RenderTypes>() {
                RenderTypes.Map, RenderTypes.OpenCards, RenderTypes.CurrentPlayer, RenderTypes.Rules
              };

      Server.Instance.Broadcast(im);
      Server.Instance.Broadcast(ut);
      Server.Instance.Broadcast(uu);
      Server.Instance.Broadcast(uc);

      Debug.Log($"Render count before send: {r.renderTypes.Count}");
      Server.Instance.Broadcast(r);
    }
    private void OnWelcome(NetMessage msg, NetworkConnection cnn) {
      Debug.Log("Message to Server: OnWelcome");
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
              break;
            }
          case NetworkManager.NetworkIdentity.HOST: {
              // Start the game immediately
              Debug.Log("Starts the game as host");
              this.InitGame();
              break;
            }
          default: {
              // Start the local game immediately
              Debug.Log("Starts local game");
              this.InitGame();
              break;
            }
        }
      } else {
        Debug.Log($"Waiting for {1 - this.playerCount} more players.");
      }
    }
    private void OnMakeMove(NetMessage msg, NetworkConnection cnn) {
      Debug.Log("Message to Server: OnMakeMove");
      Schnoz gs = this.GameServer;

      int ownerId = isLocalGame ? gs.ActivePlayerId : cnn.InternalId;
      if (ownerId != gs.ActivePlayerId) {
        Debug.Log("Cannot place unit because it is not your turn.");
        return;
      }
      NetMakeMove mm = msg as NetMakeMove;

      NetRender r = new NetRender();
      r.renderTypes = new List<RenderTypes>();

      UnitFormation unitFormation = new UnitFormation(UnitFormation.unitFormationIdToTypeDict[mm.unitFormationId]);
      unitFormation.rotation = mm.rotation;
      unitFormation.mirrorHorizontal = mm.mirrorHorizontal == 1 ? true : false;
      unitFormation.mirrorVertical = mm.mirrorVertical == 1 ? true : false;
      Coordinate coordinate = new Coordinate(mm.row, mm.col);
      bool canPlaceUnitFormation = gs.CanPlaceUnitFormation(ownerId, coordinate, unitFormation);
      if (!canPlaceUnitFormation) {
        Debug.Log("Cannot place unit because at least one tile is not placeable.");
        return;
      }
      gs.PlaceUnitFormation(ownerId, coordinate, unitFormation);

      NetUpdateUnits uu = new NetUpdateUnits();
      uu.addedUnits = gs.Map.Units;
      Server.Instance.Broadcast(uu);

      NetUpdateTerrains ut = new NetUpdateTerrains();
      ut.terrains = gs.Map.Terrains;
      Server.Instance.Broadcast(ut);

      r.renderTypes.Add(RenderTypes.Map);

      bool endOfStage = gs.Turn != 0 && (gs.Turn + 1) % gs.GameSettings.NumberOfTurnsPerStage == 0;
      Debug.Log($"TURN: {gs.Turn}, endOfStage: {endOfStage}");
      if (endOfStage) {
        gs.GameSettings.Rules.ForEach(rule => {
          Player ruleWinner = gs.DetermineRuleWinner(rule.RuleName);
          if (ruleWinner != null) {
            ruleWinner.SetScore(ruleWinner.Score + 1);
          }
        });
        NetUpdateScore us = new NetUpdateScore();
        us.ScorePlayer1 = gs.GameSettings.IdToPlayerDict[0].Score;
        us.ScorePlayer2 = gs.GameSettings.IdToPlayerDict[1].Score;
        Debug.Log($"SERVER: Score P1:{us.ScorePlayer1.ToString()}, Score P2: {us.ScorePlayer2.ToString()}");
        Server.Instance.Broadcast(us);

        r.renderTypes.Add(RenderTypes.Score);
      }

      bool oddTurn = gs.Turn % 2 != 0;
      if (oddTurn) {
        gs.DrawCards();
        NetUpdateCards uc = new NetUpdateCards();
        uc.cards = gs.Deck.OpenCards;
        Server.Instance.Broadcast(uc);
        r.renderTypes.Add(RenderTypes.OpenCards);
      } else {
        r.renderTypes.Add(RenderTypes.CurrentPlayer);
      }

      gs.EndTurn();
      Server.Instance.Broadcast(new NetEndTurn());

      r.renderTypes.Add(RenderTypes.Rules);
      Server.Instance.Broadcast(r);
    }
    #endregion

  }
}
