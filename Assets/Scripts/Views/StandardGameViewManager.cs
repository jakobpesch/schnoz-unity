using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using UnityEngine;
using Utils;
using Schnoz;

public class StandardGameViewManager : MonoBehaviour
{
  public StandardGameClient game;
  private Vector2 resolution;
  private GameObject mapGO;
  [SerializeField] private GameObject openCardsGO;

  #region Camera movement properties
  private Camera mainCam;
  [SerializeField] private float zoomMaxSize, zoomMinSize = 2f;
  [SerializeField] private Vector3 panStart, mousePositionWorldPoint;
  #endregion

  private void Awake()
  {
    this.resolution = new Vector2(Screen.width, Screen.height);

  }
  private void Update()
  {
    #region Camera movement update
    this.mousePositionWorldPoint = this.mainCam.ScreenToWorldPoint(Input.mousePosition);
    this.Pan();
    this.Zoom();
    #endregion

    #region On resize
    if (resolution.x != Screen.width || resolution.y != Screen.height)
    {
      this.RenderOpenCards();

      resolution.x = Screen.width;
      resolution.y = Screen.height;
    }
    #endregion

    #region Player input
    if (Input.GetKeyDown(KeyCode.E))
    {
      this.game.HandlePlayerInput(this, InputEventNames.RotateRightButton);
    }
    if (Input.GetKeyDown(KeyCode.Q))
    {
      this.game.HandlePlayerInput(this, InputEventNames.RotateLeftButton);
    }
    if (Input.GetKeyDown(KeyCode.W))
    {
      this.game.HandlePlayerInput(this, InputEventNames.MirrorHorizontalButton);
    }
    if (Input.GetKeyDown(KeyCode.S))
    {
      this.game.HandlePlayerInput(this, InputEventNames.MirrorVerticalButton);
    }
    #endregion
  }
  public void Zoom()
  {
    var scroll = Input.GetAxis("Mouse ScrollWheel");
    if (scroll == 0)
    {
      return;
    }
    // var zoomDirection = scroll > 0 ? -1 : -1;
    float orthographicSizeAfterZoom = this.mainCam.orthographicSize + scroll;
    this.mainCam.orthographicSize =
      orthographicSizeAfterZoom > this.zoomMaxSize ? this.zoomMaxSize :
      orthographicSizeAfterZoom < this.zoomMinSize ? this.zoomMinSize : orthographicSizeAfterZoom;
  }
  public void Pan()
  {
    if (Input.GetMouseButton(0))
    {
      if (Input.GetMouseButtonDown(0))
      {
        this.panStart = this.mousePositionWorldPoint;
      }
      Vector3 delta = this.panStart - this.mousePositionWorldPoint;
      this.mainCam.transform.position += delta;
    }
  }
  public void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
  {
    // Debug.Log($"{this} was notified about change in {e.PropertyName}.");

    if (e.PropertyName == "Map")
    {
      this.RenderMap();
      // StartCoroutine(this.RenderMapSlowly());
    }
    if (e.PropertyName == "Highlight")
    {
      this.RenderHighlights();
      // StartCoroutine(this.RenderMapSlowly());
    }
    if (e.PropertyName == "OpenCards")
    {
      this.RenderOpenCards();
      // StartCoroutine(this.RenderMapSlowly());
    }
    if (e.PropertyName == "SelectedCard")
    {
      this.RenderOpenCards();
    }
  }

  private void Start()
  {
    #region Camera movement setup
    this.mainCam = Camera.main;
    float nCols = (float)this.game.GameClient.gameSettings.NCols;
    float boardSize = (nCols + 1);
    float initialZoomSize = 1.3f * nCols / 2;
    this.mainCam.orthographicSize = initialZoomSize;
    this.mainCam.transform.position = new Vector3(nCols / 2, nCols / 2, -10);
    this.zoomMaxSize = 1 + boardSize / 2;
    #endregion

    #region Cards UI
    // CreateCardsUI();
    #endregion
  }
  private void CreateCardsUI()
  {
    Debug.Log("Creating Open Cards GO");
    this.openCardsGO = new GameObject("OpenCards");
    this.openCardsGO.transform.SetParent(GameObject.Find("UI").transform);
    RectTransform rect = openCardsGO.AddComponent<RectTransform>();
    rect.anchorMin = new Vector2(0, 0);
    rect.anchorMax = new Vector2(0, 0);
    openCardsGO.transform.localPosition = new Vector3(0, 0, 0);
    rect.anchoredPosition = new Vector3(Screen.width * 0.01f, Screen.width * 0.01f, 0);
    rect.sizeDelta = new Vector2(100, 100);
  }
  public void StartListening()
  {
    this.game.GameClient.PropertyChanged -= new PropertyChangedEventHandler(this.OnPropertyChanged);
    this.game.GameClient.PropertyChanged += new PropertyChangedEventHandler(this.OnPropertyChanged);
  }

  private void OnDestroy()
  {
    this.game.GameClient.PropertyChanged -= new PropertyChangedEventHandler(this.OnPropertyChanged);
  }

  private GameObject RenderTile(Tile tile)
  {
    // Debug.Log($"Render Tile {tile.Coordinate}");
    GameObject tileGO = new GameObject($"{tile.Coordinate}");
    SpriteRenderer sr = tileGO.AddComponent<SpriteRenderer>();
    sr.sprite = Array.Find(Resources.LoadAll<Sprite>("Sprites/Tiles"), s => s.name == "terrain_grass");
    TileView tileView = tileGO.AddComponent<TileView>();
    tileView.game = this.game;
    tileView.coordinate = tile.Coordinate;
    return tileGO;
  }

