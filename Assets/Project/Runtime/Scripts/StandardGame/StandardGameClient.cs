using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using TypeAliases;
using System.ComponentModel;
namespace Schnoz {
  public class StandardGameClient : MonoBehaviour {
    // Multiplayer logic
    [SerializeField] private int currentTeam = -1;
    public PlayerRoles AssignedRole { get; private set; }
    public bool ReadyToStartGame { get; private set; }

    public StandardGameViewManager ViewManager { get; private set; }
    public Schnoz GameClient { get; private set; }
    public float Timer;
    private bool gameStarted = false;
    [SerializeField] private GameSettings gameSettings;
    [SerializeField] private Tile hoveringTile;
    public Tile HoveringTile {
      get => this.hoveringTile;
      set {
        this.hoveringTile = value;
        this.SetHoveringTiles(this.HoveringTile);
      }
    }
    public List<Tile> HoveringTiles;
    public Guid selectedCardId;
    public Guid SelectedCardId {
      get => this.selectedCardId;
      set {
        this.selectedCardId = value;
        this.ViewManager.Render(RenderTypes.SelectedCard);
      }
    }
    public Guid SinglePieceId { get; set; }
    public Dictionary<Guid, Card> OpenCardsDict {
      get => this.GameClient.OpenCards.ToDictionary(card => card.Id);
    }
    public Dictionary<Coordinate, Tile> TileDict {
      get => this.GameClient.Map.Tiles.ToDictionary(tile => tile.Coordinate);
    }


    public void HandlePlayerInput(object sender, InputEventNames evt, object obj = null) {
      #region Change card orientation
      if (this.SelectedCardId != Guid.Empty) {
        if (evt == InputEventNames.RotateRightButton) {
          this.OpenCardsDict[this.SelectedCardId].unitFormation.RotateRight();
          this.SetHoveringTiles(this.hoveringTile);
        }
        if (evt == InputEventNames.RotateLeftButton) {
          this.OpenCardsDict[this.SelectedCardId].unitFormation.RotateLeft();
          this.SetHoveringTiles(this.hoveringTile);
        }
        if (evt == InputEventNames.MirrorHorizontalButton) {
          this.OpenCardsDict[this.SelectedCardId].unitFormation.MirrorHorizontal();
          this.SetHoveringTiles(this.hoveringTile);
        }
        if (evt == InputEventNames.MirrorVerticalButton) {
          this.OpenCardsDict[this.SelectedCardId].unitFormation.MirrorVertical();
          this.SetHoveringTiles(this.hoveringTile);
        }
      }
      #endregion

      #region Tile events
      if (typeof(TileView) == sender?.GetType()) {
        Tile tile = this.TileDict[(Coordinate)obj];

        // Place tile
        if (evt == InputEventNames.OnMouseUp) {
          if (this.SelectedCardId != Guid.Empty
            && this.OpenCardsDict.ContainsKey(this.SelectedCardId)
            && this.OpenCardsDict[this.SelectedCardId] != null) {
            UnitFormation untiFormation = this.OpenCardsDict[this.SelectedCardId].unitFormation;
            NetMakeMove mm = new NetMakeMove();
            mm.row = tile.Row;
            mm.col = tile.Col;
            mm.unitFormationId = UnitFormation.unitFormationTypeToIdDict[untiFormation.Type];
            mm.rotation = untiFormation.rotation;
            mm.mirrorHorizontal = untiFormation.mirrorHorizontal ? 1 : 0;
            mm.mirrorVertical = untiFormation.mirrorVertical ? 1 : 0;
            mm.teamId = this.currentTeam;
            RelayNetworking.Instance.SendToServer(mm);
          }
          if (this.SinglePieceId != Guid.Empty) {
            if (this.GameClient.GameSettings.PlayerIdToPlayerDict[this.GameClient.ActivePlayerId].SinglePieces > 0) {
              NetPutSinglePiece psp = new NetPutSinglePiece();
              psp.coordinate = tile.Coordinate;
              RelayNetworking.Instance.SendToServer(psp);
            }
          }
        }

        // Set hovering tile
        if (evt == InputEventNames.OnMouseEnter) {
          this.SetHoveringTiles(tile);
          return;
        }

        if (evt == InputEventNames.OnMouseExit) {
          this.SetHoveringTiles(null);
        }
      }
      #endregion

      #region Card events
      if (typeof(CardView) == sender?.GetType()) {
        Guid cardId = (Guid)obj;
        if (evt == InputEventNames.SelectCard) {
          this.SelectedCardId = cardId;
          this.SetHoveringTiles(this.HoveringTile);
        }
      }
      // if (evt == InputEventNames.SelectCard) {
      //   int selectedCardIdx = obj as int? ?? default(int);
      //   this.SelectedCardId = this.GameClient.OpenCards[selectedCardIdx].Id;
      //   this.SinglePieceId = Guid.Empty;
      //   
      // }
      #endregion

      #region SinglePiece event
      if (typeof(SinglePieceView) == sender?.GetType()) {
        if (this.GameClient.GameSettings.PlayerIdToPlayerDict[this.GameClient.ActivePlayerId].SinglePieces > 0) {
          this.SinglePieceId = (Guid)obj;
          this.SelectedCardId = Guid.Empty;
          Debug.Log(this.SinglePieceId);
        }
      }
      #endregion
    }

