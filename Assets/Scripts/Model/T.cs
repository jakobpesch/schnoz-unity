using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Schnoz;
/// <summary>
/// This is a helper class containing predicates for selecting tiles.
///</summary>
public class T {
  // public static Func<GameObject, TileProperties> Properties = tile => tile;
  // public static Func<Tile, List<Tile>> AdjacentTiles = tile => Map.I.GetTiles(tile.AdjacentTilesIds);
  // public static Func<Tile, SpriteRenderer> SpriteRenderer = tile => tile.gameObject.GetComponent<SpriteRenderer>();
  public static Func<Tile, bool> InBounds = tile => tile != null;
  public static Func<Tile, bool> Placeable = tile => tile.Placeable;
  public static Func<Tile, bool> Empty = tile => tile.Unit == null;
  public static Func<Tile, bool> Visible = tile => tile.Visible;
  public static Func<Tile, bool> IsStone = t => {
    if (t.Terrain == null)
      return false;
    if (t.Terrain.Type == TerrainType.Stone)
      return true;
    return false;
  };
  public static Func<Tile, bool> UnitIsHostile = tile => {
    if (tile.Unit == null) return false;
    // return tp.UnitProperties.Owner != GameManager.I.TemporaryCurrentPlayer && tp.UnitProperties.Owner != GameManager.I.NeutralPlayer;
    return false;
  };
  public static Func<Tile, bool> UnitIsAllied = tile => {
    if (tile.Unit == null) return false;
    // return tp.UnitProperties.Owner == GameManager.I.TemporaryCurrentPlayer || tp.UnitProperties.Owner == GameManager.I.NeutralPlayer;
    return false;
  };
  public static Func<Tile, bool> UnitIsPlayers = tile => {

    if (tile.Unit == null) return false;
    // return tp.UnitProperties.Owner == GameManager.I.TemporaryCurrentPlayer;
    return false;
  };
  // public static Func<Tile, Map, bool> AdjacentUnitPlayer = (tile, map) => map.GetTiles(tile.AdjacentTilesIds).Any(T.UnitIsPlayers);
  // public static Func<Tile, Map, bool> AdjacentUnitAllied = (tile, map) => map.GetTiles(tile.AdjacentTilesIds).Any(T.UnitIsAllied);
  // public static Func<Tile, Map, bool> AdjacentUnitHostile = (tile, map) => map.GetTiles(tile.AdjacentTilesIds).Any(T.UnitIsHostile);
  // public static Func<Tile, Map, bool> AllAdjacentTilesEmpty = (tile, map) => map.GetTiles(tile.AdjacentTilesIds).All(Empty);

  /// <summary>
  /// Takes in a list of tiles and returns all the tiles that cannot be placed upon.
  /// </summary>
  public static Func<List<Tile>, IEnumerable<Tile>> PreventingPlacement = tiles => tiles.Where(H.Or(H.Not(InBounds), H.Or(H.Not(Placeable), H.Or(H.Not(Empty), H.Or(H.Not(Visible))))));

}
