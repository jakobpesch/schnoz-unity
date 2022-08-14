using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using TypeAliases;
using Unity.Networking.Transport;
using System.Collections;

namespace Schnoz {
  public class StandardGameServer : MonoBehaviour {
    // Multiplayer logic
    private bool isLocalGame {
      get => RelayNetworking.Instance.NI == RelayNetworking.NetworkIdentity.LOCAL;
    }
    [SerializeField] private int playerCount = -1;
    public Schnoz GameServer { get; private set; }
    [SerializeField] private GameSettings gameSettings;
    private float Timer = 0f;
    private bool gameStarted;

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
      NetUtility.S_START_GAME += this.OnStartGame;
      NetUtility.S_MAKE_MOVE += this.OnMakeMove;
      NetUtility.S_PUT_SINGLE_PIECE += this.OnPutSinglePiece;
    }


    private void UnregisterEvents() {
      NetUtility.S_WELCOME -= this.OnWelcome;
      NetUtility.S_START_GAME -= this.OnStartGame;
      NetUtility.S_MAKE_MOVE -= this.OnMakeMove;
      NetUtility.S_PUT_SINGLE_PIECE -= this.OnPutSinglePiece;
    }

    private void FixedUpdate() {
      if (this.gameStarted && !this.GameServer.GameOver) {
        this.Timer -= Time.deltaTime;
        if (this.Timer <= 0) {
          this.OnMakeMove(null, new NetworkConnection());
          this.Timer = this.GameServer.GameSettings.SecondsPerTurn;
        }
      }
    }


    private void OnWelcome(NetMessage msg, NetworkConnection cnn) {
      Debug.Log("OnWelcome");
      NetWelcome welcome = msg as NetWelcome;
      welcome.AssignedPlayerId = (PlayerIds)(++this.playerCount);
      welcome.AssignedRole = this.playerCount == 0 ? PlayerRoles.ADMIN : PlayerRoles.PLAYER;
      RelayNetworking.Instance.SendToClient(cnn, welcome);

      bool allPlayersPresent = this.playerCount == 1;
      if (this.isLocalGame || allPlayersPresent) {
        Debug.Log("All players present");
        RelayNetworking.Instance.Broadcast(new NetAllPlayersConnected());
      } else {
        Debug.Log($"Waiting for {1 - this.playerCount} more players.");
      }
    }

    private void OnStartGame(NetMessage msg, NetworkConnection cnn) {
      NetStartGame sg = msg as NetStartGame;
      this.gameSettings = new GameSettings(sg.mapSize, sg.partsGrass, sg.partsStone, sg.partsWater, sg.partsBush, 3, 0, sg.numberOfStages, sg.secondsPerTurn, sg.ruleNames);
      this.InitGame();
    }

