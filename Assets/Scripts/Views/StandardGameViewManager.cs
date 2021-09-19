using System.Linq;
using System.Collections;
using System.ComponentModel;
using UnityEngine;
using Utils;
using Schnoz;

public class StandardGameViewManager : MonoBehaviour
{
  public StandardGame game;
  private InputManager inputManager;
  private Camera mainCam;
  private Vector2 resolution;

  private GameObject mapGO;
  private void Awake()
  {
    this.resolution = new Vector2(Screen.width, Screen.height);
  }
  public void Update()
  {
    if (resolution.x != Screen.width || resolution.y != Screen.height)
    {
      this.RenderCurrentCards();

      resolution.x = Screen.width;
      resolution.y = Screen.height;
    }
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
    if (e.PropertyName == "CurrentCards")
    {
      this.RenderCurrentCards();
      // StartCoroutine(this.RenderMapSlowly());
    }
    if (e.PropertyName == "SelectedCard")
    {
      this.RenderCurrentCards();
    }
  }

  private void Start()
  {
    this.mainCam = Camera.main;
    float nCols = (float)this.game.Schnoz.gameSettings.NCols;
    float boardSize = (nCols + 1);
    float zoomMaxSize = 1 + boardSize / 2;
    // float min = 5; float max = (float)nCols;
    // float initialZoomSize = 1.3f * (Mathf.Abs(min) + Mathf.Abs(max)) / 2;
    float initialZoomSize = 1.3f * nCols / 2;
    Camera.main.orthographicSize = initialZoomSize;
    Camera.main.transform.position = new Vector3(nCols / 2, nCols / 2, -10);
  }

  public void StartListening()
  {
    this.game.Schnoz.PropertyChanged -= new PropertyChangedEventHandler(this.OnPropertyChanged);
    this.game.Schnoz.PropertyChanged += new PropertyChangedEventHandler(this.OnPropertyChanged);
  }

  private void OnDestroy()
  {
    this.game.Schnoz.PropertyChanged -= new PropertyChangedEventHandler(this.OnPropertyChanged);
  }

  private GameObject RenderTile(Tile tile)
  {
    // Debug.Log($"Render Tile {tile.Coordinate}");
    GameObject tileGO = new GameObject($"{tile.Coordinate}");
    SpriteRenderer sr = tileGO.AddComponent<SpriteRenderer>();
    sr.sprite = Resources.Load<Sprite>("Sprites/tile_grass");
    TileView tileView = tileGO.AddComponent<TileView>();
    tileView.game = this.game;
    tileView.tileId = tile.Id;
    return tileGO;
  }

  private GameObject RenderUnit(Unit unit)
  {
    GameObject unitGO = new GameObject($"Unit:{unit.UnitName}:{unit.Vision}");
    SpriteRenderer sr = unitGO.AddComponent<SpriteRenderer>();
    sr.sprite = Resources.Load<Sprite>("Sprites/bob");
    sr.sortingOrder = 10;
    return unitGO;
  }

  private void RenderHighlights()
  {
    foreach (Transform child in this.mapGO.transform)
    {
      TileView tileView = child.GetComponent<TileView>();
      if (this.game.TileDict.ContainsKey(tileView.tileId))
      {
        Tile tile = this.game.TileDict[tileView.tileId];
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

    this.game.Schnoz.Map.Tiles.ForEach((Tile tile) =>
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
    });
    // Debug.Log("Rendered Map successfully!");
  }
  private IEnumerator RenderMapSlowly(float interval = 0.1f)
  {
    // Debug.Log("Rendering Map");
    GameObject mapGO = GameObject.Find("Map");
    Destroy(mapGO);
    mapGO = new GameObject("Map");

    foreach (Tile tile in this.game.Schnoz.Map.Tiles)
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
    Debug.Log(this.game.SelectedCardId == card.Id);
    if (this.game.SelectedCardId == card.Id)
    {
      sr.color = Color.green;
    }
    string fileName = card.Type.ToString();
    sr.sprite = Resources.Load<Sprite>($"Sprites/{fileName}");
    sr.sortingOrder = 20;
    return cardGO;
  }
  private void RenderCurrentCards()
  {
    // Debug.Log("Rendering Current Cards");
    GameObject currentCardsGO = GameObject.Find("CurrentCards");
    Destroy(currentCardsGO);
    currentCardsGO = new GameObject("CurrentCards");
    currentCardsGO.transform.SetParent(GameObject.Find("Canvas").transform);
    RectTransform rect = currentCardsGO.AddComponent<RectTransform>();
    rect.anchorMin = new Vector2(0, 0);
    rect.anchorMax = new Vector2(0, 0);
    Debug.Log(rect.position);
    currentCardsGO.transform.localPosition = new Vector3(0, 0, 0);
    rect.anchoredPosition = new Vector3(Screen.width * 0.01f, Screen.width * 0.01f, 0);
    rect.sizeDelta = new Vector2(100, 100);
    Debug.Log(rect.anchoredPosition);
    int index = 0;
    this.game.Schnoz.CurrentCards.ForEach(card =>
    {
      GameObject cardGO = this.RenderCard(card);
      cardGO.transform.SetParent(currentCardsGO.transform);
      cardGO.transform.localPosition = new Vector2(0, index++);
      CardView cardView = cardGO.AddComponent<CardView>();
      cardView.game = this.game;
      cardView.cardId = card.Id;
    });
  }
}