    #region UI
    private void SetHoveringTiles(Tile tile) {
      if (tile == null) {
        this.HoveringTiles = new List<Tile>();
        this.ViewManager.Render(RenderTypes.Highlight);
        return;
      }
      this.hoveringTile = tile;
      this.HoveringTiles = new List<Tile>();
      if (this.SelectedCardId == Guid.Empty) {
        if (this.SinglePieceId == Guid.Empty) {
          return;
        }
        this.HoveringTiles.Add(tile);
        this.ViewManager.Render(RenderTypes.Highlight);
        return;
      }
      Arrangement arrangement = this.OpenCardsDict[this.SelectedCardId].unitFormation.Arrangement;
      if (arrangement == null) {
        return;
      }
      foreach (Coordinate offset in arrangement) {
        Coordinate c = tile.Coordinate + offset;
        if (this.GameClient.Map.CoordinateToTileDict.ContainsKey(c)) {
          Tile t = this.GameClient.Map.CoordinateToTileDict[c];
          this.HoveringTiles.Add(t);
        }
      }
      this.ViewManager.Render(RenderTypes.Highlight);
    }
    #endregion

    #region Networking
    // Setup and cleanup
    private void Awake() {
      this.RegisterEvents();
      this.ViewManager = GameObject.Find("UIView").GetComponent<StandardGameViewManager>();
    }

    private void Update() {
      if (this.gameStarted && !this.GameClient.GameOver) {
        if (this.Timer > 0) {
          this.Timer -= Time.deltaTime;
        } else {
          this.Timer = 0;
        }
        this.ViewManager.Render(RenderTypes.Timer);
      }
    }

    private void OnDestroy() {
      this.UnregisterEvents();
    }
    private void RegisterEvents() {
      NetUtility.C_WELCOME += this.OnWelcome;
      NetUtility.C_ALL_PLAYERS_CONNECTED += this.OnAllPlayersConnected;
      NetUtility.C_INITIALISE_MAP += this.OnInitialiseMap;
      NetUtility.C_UPDATE_TERRAINS += this.OnUpdateTerrains;
      NetUtility.C_UPDATE_UNITS += this.OnUpdateUnits;
      NetUtility.C_UPDATE_CARDS += this.OnUpdateCards;
      NetUtility.C_RENDER += this.OnRender;
      NetUtility.C_END_TURN += this.OnEndTurn;
      NetUtility.C_UPDATE_SCORE += this.OnUpdateScore;
      NetUtility.C_UPDATE_SINGLE_PIECES += this.OnUpdateSinglePieces;
      NetUtility.C_GAME_OVER += this.OnGameOver;
    }
    private void UnregisterEvents() {
      NetUtility.C_WELCOME -= this.OnWelcome;
      NetUtility.C_ALL_PLAYERS_CONNECTED -= this.OnAllPlayersConnected;
      NetUtility.C_INITIALISE_MAP -= this.OnInitialiseMap;
      NetUtility.C_UPDATE_TERRAINS -= this.OnUpdateTerrains;
      NetUtility.C_UPDATE_UNITS -= this.OnUpdateUnits;
      NetUtility.C_UPDATE_CARDS -= this.OnUpdateCards;
      NetUtility.C_RENDER -= this.OnRender;
      NetUtility.C_END_TURN -= this.OnEndTurn;
      NetUtility.C_UPDATE_SCORE -= this.OnUpdateScore;
      NetUtility.C_UPDATE_SINGLE_PIECES -= this.OnUpdateSinglePieces;
      NetUtility.C_GAME_OVER -= this.OnGameOver;
    }