  private GameObject RenderUnit(Unit unit)
  {
    GameObject unitGO = new GameObject($"Player {unit.OwnerId}'s unit.");
    SpriteRenderer sr = unitGO.AddComponent<SpriteRenderer>();
    string untiSpritePath;
    switch (unit.OwnerId)
    {
      case 0: { untiSpritePath = "Sprites/bob"; break; }
      case 1: { untiSpritePath = "Sprites/maurice"; break; }
      case 2: { untiSpritePath = "Sprites/house"; break; }
      default: { untiSpritePath = ""; break; }
    }
    sr.sprite = Resources.Load<Sprite>(untiSpritePath);
    sr.sortingOrder = 10;
    return unitGO;
  }
  private GameObject RenderTerrain(Schnoz.Terrain terrain)
  {
    GameObject terrainGO = new GameObject($"{terrain.Type.ToString()}");
    SpriteRenderer sr = terrainGO.AddComponent<SpriteRenderer>();
    switch (terrain.Type)
    {
      case TerrainType.Water:
        {
          sr.sprite = Array.Find(Resources.LoadAll<Sprite>("Sprites/Tiles"), s => s.name == "terrain_water");
          break;
        }
      case TerrainType.Bush:
        {
          sr.sprite = Array.Find(Resources.LoadAll<Sprite>("Sprites/Tiles"), s => s.name == "terrain_bush");
          break;
        }
      case TerrainType.Stone:
        {
          sr.sprite = Array.Find(Resources.LoadAll<Sprite>("Sprites/Tiles"), s => s.name == "terrain_stone");
          break;
        }
      default:
        {
          break;
        }
    }

    sr.sortingOrder = 5;
    return terrainGO;
  }
  private GameObject RenderGround()
  {
    GameObject terrainGO = new GameObject("Grass");
    SpriteRenderer sr = terrainGO.AddComponent<SpriteRenderer>();
    sr.sprite = Array.Find(Resources.LoadAll<Sprite>("Sprites/Tiles"), s => s.name == "terrain_grass");
    sr.sortingOrder = 4;
    return terrainGO;
  }

  private void RenderHighlights()
  {
    foreach (Transform child in this.mapGO.transform)
    {
      TileView tileView = child.GetComponent<TileView>();
      if (this.game.TileDict.ContainsKey(tileView.coordinate))
      {
        Tile tile = this.game.TileDict[tileView.coordinate];
        SpriteRenderer sr = tileView.GetComponent<SpriteRenderer>();
        sr.color = this.game.HoveringTiles.Any(t => t.Coordinate == tile.Coordinate) ? new Color(0.9f, 0.9f, 0.9f) : Color.white;
      }
    }
  }

  private void RenderMap()
  {
    // Debug.Log("Rendering Map");
    if (this.mapGO != null)
    {
      Destroy(mapGO);
    }
    this.mapGO = new GameObject("Map");

    this.game.GameClient.Map.Tiles.ForEach((Tile tile) =>
    {
      GameObject tileGO = this.RenderTile(tile);
      tileGO.transform.SetParent(this.mapGO.transform);
      tileGO.transform.localPosition = new Vector2(tile.Coordinate.col, tile.Coordinate.row);
      if (tile.Unit != null)
      {
        GameObject unitGO = this.RenderUnit(tile.Unit);
        unitGO.transform.SetParent(tileGO.transform);
        unitGO.transform.localPosition = new Vector2(0, 0);
      }

      if (tile.Terrain.Type != TerrainType.Grass)
      {
        GameObject terrainGO = this.RenderTerrain(tile.Terrain);
        terrainGO.transform.SetParent(tileGO.transform);
        terrainGO.transform.localPosition = new Vector2(0, 0);
      }
    });
    // Debug.Log("Rendered Map successfully!");
  }
  private IEnumerator RenderMapSlowly(float interval = 0.1f)
  {
    // Debug.Log("Rendering Map");
    GameObject mapGO = GameObject.Find("Map");
    Destroy(mapGO);
    mapGO = new GameObject("Map");

    foreach (Tile tile in this.game.GameClient.Map.Tiles)
    {
      yield return new WaitForSeconds(interval);
      GameObject tileGO = this.RenderTile(tile);
      tileGO.transform.SetParent(mapGO.transform);
      tileGO.transform.localPosition = new Vector2(tile.Coordinate.col, tile.Coordinate.row);
      if (tile.Unit != null)
      {
        GameObject unitGO = this.RenderUnit(tile.Unit);
        unitGO.transform.SetParent(tileGO.transform);
        unitGO.transform.localPosition = new Vector2(0, 0);
      }
    }

    // Debug.Log("Rendered Map successfully!");
  }

  private GameObject RenderCard(Card card)
  {
    GameObject cardGO = new GameObject($"{card.Type}");
    SpriteRenderer sr = cardGO.AddComponent<SpriteRenderer>();
    if (this.game.SelectedCardId == card.Id)
    {
      sr.color = Color.green;
    }
    string fileName = card.Type.ToString();
    sr.sprite = Resources.Load<Sprite>($"Sprites/{fileName}");
    sr.sortingOrder = 20;
    return cardGO;
  }
  private void RenderOpenCards()
  {
    Debug.Log("Rendering Open Cards");
    if (this.openCardsGO != null)
    {
      Debug.Log("Destroying OpenCardsGo");
      GameObject.Destroy(this.openCardsGO);
    }
    this.CreateCardsUI();

    int index = 0;
    this.game.GameClient.OpenCards.ForEach(card =>
    {
      GameObject cardGO = this.RenderCard(card);
      cardGO.transform.SetParent(this.openCardsGO.transform);
      cardGO.transform.localPosition = new Vector2(0, index++);
      CardView cardView = cardGO.AddComponent<CardView>();
      cardView.game = this.game;
      cardView.cardId = card.Id;
    });
  }
}
