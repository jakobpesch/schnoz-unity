using System.ComponentModel;
using UnityEngine;
using Utils;
using Schnoz;

public class StandardGameViewManager : MonoBehaviour
{
  public StandardGame game;
  private InputManager inputManager;
  public Sprite sprite;
  private Camera mainCam;
  private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
  {
    Debug.Log($"{this} was notified about change in {e.PropertyName}.");

    if (e.PropertyName == "Map")
    {
      this.RenderMap();
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
    // Debug.Log($"Render Tile {tile.Pos}");
    GameObject tileGO = new GameObject($"{tile.Pos}");
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
    sr.sprite = Resources.Load<Sprite>("Sprites/capital");
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
      tileGO.transform.localPosition = new Vector2(tile.Pos.col, tile.Pos.row);
      if (tile.Unit != null)
      {
        GameObject unitGO = this.RenderUnit(tile.Unit);
        unitGO.transform.SetParent(tileGO.transform);
        unitGO.transform.localPosition = new Vector2(0, 0);
      }
    });
    Debug.Log("Rendered Map successfully!");
  }
}