    private void OnUpdateSinglePieces(NetMessage msg) {
      NetUpdateSinglePieces usp = msg as NetUpdateSinglePieces;
      this.GameClient.GameSettings.PlayerIdToPlayerDict[PlayerIds.Player1].SetSinglePiece(usp.SinglePiecesPlayer1);
      this.GameClient.GameSettings.PlayerIdToPlayerDict[PlayerIds.Player2].SetSinglePiece(usp.SinglePiecesPlayer2);
    }

    /// <summary>
    /// The server accepted the connection and assigned a team to the client
    /// </summary>
    /// <param name="msg"></param>
    private void OnWelcome(NetMessage msg) {
      NetWelcome nw = msg as NetWelcome;
      this.currentTeam = nw.AssignedTeam;
      this.AssignedRole = nw.AssignedRole;
      this.ActivateViewManager();
    }

    private void OnAllPlayersConnected(NetMessage msg) {
      this.ReadyToStartGame = true;
      Debug.Log(this.AssignedRole);
      this.ViewManager.Render(RenderTypes.GameSettings);
    }

    private void OnRender(NetMessage msg) {
      NetRender r = msg as NetRender;
      r.renderTypes.ForEach((Action<RenderTypes>)(renderType => {
        this.ViewManager.Render(renderType);
      }));
    }

    private void OnGameOver(NetMessage msg) {
      NetGameOver go = msg as NetGameOver;
      this.GameClient.GameOver = true;
      this.ViewManager.CardsView.gameObject.SetActive(false);
      this.ViewManager.TurnsView.gameObject.SetActive(false);
      this.ViewManager.GameSettingsView.gameObject.SetActive(false);
      this.ViewManager.TimerView.gameObject.SetActive(false);

      Player winner = this.GameClient.Players.Find(player => player.Id == go.winnerId);
      if (winner == null) {
        this.ViewManager.GameOverView.Result.text = "It's a draw";
      } else {
        this.ViewManager.GameOverView.Result.text = winner.PlayerName + " won!";
      }
      this.ViewManager.Render(RenderTypes.GameOver);
    }
    private void OnUpdateCards(NetMessage msg) {
      NetUpdateCards uc = msg as NetUpdateCards;
      this.GameClient.OpenCards = uc.cards;
      this.selectedCardId = Guid.Empty;
    }

    private void OnInitialiseMap(NetMessage msg) {
      NetInitialiseMap im = msg as NetInitialiseMap;

      this.gameSettings = new GameSettings(im.mapSize, im.partsGrass, im.partsStone, im.partsWater, im.partsBush, 3, 0, im.numberOfStages, im.secondsPerTurn, im.ruleNames);
      this.GameClient = new Schnoz(gameSettings);
      // this.ActivateViewManager();
      this.Timer = this.GameClient.GameSettings.SecondsPerTurn;
      this.gameStarted = true;
      this.GameClient.InitialiseEmptyMap();
      this.ViewManager.SetCamera();
    }

    private void OnUpdateUnits(NetMessage msg) {
      NetUpdateUnits uu = msg as NetUpdateUnits;

      foreach (Unit unit in uu.addedUnits) {
        this.GameClient.PlaceUnit(unit.OwnerId, unit.Coordinate);
      }

      foreach (Coordinate coordinate in uu.removedUnitsCoordinates) {
        this.GameClient.RemoveUnit(coordinate);
      }
    }

    private void OnUpdateTerrains(NetMessage msg) {
      NetUpdateTerrains ut = msg as NetUpdateTerrains;
      foreach (Terrain terrain in ut.terrains) {
        this.GameClient.PlaceTerrain(terrain.Type, terrain.Coordinate);
      }
    }

    private void OnUpdateScore(NetMessage msg) {
      NetUpdateScore us = msg as NetUpdateScore;
      this.GameClient.GameSettings.PlayerIdToPlayerDict[PlayerIds.Player1].SetScore(us.ScorePlayer1);
      this.GameClient.GameSettings.PlayerIdToPlayerDict[PlayerIds.Player2].SetScore(us.ScorePlayer2);
    }

    private void OnEndTurn(NetMessage msg) {
      NetEndTurn et = msg as NetEndTurn;
      this.Timer = this.GameClient.GameSettings.SecondsPerTurn;
      this.GameClient.EndTurn();
    }

    #endregion
    private void ActivateViewManager() {
      this.ViewManager.enabled = true;
      this.ViewManager.game = this;
    }
  }
}