    private void InitGame() {
      this.GameServer = new Schnoz(this.gameSettings);

      this.Timer = this.GameServer.GameSettings.SecondsPerTurn;

      GameSettings settings = this.GameServer.GameSettings;
      this.GameServer.InitialiseMap();
      this.GameServer.CreateDeck();
      // this.GameServer.ShuffleDeck();
      this.GameServer.DrawCards();

      NetInitialiseMap initialiseMap = new NetInitialiseMap();
      initialiseMap.mapSize = settings.NRows;
      initialiseMap.numberOfStages = settings.NumberOfStages;
      initialiseMap.secondsPerTurn = settings.SecondsPerTurn;

      initialiseMap.partsGrass = settings.PartsGrass;
      initialiseMap.partsStone = settings.PartsStone;
      initialiseMap.partsWater = settings.PartsWater;
      initialiseMap.partsBush = settings.PartsBush;

      initialiseMap.ruleNames = settings.Rules.Select(rule => rule.RuleName).ToList();

      NetUpdateTerrains updateTerrain = new NetUpdateTerrains();
      updateTerrain.terrains = this.GameServer.Map.Terrains;

      NetUpdateUnits updateUnits = new NetUpdateUnits();
      updateUnits.addedUnits = this.GameServer.Map.Units;

      NetUpdateCards updateCards = new NetUpdateCards();
      updateCards.cards = this.GameServer.Deck.OpenCards;

      NetUpdateSinglePieces updateSinglePieces = new NetUpdateSinglePieces();
      updateSinglePieces.SinglePiecesPlayer1 = this.GameServer.GameSettings.PlayerIdToPlayerDict[PlayerIds.Player1].SinglePieces;
      updateSinglePieces.SinglePiecesPlayer2 = this.GameServer.GameSettings.PlayerIdToPlayerDict[PlayerIds.Player2].SinglePieces;

      NetRender render = new NetRender();
      render.renderTypes = new List<RenderTypes>() {
        RenderTypes.Map, RenderTypes.Turns, RenderTypes.Timer, RenderTypes.OpenCards, RenderTypes.Rules, RenderTypes.SinglePieces
      };

      RelayNetworking.Instance.Broadcast(initialiseMap);
      RelayNetworking.Instance.Broadcast(updateTerrain);
      RelayNetworking.Instance.Broadcast(updateUnits);
      RelayNetworking.Instance.Broadcast(updateCards);
      RelayNetworking.Instance.Broadcast(updateSinglePieces);
      RelayNetworking.Instance.Broadcast(render);
      this.Timer = this.GameServer.GameSettings.SecondsPerTurn;
      this.gameStarted = true;
    }

    private void OnMakeMove(NetMessage msg, NetworkConnection cnn) {
      NetMakeMove mm = msg as NetMakeMove;

      NetRender render = new NetRender();
      render.renderTypes = new List<RenderTypes>();

      bool timedOut = msg == null;
      if (!timedOut) {
        PlayerIds ownerId = msg != null && isLocalGame ? this.GameServer.ActivePlayerId : (PlayerIds)cnn.InternalId;
        if (ownerId != this.GameServer.ActivePlayerId) {
          Debug.Log("Cannot place unit because it is not your turn.");
          return;
        }

        UnitFormation unitFormation = new UnitFormation(UnitFormation.unitFormationIdToTypeDict[mm.unitFormationId]);
        unitFormation.rotation = mm.rotation;
        unitFormation.mirrorHorizontal = mm.mirrorHorizontal == 1;
        unitFormation.mirrorVertical = mm.mirrorVertical == 1;
        Coordinate coordinate = new Coordinate(mm.row, mm.col);
        bool canPlaceUnitFormation = this.GameServer.CanPlaceUnitFormation(ownerId, coordinate, unitFormation);
        if (!canPlaceUnitFormation) {
          Debug.Log("Cannot place unit because at least one tile is not placeable.");
          return;
        }
        this.GameServer.PlaceUnitFormation(ownerId, coordinate, unitFormation);

        NetUpdateUnits updateUnits = new NetUpdateUnits();
        updateUnits.addedUnits = this.GameServer.Map.Units;
        RelayNetworking.Instance.Broadcast(updateUnits);

        NetUpdateTerrains updateTerrains = new NetUpdateTerrains();
        updateTerrains.terrains = this.GameServer.Map.Terrains;
        RelayNetworking.Instance.Broadcast(updateTerrains);

        render.renderTypes.Add(RenderTypes.Map);
      }

      bool endOfStage = this.GameServer.Turn != 0 && (this.GameServer.Turn + 1) % this.GameServer.GameSettings.NumberOfTurnsPerStage == 0;
      if (endOfStage) {
        this.GameServer.GameSettings.Rules.ForEach(rule => {
          Player ruleWinner = this.GameServer.DetermineRuleWinner(rule.RuleName);
          if (ruleWinner != null) {
            ruleWinner.SetScore(ruleWinner.Score + 1);
          }
        });
        NetUpdateScore us = new NetUpdateScore();
        us.ScorePlayer1 = this.GameServer.GameSettings.PlayerIdToPlayerDict[PlayerIds.Player1].Score;
        us.ScorePlayer2 = this.GameServer.GameSettings.PlayerIdToPlayerDict[PlayerIds.Player2].Score;
        RelayNetworking.Instance.Broadcast(us);
        render.renderTypes.Add(RenderTypes.Score);

        // this.GameServer.GameSettings.Players.ForEach(player => player.AddSinglePiece());
        // NetUpdateSinglePieces usp = new NetUpdateSinglePieces();
        // usp.SinglePiecesPlayer1 = this.GameServer.GameSettings.PlayerIdToPlayerDict[PlayerIds.Player1].SinglePieces;
        // usp.SinglePiecesPlayer2 = this.GameServer.GameSettings.PlayerIdToPlayerDict[PlayerIds.Player2].SinglePieces;
        // RelayNetworking.Instance.Broadcast(usp);
        // r.renderTypes.Add(RenderTypes.SinglePieces);
      }

      bool oddTurn = this.GameServer.Turn % 2 != 0;
      if (oddTurn) {
        this.GameServer.DrawCards();
        NetUpdateCards updateCards = new NetUpdateCards();
        updateCards.cards = this.GameServer.Deck.OpenCards;
        RelayNetworking.Instance.Broadcast(updateCards);
        render.renderTypes.Add(RenderTypes.OpenCards);
      } else {
        // r.renderTypes.Add(RenderTypes.SinglePieces);
      }

      bool gameOver = this.GameServer.Turn == this.GameServer.GameSettings.TurnOrder.Count - 1;
      if (gameOver) {
        this.GameServer.GameOver = true;
        NetGameOver go = new NetGameOver();
        PlayerIds leadingPlayerId = this.GameServer.GetLeadingPlayerId();

        go.winnerId = leadingPlayerId;
        RelayNetworking.Instance.Broadcast(go);
      } else {
        this.GameServer.EndTurn();
        RelayNetworking.Instance.Broadcast(new NetEndTurn());
      }
      render.renderTypes.Add(RenderTypes.Turns);

      render.renderTypes.Add(RenderTypes.Timer);
      render.renderTypes.Add(RenderTypes.Rules);
      RelayNetworking.Instance.Broadcast(render);
      this.Timer = this.GameServer.GameSettings.SecondsPerTurn;
    }


