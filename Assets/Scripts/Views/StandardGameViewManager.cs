
using System.Collections.Generic;
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


  private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
  {
    Debug.Log($"{this} was notified about change in {e.PropertyName}.");

    if (e.PropertyName == "Map")
    {
      this.RenderMap();
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
    tileView.tile = tile;
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

  private void RenderMap()
  {
    Debug.Log("Rendering Map");
    GameObject mapGO = GameObject.Find("Map");
    Destroy(mapGO);
    mapGO = new GameObject("Map");

    this.game.Schnoz.Map.Tiles.ForEach((Tile tile) =>
    {
      GameObject tileGO = this.RenderTile(tile);
      tileGO.transform.SetParent(mapGO.transform);
      Debug.Log(tile.Coordinate);
      tileGO.transform.localPosition = new Vector2(tile.Coordinate.col, tile.Coordinate.row);
      if (tile.Unit != null)
      {
        GameObject unitGO = this.RenderUnit(tile.Unit);
        unitGO.transform.SetParent(tileGO.transform);
        unitGO.transform.localPosition = new Vector2(0, 0);
      }
    });
    Debug.Log("Rendered Map successfully!");
  }
  private IEnumerator RenderMapSlowly(float interval = 0.1f)
  {
    Debug.Log("Rendering Map");
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

    Debug.Log("Rendered Map successfully!");
  }

  private GameObject RenderCard(Card card)
  {
    GameObject cardGO = new GameObject($"{card.Type}");
    SpriteRenderer sr = cardGO.AddComponent<SpriteRenderer>();
    if (this.game.Schnoz.SelectedCard?.Id == card.Id)
    {
      sr.color = Color.green;
    }
    string fileName = card.Type.ToString();
    Debug.Log(fileName);
    sr.sprite = Resources.Load<Sprite>($"Sprites/{fileName}");
    sr.sortingOrder = 20;
    return cardGO;
  }
  private void RenderCurrentCards()
  {
    Debug.Log("Rendering Current Cards");
    GameObject currentCardsGO = GameObject.Find("CurrentCards");
    Destroy(currentCardsGO);
    currentCardsGO = new GameObject("CurrentCards");
    int index = 0;
    this.game.Schnoz.CurrentCards.ForEach(card =>
    {
      GameObject cardGO = this.RenderCard(card);
      cardGO.transform.SetParent(currentCardsGO.transform);
      cardGO.transform.localPosition = new Vector2(-3, index++);
      CardView cardView = cardGO.AddComponent<CardView>();
      cardView.game = this.game;
      cardView.card = card;
    });
  }
}
