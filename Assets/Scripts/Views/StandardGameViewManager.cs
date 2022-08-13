using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using Schnoz;
using TMPro;

public class StandardGameViewManager : MonoBehaviour {
  public StandardGameClient game;
  public StandingView StandingView;
  public CardsView CardsView;
  public TurnsView TurnsView;
  public ActionsView ActionsView;
  public TimerView TimerView;
  public GameSettingsView GameSettingsView;
  public GameOverView GameOverView;
  private Vector2 resolution;
  private GameObject mapGO;

  [SerializeField] private GameObject openCardsGO;
  [SerializeField] private GameObject rulesGO;
  [SerializeField] private List<GameObject> units;
  [SerializeField] private List<GameObject> terrains;

  [SerializeField] private List<TextMeshProUGUI> scores;

  #region Camera movement properties
  private Camera mainCam;
  [SerializeField] private float zoomMaxSize, zoomMinSize = 2f;
  [SerializeField] private Vector3 panStart, mousePositionWorldPoint;
  public bool IsPanning { get; private set; }
  #endregion

  #region Sprites
  [SerializeField] private Sprite fogSprite;
  [SerializeField] private Sprite bobSprite;
  [SerializeField] private Sprite sigeneSprite;
  [SerializeField] private Sprite mauriceSprite;
  [SerializeField] private Sprite houseSprite;
  [SerializeField] private Sprite grassTileSprite;
  [SerializeField] private Sprite terrainWater;
  [SerializeField] private Sprite terrainBush;
  [SerializeField] private Sprite terrainStone;
  #endregion

  private void Awake() {
    this.fogSprite = Array.Find(Resources.LoadAll<Sprite>("Sprites/Tiles"), s => s.name == "fog");
    this.bobSprite = Resources.Load<Sprite>("Sprites/bob");
    this.sigeneSprite = Resources.Load<Sprite>("Sprites/sigene");
    this.mauriceSprite = Resources.Load<Sprite>("Sprites/maurice");
    this.houseSprite = Resources.Load<Sprite>("Sprites/house");
    var tiles = Resources.LoadAll<Sprite>("Sprites/Tiles");
    this.grassTileSprite = Array.Find(tiles, s => s.name == "terrain_grass");
    this.terrainWater = Array.Find(tiles, s => s.name == "terrain_water");
    this.terrainBush = Array.Find(tiles, s => s.name == "terrain_bush");
    this.terrainStone = Array.Find(tiles, s => s.name == "terrain_stone");
  }
  private void Update() {
    if (this.game.GameClient == null) {
      return;
    }
    if (this.game.GameClient.GameOver && Input.GetKeyDown(KeyCode.Escape)) {
      SchnozSceneManager.Instance.ChangeScene(SceneIndexes.GAME, SceneIndexes.MAIN_MENU);
      Client.Instance.Shutdown();
      Server.Instance.Shutdown();
    }

    #region Camera movement update
    this.mousePositionWorldPoint = this.mainCam.ScreenToWorldPoint(Input.mousePosition);
    this.Pan();
    this.Zoom();
    #endregion

    #region Player input
    if (Input.GetKeyDown(KeyCode.E)) {
      this.game.HandlePlayerInput(this, InputEventNames.RotateRightButton);
    }
    if (Input.GetKeyDown(KeyCode.Q)) {
      this.game.HandlePlayerInput(this, InputEventNames.RotateLeftButton);
    }
    if (Input.GetKeyDown(KeyCode.W)) {
      this.game.HandlePlayerInput(this, InputEventNames.MirrorHorizontalButton);
    }
    if (Input.GetKeyDown(KeyCode.S)) {
      this.game.HandlePlayerInput(this, InputEventNames.MirrorVerticalButton);
    }
    #endregion
  }
  public void Zoom() {
    var scroll = Input.GetAxis("Mouse ScrollWheel");
    if (scroll == 0) {
      return;
    }
    // var zoomDirection = scroll > 0 ? -1 : -1;
    float orthographicSizeAfterZoom = this.mainCam.orthographicSize - scroll;
    this.mainCam.orthographicSize =
      orthographicSizeAfterZoom > this.zoomMaxSize ? this.zoomMaxSize :
      orthographicSizeAfterZoom < this.zoomMinSize ? this.zoomMinSize : orthographicSizeAfterZoom;
  }
  public void Pan() {
    if (Input.GetMouseButtonDown(0)) {
      this.panStart = this.mousePositionWorldPoint;
    }
    if (Input.GetMouseButton(0)) {
      Vector3 delta = this.panStart - this.mousePositionWorldPoint;
      if (delta != Vector3.zero) {
        if (!this.IsPanning) {
          this.IsPanning = true;
        }
        this.mainCam.transform.position += delta;
      }
    }
    if (Input.GetMouseButtonUp(0)) {
      this.IsPanning = false;
    }
  }
  public void Render(RenderTypes renderType) {
    var gameScene = SceneManager.GetSceneByBuildIndex((int)SceneIndexes.GAME);
    SceneManager.SetActiveScene(gameScene);
    switch (renderType) {
      case RenderTypes.Map: {
          this.RenderMap();
          break;
        }
      case RenderTypes.GameOver: {
          this.GameOverView.GameClient = this.game;
          this.GameOverView.Render();
          break;
        }
      case RenderTypes.GameSettings: {
          this.GameSettingsView.GameClient = this.game;
          this.GameSettingsView.Render();
          break;
        }
      case RenderTypes.Turns: {
          this.RenderTurns();
          break;
        }
      case RenderTypes.Timer: {
          this.TimerView.GameClient = this.game;
          this.TimerView.Render();
          break;
        }
      case RenderTypes.Highlight: {
          this.RenderHighlights();
          break;
        }
      case RenderTypes.OpenCards: {
          this.CardsView.GameClient = this.game;
          this.CardsView.Render();
          break;
        }
      case RenderTypes.SelectedCard: {
          this.CardsView.GameClient = this.game;
          this.CardsView.Render();
          break;
        }
      case RenderTypes.Rules: {
          this.RenderRuleStanding();
          break;
        }
      case RenderTypes.Score: {
          this.RenderRuleStanding();
          break;
        }
      case RenderTypes.SinglePieces: {
          this.RenderSinglePieces();
          break;
        }
      default:
        throw new Exception("Unknown Render Type");
    }

  }