    private void OnPutSinglePiece(NetMessage msg, NetworkConnection cnn) {
      Debug.Log("Message to Server: OnPutSinglePiece");
      Schnoz gs = this.GameServer;

      PlayerIds ownerId = isLocalGame ? gs.ActivePlayerId : (PlayerIds)cnn.InternalId;
      if (ownerId != gs.ActivePlayerId) {
        Debug.Log("Cannot place unit because it is not your turn.");
        return;
      }
      NetPutSinglePiece psp = msg as NetPutSinglePiece;
      Coordinate coordinate = psp.coordinate;
      bool canBePlaced = this.GameServer.Map.CoordinateToTileDict[coordinate].Placeable;

      if (!canBePlaced) {
        Debug.Log("Cannot place single piece here.");
        return;
      }
      NetRender r = new NetRender();

      gs.PlaceUnit(ownerId, coordinate);
      r.renderTypes.Add(RenderTypes.Map);

      gs.GameSettings.PlayerIdToPlayerDict[ownerId].RemoveSinglePiece();

      NetUpdateSinglePieces usp = new NetUpdateSinglePieces();
      usp.SinglePiecesPlayer1 = this.GameServer.GameSettings.PlayerIdToPlayerDict[PlayerIds.Player1].SinglePieces;
      usp.SinglePiecesPlayer2 = this.GameServer.GameSettings.PlayerIdToPlayerDict[PlayerIds.Player2].SinglePieces;
      r.renderTypes.Add(RenderTypes.SinglePieces);
      r.renderTypes.Add(RenderTypes.Rules);
      RelayNetworking.Instance.Broadcast(usp);

      NetUpdateUnits uu = new NetUpdateUnits();
      uu.addedUnits = gs.Map.Units;
      RelayNetworking.Instance.Broadcast(uu);

      RelayNetworking.Instance.Broadcast(r);
    }
    #endregion

  }
}
