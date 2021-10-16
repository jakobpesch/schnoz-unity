using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using UnityEngine;
using Utils;
using Schnoz;
using TMPro;

public class StandardGameViewManager : MonoBehaviour {
  public StandardGameClient game;
  private Vector2 resolution;
  private GameObject mapGO;
  [SerializeField] private GameObject openCardsGO;
  [SerializeField] private GameObject rulesGO;

  [SerializeField] private List<TextMeshProUGUI> scores;

  #region Camera movement properties
  private Camera mainCam;
  [SerializeField] private float zoomMaxSize, zoomMinSize = 2f;
  [SerializeField] private Vector3 panStart, mousePositionWorldPoint;
  public bool IsPanning { get; private set; }
  #endregion

  private void Awake() {
    this.resolution = new Vector2(Screen.width, Screen.height);

  }
  private void Update() {
    #region Camera movement update
    this.mousePositionWorldPoint = this.mainCam.ScreenToWorldPoint(Input.mousePosition);
    this.Pan();
    this.Zoom();
    #endregion

    #region On resize
    if (resolution.x != Screen.width || resolution.y != Screen.height) {
      resolution.x = Screen.width;
      resolution.y = Screen.height;
      this.RenderOpenCards(resolution);
    }
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
    if (Input.GetKeyDown(KeyCode.Alpha1)) {
      this.game.HandlePlayerInput(this, InputEventNames.SelectCard, 2);
    }
    if (Input.GetKeyDown(KeyCode.Alpha2)) {
      this.game.HandlePlayerInput(this, InputEventNames.SelectCard, 1);
    }
    if (Input.GetKeyDown(KeyCode.Alpha3)) {
      this.game.HandlePlayerInput(this, InputEventNames.SelectCard, 0);
    }
    #endregion
  }
  public void Zoom() {
    var scroll = Input.GetAxis("Mouse ScrollWheel");
    if (scroll == 0) {
      return;
    }
    // var zoomDirection = scroll > 0 ? -1 : -1;
    float orthographicSizeAfterZoom = this.mainCam.orthographicSize + scroll;
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
  public void Render(object sender, PropertyChangedEventArgs e) {
    // Debug.Log($"{this} was notified about change in {e.PropertyName}.");
    // Debug.Log($"Render {e}");
    if (e.PropertyName == "Map") {
      this.RenderMap();
      // StartCoroutine(this.RenderMapSlowly());
    }
    if (e.PropertyName == "Highlight") {
      this.RenderHighlights();
      // StartCoroutine(this.RenderMapSlowly());
    }
    if (e.PropertyName == "OpenCards") {
      this.RenderOpenCards(new Vector2(Screen.width, Screen.height));
      // StartCoroutine(this.RenderMapSlowly());
    }
    if (e.PropertyName == "SelectedCard") {
      this.RenderOpenCards(new Vector2(Screen.width, Screen.height));
    }
    if (e.PropertyName == "Rules") {
      this.RenderRuleStanding();
    }
    if (e.PropertyName == "Score") {
      this.RenderScore();
    }
    if (e.PropertyName == "CurrentPlayer") {
      this.CurrentPlayer();
    }
  }

  private void CurrentPlayer() {
    this.game.GameClient.Players.ForEach(player => {
      string charPath = $"UI/Points/Score Details/Player {player.Id + 1}/Character";
      var scale = this.game.GameClient.ActivePlayerId == player.Id ? 110 : 90;
      GameObject.Find(charPath).transform.localScale = new Vector3(scale, scale, scale);
    });
  }

  private void Start() {
    #region Camera movement setup
    this.mainCam = Camera.main;
    float nCols = (float)this.game.GameClient.GameSettings.NCols;
    float boardSize = (nCols + 1);
    float initialZoomSize = 1.3f * nCols / 2;
    this.mainCam.orthographicSize = initialZoomSize;
    this.mainCam.transform.position = new Vector3(nCols / 2, nCols / 2, -10);
    this.zoomMaxSize = 1 + boardSize / 2;
    #endregion

    #region Cards UI
    // CreateCardsUI();
    #endregion

    // this.scores = new List<TextMeshProUGUI>();
    // this.game.GameClient.gameSettings
    //    .Players.ForEach(player => this.game.GameClient.gameSettings
    //    .Rules.ForEach(rule =>
    //        {
    //          Debug.Log("TESTSTSTTSTSTS");
    //          var s = GameObject.Instantiate(Resources.Load<GameObject>("Prefab/Score")).GetComponent<TextMeshProUGUI>();
    //          this.scores.Add(s);
    //          s.transform.SetParent(GameObject.Find("UI").transform);
    //          RectTransform rect = s.gameObject.AddComponent<RectTransform>();
    //          rect.anchorMin = new Vector2(0, 1);
    //          rect.anchorMax = new Vector2(0, 1);
    //          s.transform.localPosition = new Vector3(0, 0, 0);
    //          rect.anchoredPosition = new Vector3(Screen.width * 0.01f, Screen.width * 0.01f, 0);
    //          rect.sizeDelta = new Vector2(100, 100);
    //        }
    //        ));

  }
  public void StartListening() {
    this.game.GameClient.PropertyChanged -= new PropertyChangedEventHandler(this.Render);
    this.game.GameClient.PropertyChanged += new PropertyChangedEventHandler(this.Render);
  }

  private void OnDestroy() {
    this.game.GameClient.PropertyChanged -= new PropertyChangedEventHandler(this.Render);
  }

  private GameObject RenderTile(Tile tile) {
    // Debug.Log($"Render Tile {tile.Coordinate}");
    GameObject tileGO = new GameObject($"{tile.Coordinate}");
    SpriteRenderer sr = tileGO.AddComponent<SpriteRenderer>();
    sr.sprite = Array.Find(Resources.LoadAll<Sprite>("Sprites/Tiles"), s => s.name == "terrain_grass");
    TileView tileView = tileGO.AddComponent<TileView>();
    tileView.game = this.game;
    tileView.viewManager = this;
    tileView.coordinate = tile.Coordinate;
    return tileGO;
  }

  private GameObject RenderUnit(Unit unit) {
    GameObject unitGO = new GameObject($"Player {unit.OwnerId}'s unit.");
    SpriteRenderer sr = unitGO.AddComponent<SpriteRenderer>();
    string untiSpritePath;
    switch (unit.OwnerId) {
      case 0: { untiSpritePath = "Sprites/bob"; break; }
      case 1: { untiSpritePath = "Sprites/sigene"; break; }
      case 2: { untiSpritePath = "Sprites/house"; break; }
      default: { untiSpritePath = ""; break; }
    }
    sr.sprite = Resources.Load<Sprite>(untiSpritePath);
    sr.sortingOrder = 10;
    return unitGO;
  }
  private GameObject RenderTerrain(Schnoz.Terrain terrain) {
    GameObject terrainGO = new GameObject($"{terrain.Type.ToString()}");
    SpriteRenderer sr = terrainGO.AddComponent<SpriteRenderer>();
    switch (terrain.Type) {
      case TerrainType.Water: {
          sr.sprite = Array.Find(Resources.LoadAll<Sprite>("Sprites/Tiles"), s => s.name == "terrain_water");
          break;
        }
      case TerrainType.Bush: {
          sr.sprite = Array.Find(Resources.LoadAll<Sprite>("Sprites/Tiles"), s => s.name == "terrain_bush");
          break;
        }
      case TerrainType.Stone: {
          sr.sprite = Array.Find(Resources.LoadAll<Sprite>("Sprites/Tiles"), s => s.name == "terrain_stone");
          break;
        }
      default: {
          break;
        }
    }

    sr.sortingOrder = 5;
    return terrainGO;
  }
  private GameObject RenderGround() {
    GameObject terrainGO = new GameObject("Grass");
    SpriteRenderer sr = terrainGO.AddComponent<SpriteRenderer>();
    sr.sprite = Array.Find(Resources.LoadAll<Sprite>("Sprites/Tiles"), s => s.name == "terrain_grass");
    sr.sortingOrder = 4;
    return terrainGO;
  }

  private void RenderHighlights() {
    foreach (Transform child in this.mapGO.transform) {
      TileView tileView = child.GetComponent<TileView>();
      if (this.game.TileDict.ContainsKey(tileView.coordinate)) {
        Tile tile = this.game.TileDict[tileView.coordinate];
        SpriteRenderer sr = tileView.GetComponent<SpriteRenderer>();
        sr.color = this.game.HoveringTiles.Any(t => t.Coordinate == tile.Coordinate) ? new Color(0.9f, 0.9f, 0.9f) : Color.white;
      }
    }
  }

  private void RenderMap() {
    // Debug.Log("Rendering Map");
    if (this.mapGO != null) {
      Destroy(mapGO);
    }
    this.mapGO = new GameObject("Map");

    this.game.GameClient.Map.Tiles.ForEach((Tile tile) => {
      GameObject tileGO = this.RenderTile(tile);
      tileGO.transform.SetParent(this.mapGO.transform);
      tileGO.transform.localPosition = new Vector2(tile.Coordinate.col, tile.Coordinate.row);
      if (tile.Unit != null) {
        GameObject unitGO = this.RenderUnit(tile.Unit);
        unitGO.transform.SetParent(tileGO.transform);
        unitGO.transform.localPosition = new Vector2(0, 0);
      }

      if (tile.Terrain.Type != TerrainType.Grass) {
        GameObject terrainGO = this.RenderTerrain(tile.Terrain);
        terrainGO.transform.SetParent(tileGO.transform);
        terrainGO.transform.localPosition = new Vector2(0, 0);
      }
    });
    // Debug.Log("Rendered Map successfully!");
  }
  private IEnumerator RenderMapSlowly(float interval = 0.1f) {
    // Debug.Log("Rendering Map");
    GameObject mapGO = GameObject.Find("Map");
    Destroy(mapGO);
    mapGO = new GameObject("Map");

    foreach (Tile tile in this.game.GameClient.Map.Tiles) {
      yield return new WaitForSeconds(interval);
      GameObject tileGO = this.RenderTile(tile);
      tileGO.transform.SetParent(mapGO.transform);
      tileGO.transform.localPosition = new Vector2(tile.Coordinate.col, tile.Coordinate.row);
      if (tile.Unit != null) {
        GameObject unitGO = this.RenderUnit(tile.Unit);
        unitGO.transform.SetParent(tileGO.transform);
        unitGO.transform.localPosition = new Vector2(0, 0);
      }
    }

    // Debug.Log("Rendered Map successfully!");
  }

  private GameObject RenderCard(Card card) {
    GameObject cardGO = new GameObject($"{card.Type}");
    SpriteRenderer sr = cardGO.AddComponent<SpriteRenderer>();
    if (this.game.SelectedCardId == card.Id) {
      sr.color = Color.green;
    }
    string fileName = card.Type.ToString();
    sr.sprite = Resources.Load<Sprite>($"Sprites/{fileName}");
    sr.sortingOrder = 20;
    return cardGO;
  }
  private void RenderOpenCards(Vector2 resolution) {
    Debug.Log("Rendering Open Cards");
    if (this.openCardsGO != null) {
      Debug.Log("Destroying OpenCardsGo");
      GameObject.Destroy(this.openCardsGO);
    }
    this.CreateCardsUI(resolution);

    int index = 0;
    this.game.GameClient.OpenCards.ForEach(card => {
      GameObject cardGO = this.RenderCard(card);
      cardGO.transform.SetParent(this.openCardsGO.transform);
      cardGO.transform.localScale = Vector3.one;
      cardGO.transform.localPosition = new Vector2(0, (float)index++);
      CardView cardView = cardGO.AddComponent<CardView>();
      cardView.game = this.game;
      cardView.cardId = card.Id;
    });
  }
  private void CreateCardsUI(Vector2 resolution) {
    Debug.Log("Creating Open Cards GO");
    this.openCardsGO = new GameObject("OpenCards");
    this.openCardsGO.transform.SetParent(GameObject.Find("UI").transform);
    RectTransform rect = openCardsGO.AddComponent<RectTransform>();
    rect.anchorMin = new Vector2(0, 0);
    rect.anchorMax = new Vector2(0, 0);
    openCardsGO.transform.localPosition = new Vector3(0, 0, 0);
    rect.anchoredPosition = new Vector3(Screen.height * 0.1f, Screen.height * 0.1f, 0);
    rect.sizeDelta = Vector2.one;
    rect.localScale = new Vector2(Screen.height * 0.1f, Screen.height * 0.1f);
  }

  private void CreateRulesUI() {

    // Debug.Log("Creating Rules GO");
    // this.scores.ForEach(scoreText =>
    // {
    //   scoreText.SetText("Score");
    // });
    // this.rulesGO = new GameObject("Rules");
    // this.rulesGO.transform.SetParent(GameObject.Find("UI").transform);
    // RectTransform rect = rulesGO.AddComponent<RectTransform>();
    // rect.anchorMin = new Vector2(0, 1);
    // rect.anchorMax = new Vector2(0, 1);
    // rulesGO.transform.localPosition = new Vector3(0, 0, 0);
    // rect.anchoredPosition = new Vector3(Screen.width * 0.01f, Screen.width * 0.01f, 0);
    // rect.sizeDelta = new Vector2(100, 100);
  }

  private void RenderRuleStanding() {
    var gc = this.game.GameClient;

    gc.Players.ForEach(player => {
      gc.GameSettings.Rules.ForEach(rule => {
        RuleEvaluation eval = rule.Evaluate(player, gc.Map);
        string pathRule = $"UI/Points/Rule Details/VStack/{eval.RuleName}/Player{eval.PlayerId + 1}/Value";
        GameObject.Find(pathRule).GetComponent<TextMeshProUGUI>().text = eval.Points.ToString();
      }
     );
    });
  }

  private void RenderScore() {
    var gc = this.game.GameClient;

    gc.Players.ForEach(player => {
      int score = player.Score;
      string pathScore = $"UI/Points/Score Details/Player {player.Id + 1}/Value";
      GameObject.Find(pathScore).GetComponent<TextMeshProUGUI>().text = score.ToString();
    });
  }
}