  private void RenderTurns() {
    this.TurnsView.GameClient = this.game;
    this.TurnsView.Render();
  }

  public void SetCamera() {
    #region Camera movement setup
    this.mainCam = Camera.main;
    var visibleTiles = this.game.GameClient.Map.Tiles.Where(tile => tile.Visible);
    var min = visibleTiles.Min(tile => tile.Coordinate.col);
    var max = visibleTiles.Max(tile => tile.Coordinate.col);
    float nVisibleCols = (float)(max - min);
    float visibleArea = (nVisibleCols + 1);
    float boardSize = this.game.GameClient.GameSettings.NCols;
    float initialZoomSize = 1.3f * nVisibleCols / 2;
    this.mainCam.orthographicSize = initialZoomSize;
    this.mainCam.transform.position = new Vector3(boardSize / 2, boardSize / 2, -10);
    this.zoomMaxSize = 1 + boardSize / 2;
    #endregion

    // var singlePiecePlayer1 = GameObject.Find($"UIView/ActionsView/Player").GetComponent<SinglePieceView>();
    // singlePiecePlayer1.game = this.game;
  }
  private GameObject RenderTile(Tile tile) {
    GameObject tileGO = new GameObject($"{tile.Coordinate}");
    SpriteRenderer sr = tileGO.AddComponent<SpriteRenderer>();
    sr.sprite = this.grassTileSprite;
    TileView tileView = tileGO.AddComponent<TileView>();
    tileView.game = this.game;
    tileView.viewManager = this;
    tileView.coordinate = tile.Coordinate;
    return tileGO;
  }
  private GameObject RenderUnit(Unit unit) {
    GameObject unitGO = new GameObject($"Unit ({unit.OwnerId})");
    SpriteRenderer sr = unitGO.AddComponent<SpriteRenderer>();
    Sprite unitSprite;
    switch (unit.OwnerId) {
      case PlayerIds.Player1: { unitSprite = this.bobSprite; break; }
      case PlayerIds.Player2: { unitSprite = this.mauriceSprite; break; }
      case PlayerIds.NeutralPlayer: { unitSprite = this.houseSprite; break; }
      default: { unitSprite = null; break; }
    }
    sr.sprite = unitSprite;
    sr.sortingOrder = 10;
    return unitGO;
  }
  private GameObject RenderUnitHover(Unit unit) {
    var unitGO = this.RenderUnit(unit);
    unitGO.name = "HoverUnit";
    unitGO.tag = "HoverUnit";
    unitGO.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.3f);
    SpriteRenderer sr = unitGO.GetComponent<SpriteRenderer>();
    sr.sortingOrder = 5;
    return unitGO;
  }
  private GameObject RenderTerrain(Schnoz.Terrain terrain) {
    GameObject terrainGO = new GameObject($"{terrain.Type.ToString()}");
    SpriteRenderer sr = terrainGO.AddComponent<SpriteRenderer>();
    switch (terrain.Type) {
      case TerrainType.Water: {
          sr.sprite = this.terrainWater;
          break;
        }
      case TerrainType.Bush: {
          sr.sprite = this.terrainBush;
          break;
        }
      case TerrainType.Stone: {
          sr.sprite = this.terrainStone;
          break;
        }
      default: {
          break;
        }
    }

    sr.sortingOrder = 5;
    return terrainGO;
  }
  private GameObject RenderFog(Tile tile) {
    GameObject fogGO = new GameObject("Fog");
    SpriteRenderer sr = fogGO.AddComponent<SpriteRenderer>();
    sr.sprite = this.fogSprite;
    if (this.game.GameClient.Map.GetAdjacentTiles(tile).Any(t => t.Visible)) {
      sr.color = new Color(1, 1, 1, 0.5f);
    }
    sr.sortingOrder = 20;
    return fogGO;
  }

  private void RenderSinglePieces() {
    Debug.Log("RenderSinglePieces: NotYetImplemented");
    // string path = $"UI/SinglePieces/Player/Value";
    // GameObject.Find(path).GetComponent<TextMeshProUGUI>().text = this.game.GameClient.GameSettings.PlayerIdToPlayerDict[this.game.GameClient.ActivePlayerId].SinglePieces.ToString();
  }
  private void RenderHighlights() {
    foreach (GameObject hoverUnit in GameObject.FindGameObjectsWithTag("HoverUnit")) {
      Destroy(hoverUnit);
    }

    foreach (Tile hoveringTile in this.game.HoveringTiles) {
      // sr.color = new Color(0.9f, 0.9f, 0.9f);
      GameObject fogGO = new GameObject("HoverUnit");
      fogGO.tag = "HoverUnit";
      fogGO.transform.SetParent(GameObject.Find($"Map/{hoveringTile.Coordinate}").transform);
      fogGO.transform.localPosition = new Vector2(0, 0);
      SpriteRenderer sr = fogGO.AddComponent<SpriteRenderer>();
      sr.sprite = this.fogSprite;
      sr.color = new Color(1, 1, 1, 0.1f);

      GameObject hoveringUnitGO = this.RenderUnitHover(new Unit(this.game.GameClient.ActivePlayerId, hoveringTile.Coordinate));
      hoveringUnitGO.transform.SetParent(GameObject.Find($"Map/{hoveringTile.Coordinate}").transform);
      hoveringUnitGO.transform.localPosition = new Vector2(0, 0);
    }

    // foreach (Tile placeableTile in this.game.PlaceableTiles) {
    // if (this.terrainGOs.Any(terrainGO => {
    //   terrainGO.GetComponent<Terrain()
    // }))
    // }
  }
  private void RenderMap() {
    if (this.mapGO != null) {
      Destroy(mapGO);
    }
    this.mapGO = new GameObject("Map");
    this.units = new List<GameObject>();
    this.terrains = new List<GameObject>();

    this.game.GameClient.Map.Tiles.ForEach((Tile tile) => {
      GameObject tileGO = this.RenderTile(tile);
      tileGO.transform.SetParent(this.mapGO.transform);
      tileGO.transform.localPosition = new Vector2(tile.Coordinate.col, tile.Coordinate.row);
      if (tile.Unit != null) {
        GameObject unit = this.RenderUnit(tile.Unit);
        unit.transform.SetParent(tileGO.transform);
        unit.transform.localPosition = new Vector2(0, 0);
        units.Add(unit);
      }

      if (tile.Terrain.Type != TerrainType.Grass) {
        GameObject terrain = this.RenderTerrain(tile.Terrain);
        terrain.transform.SetParent(tileGO.transform);
        terrain.transform.localPosition = new Vector2(0, 0);
        terrains.Add(terrain);
      }

      if (!tile.Visible) {
        GameObject fogGO = this.RenderFog(tile);
        fogGO.transform.SetParent(tileGO.transform);
        fogGO.transform.localPosition = new Vector2(0, 0);
      }
    });
  }
  private void RenderRuleStanding() {
    this.StandingView.GameClient = this.game;
    this.StandingView.Render();
  }
}
