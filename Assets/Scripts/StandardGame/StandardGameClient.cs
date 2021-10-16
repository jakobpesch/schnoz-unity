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
    [SerializeField] private StandardGameViewManager viewManager;
    public Schnoz GameClient { get; private set; }
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
      }
    }
    public Dictionary<Guid, Card> OpenCardsDict {
      get => this.GameClient.OpenCards.ToDictionary(card => card.Id);
    }
    public Dictionary<Coordinate, Tile> TileDict {
      get => this.GameClient.Map.Tiles.ToDictionary(tile => tile.Coordinate);
    }
    public void HandlePlayerInput(object sender, InputEventNames evt, object obj = null) {
      #region Change card orientation
      if (this.SelectedCardId != null) {
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
          if (this.SelectedCardId != null
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
            Debug.Log(JsonUtility.ToJson(mm));
            Client.Instance.SendToServer(mm);
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
        if (evt == InputEventNames.OnMouseUp) {
          this.SelectedCardId = cardId;
          this.viewManager.Render(this, new PropertyChangedEventArgs("SelectedCard"));
        }
      }
      if (evt == InputEventNames.SelectCard) {
        int selectedCardIdx = obj as int? ?? default(int);
        this.SelectedCardId = this.GameClient.OpenCards[selectedCardIdx].Id;
        this.viewManager.Render(this, new PropertyChangedEventArgs("SelectedCard"));
      }
      #endregion
    }

    #region UI
    private void SetHoveringTiles(Tile tile) {
      if (tile == null) {
        this.HoveringTiles = new List<Tile>();
        this.viewManager.Render(this, new PropertyChangedEventArgs("Highlight"));
        return;
      }
      this.hoveringTile = tile;
      if (this.SelectedCardId == Guid.Empty) {
        return;
      }
      this.HoveringTiles = new List<Tile>();
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
      this.viewManager.Render(this, new PropertyChangedEventArgs("Highlight"));
    }
    #endregion

    #region Networking
    // Setup and cleanup
    private void Awake() {
      this.RegisterEvents();
    }
    private void OnDestroy() {
      this.UnregisterEvents();
    }
    private void RegisterEvents() {
      NetUtility.C_WELCOME += this.OnWelcome;
      NetUtility.C_INITIALISE_MAP += this.OnInitialiseMap;
      NetUtility.C_UPDATE_TERRAINS += this.OnUpdateTerrains;
      NetUtility.C_UPDATE_UNITS += this.OnUpdateUnits;
      NetUtility.C_UPDATE_CARDS += this.OnUpdateCards;
      NetUtility.C_RENDER += this.OnRender;
      NetUtility.C_END_TURN += this.OnEndTurn;
      NetUtility.C_UPDATE_SCORE += this.OnUpdateScore;
    }
    private void UnregisterEvents() {
      NetUtility.C_WELCOME -= this.OnWelcome;
      NetUtility.C_INITIALISE_MAP -= this.OnInitialiseMap;
      NetUtility.C_UPDATE_TERRAINS -= this.OnUpdateTerrains;
      NetUtility.C_UPDATE_UNITS -= this.OnUpdateUnits;
      NetUtility.C_UPDATE_CARDS -= this.OnUpdateCards;
      NetUtility.C_RENDER -= this.OnRender;
      NetUtility.C_END_TURN -= this.OnEndTurn;
      NetUtility.C_UPDATE_SCORE -= this.OnUpdateScore;
    }



    /// <summary>
    /// The server accepted the connection and assigned a team to the client
    /// </summary>
    /// <param name="msg"></param>
    private void OnWelcome(NetMessage msg) {
      Debug.Log("Message to Client: OnWelcome");
      NetWelcome nw = msg as NetWelcome;
      this.currentTeam = nw.AssinedTeam;
    }

    private void OnRender(NetMessage msg) {
      Debug.Log("Message to Client: OnRender");
      NetRender r = msg as NetRender;
      Debug.Log(r.renderTypes.Count);
      r.renderTypes.ForEach(r => Debug.Log(r));
      r.renderTypes.ForEach(renderType => {
        this.viewManager.Render(this, new PropertyChangedEventArgs(renderType.ToString()));
      });
      // this.viewManager.Render(this, new PropertyChangedEventArgs("Map"));
      // this.viewManager.Render(this, new PropertyChangedEventArgs("OpenCards"));

      // this.viewManager.Render(this, new PropertyChangedEventArgs("Rules"));

    }
    private void OnUpdateCards(NetMessage msg) {
      Debug.Log("Message to Client: OnUpdateCards");

      NetUpdateCards uc = msg as NetUpdateCards;
      this.GameClient.OpenCards = uc.cards;
      this.selectedCardId = Guid.Empty;
    }

    private void OnInitialiseMap(NetMessage msg) {
      Debug.Log("Message to Client: OnInitialiseMap");

      NetInitialiseMap im = msg as NetInitialiseMap;

      List<RuleNames> ruleNames = new List<RuleNames>();
      ruleNames.Add(RuleNames.DiagonalToTopRight);
      ruleNames.Add(RuleNames.Water);

      this.gameSettings = new GameSettings(im.nRows, im.nCols, 3, 0, 6, 60, ruleNames);
      this.GameClient = new Schnoz(gameSettings);
      this.CreateViewManager();

      this.GameClient.InitialiseMap();
    }

    private void OnUpdateUnits(NetMessage msg) {
      Debug.Log("Message to Client: OnUpdateUnits");
      NetUpdateUnits uu = msg as NetUpdateUnits;

      foreach (Unit unit in uu.addedUnits) {
        this.GameClient.PlaceUnit(unit.OwnerId, unit.Coordinate);
      }

      foreach (Coordinate coordinate in uu.removedUnitsCoordinates) {
        this.GameClient.RemoveUnit(coordinate);
      }
    }

    private void OnUpdateTerrains(NetMessage msg) {
      Debug.Log("Message to Client: OnUpdateTerrains");
      NetUpdateTerrains ut = msg as NetUpdateTerrains;
      foreach (Terrain terrain in ut.terrains) {
        this.GameClient.PlaceTerrain(terrain.Type, terrain.Coordinate);
      }
    }

    private void OnUpdateScore(NetMessage msg) {
      Debug.Log("Message to Client: OnUpdateScore");
      NetUpdateScore us = msg as NetUpdateScore;
      Debug.Log($"CLIENT: Score P1:{us.ScorePlayer1.ToString()}, Score P2: {us.ScorePlayer2.ToString()}");
      this.GameClient.GameSettings.IdToPlayerDict[0].SetScore(us.ScorePlayer1);
      this.GameClient.GameSettings.IdToPlayerDict[1].SetScore(us.ScorePlayer2);
    }

    private void OnEndTurn(NetMessage msg) {
      Debug.Log("Message to Client: OnEndTurn");
      NetEndTurn et = msg as NetEndTurn;
      this.GameClient.EndTurn();
    }

    #endregion
    private void CreateViewManager() {
      this.viewManager = this.gameObject.AddComponent<StandardGameViewManager>();
      this.viewManager.enabled = true;
      this.viewManager.game = this;
      this.viewManager.StartListening();
    }
  }
}
