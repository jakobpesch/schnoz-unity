using System;
using System.Collections.Generic;
using Unity.Collections;
using System.Linq;
using Utils;
using TypeAliases;
using UnityEngine;

namespace Schnoz
{
  [Serializable]
  public class Map : Observable
  {
    [SerializeField] private int nRows;
    [SerializeField] private int nCols;
    [SerializeField] private List<Unit> units;
    public List<Unit> Units
    {
      get
      {
        List<Unit> u = Tiles.Where(tile => tile.Unit != null).Select(tile => tile.Unit).ToList();
        return u;
      }
    }
    [SerializeField]
    private Tile centerTile
    {
      get
      {
        int middleRow = (int)Math.Ceiling((float)(nRows / 2));
        int middleCol = (int)Math.Ceiling((float)(nCols / 2));
        return CoordinateToTileDict[new Coordinate(middleRow, middleCol)];
      }
    }
    public List<Tile> Tiles { get; }
    public Dictionary<Coordinate, Tile> CoordinateToTileDict { get; }
    public List<List<Tile>> DiagonalsFromBottomLeftToTopRight { get => GetDiagonalsFromBottomLeftToTopRight(); }
    public List<List<Tile>> DiagonalsFromTopLeftToBottomRight { get => GetDiagonalsFromTopLeftToBottomRight(); }
    public Map(int nRows, int nCols)
    {
      this.nRows = nRows;
      this.nCols = nCols;
      this.Tiles = new List<Tile>();
      this.CoordinateToTileDict = new Dictionary<Coordinate, Tile>();
      for (int row = 0; row < nRows; row++)
      {
        for (int col = 0; col < nCols; col++)
        {
          Tile tile = new Tile(row, col);
          this.Tiles.Add(tile);

          CoordinateToTileDict.Add(tile.Coordinate, tile);
        }
      }
    }
    public Map(NetMap netMap) : this(netMap.r, netMap.c)
    {
      netMap.u.ForEach(netUnit =>
      {
        Coordinate coordinate = new Coordinate(netUnit.r, netUnit.c);
        this.CoordinateToTileDict[coordinate].SetUnit(new Unit(netUnit));
      });
    }

    public string Serialize()
    {
      NetMap netMap = new NetMap();
      netMap.r = this.nRows;
      netMap.c = this.nCols;
      netMap.u = this.Units.Select(unit =>
      {
        NetUnit netUnit = new NetUnit();
        netUnit.i = unit.OwnerId;
        netUnit.r = unit.Coordinate.row;
        netUnit.c = unit.Coordinate.col;
        return netUnit;
      }).ToList();
      return JsonUtility.ToJson(netMap);
    }
    public List<Tile> GetTiles(List<Guid> ids)
    {
      return this.Tiles.FindAll(t => ids.Any(id => t.Id == id)).ToList();
    }
    public void UpdateFog(Tile baseTile)
    {
      foreach (Tile tile in this.GetTilesWithinRadius(baseTile))
      {
        if (!tile.Visible)
        {
          tile.SetVisibility(true);
        }
      }
    }
    public List<Tile> GetTilesWithinRadius(Tile baseTile, float radius = 3f)
    {
      return this.Tiles.Where(tile =>
      {
        float distance = Math.Abs(System.Numerics.Vector2.Distance(new System.Numerics.Vector2(baseTile.Row, baseTile.Col), new System.Numerics.Vector2(tile.Row, tile.Col)));
        return distance <= radius + 0.2f;
      }).ToList();
    }
    public IEnumerable<Unit> GetAdjacentEnemies(Tile centerTile, List<Tile> adjacentTiles)
    {
      return adjacentTiles.FindAll(
          at => at.Unit.OwnerId
          != centerTile.Unit.OwnerId).Select(atWithEnemies => atWithEnemies.Unit);
    }
    public IEnumerable<Unit> GetAdjacentAllies(Tile centerTile, List<Tile> adjacentTiles)
    {
      return GetAdjacentTiles(centerTile).Select(adjacentTile =>
        this.CoordinateToTileDict
  .ContainsKey(adjacentTile.Coordinate)
        ? this.CoordinateToTileDict
  [adjacentTile.Coordinate].Unit
        : null
      );
    }
    public IEnumerable<Tile> GetTilesWithUnitsFrom(List<Tile> tiles)
    {
      return tiles.FindAll(tile =>
          tile != null &&
          tile.Unit != null);
    }
    public List<Tile> GetAdjacentTiles(Tile middleTile)
    {
      List<Coordinate> directions = new List<Coordinate>()
      {
        new Coordinate(0, 1),
        new Coordinate(0, -1),
        new Coordinate(1, 0),
        new Coordinate(-1, 0)
      };
      List<Tile> adjacentTiles = directions.Select(dir =>
      {
        Coordinate adjacentCoordinate = middleTile.Coordinate + dir;
        if (!this.CoordinateToTileDict
  .ContainsKey(adjacentCoordinate))
        {
          return null;
        }
        return this.CoordinateToTileDict
  [adjacentCoordinate];
      }).ToList();
      return adjacentTiles.ToList();
    }
    private List<List<Tile>> GetDiagonalsFromBottomLeftToTopRight()
    {
      IEnumerable<Tile> bottomAndLeftBorderTiles = this.Tiles.Where(tile =>
        tile.Row == 0 || tile.Col == 0);

      List<List<Tile>> diagonals = new List<List<Tile>>();

      foreach (Tile borderTile in bottomAndLeftBorderTiles)
      {
        List<Tile> diagonal = new List<Tile>() { borderTile };
        int i = 1;
        while (true)
        {
          // Checks the next tile to the top right
          Coordinate nextCoordInDiagonal = borderTile.Coordinate + new Coordinate(i, i);
          Tile nextTileInDiagonal = this.Tiles.Find(tile => tile.Coordinate == nextCoordInDiagonal);
          if (nextTileInDiagonal == null)
          {
            break;
          }
          diagonal.Add(nextTileInDiagonal);
          i++;
        }
        diagonals.Add(diagonal);
      }
      return diagonals;
    }
    private List<List<Tile>> GetDiagonalsFromTopLeftToBottomRight()
    {
      IEnumerable<Tile> topAndLeftBorderTiles = this.Tiles.Where(tile =>
        tile.Row == this.nRows || tile.Col == 0);

      List<List<Tile>> diagonals = new List<List<Tile>>();

      foreach (Tile borderTile in topAndLeftBorderTiles)
      {
        List<Tile> diagonal = new List<Tile>() { borderTile };
        int i = 1;
        while (true)
        {
          // Checks the next tile to the top right
          Coordinate nextCoordInDiagonal = borderTile.Coordinate + new Coordinate(-i, i);
          Tile nextTileInDiagonal = this.Tiles.Find(tile => tile.Coordinate == nextCoordInDiagonal);
          if (nextTileInDiagonal == null)
          {
            break;
          }
          diagonal.Add(nextTileInDiagonal);
          i++;
        }
        diagonals.Add(diagonal);
      }
      return diagonals;
    }

    // private void UpdateArea(Tile tile, TileArea area)
    // {
    //   // Check current tile

    //   bool isPartOfAnotherArea = tile.Area != null;
    //   bool isVisible = tile.Visible;
    //   bool hasTerrain = tile.Terrain != null;

    //   if (!isVisible || !hasTerrain || isPartOfAnotherArea)
    //   {
    //     return;
    //   }

    //   if (!this.Areas.Contains(area))
    //   {
    //     this.Areas.Add(area);
    //   }

    //   // Add Area and Tile
    //   tile.SetArea(area);
    //   area.Add(tile);

    //   // Check neighbors
    //   foreach (Tile at in tile.AdjacentTiles)
    //   {
    //     if (at.Terrain == null)
    //     {
    //       continue;
    //     }

    //     bool sameTerrainType = tile.Terrain.Type == at.Terrain.Type; // todo: custom class equality
    //     if (sameTerrainType)
    //     {
    //       UpdateArea(at, area);
    //     }
    //   }
    // }
    // private void UpdateTerrainTiles()
    // {
    //   waterTiles = GetTerrainTiles(terrainWater);
    //   stoneTiles = GetTerrainTiles(terrainStone);
    //   bushTiles = GetTerrainTiles(terrainBush);
    // }
    // private void UpdateTerrainTile(string terrainType)
    // {
    //   SchnozTerrain terrain = null;
    //   List<Tile> terrainTiles;
    //   if (terrainType == "water")
    //     waterTiles = GetTerrainTiles(this.terrainWater);
    //   if (terrainType == "stone")
    //     stoneTiles = GetTerrainTiles(this.terrainStone);
    //   else //bush
    //     bushTiles = GetTerrainTiles(this.terrainBush);
    // }
    // private List<GameObject> GetTerrainTiles(GameObject terrain)
    // {
    //   List<GameObject> terrainTiles = new List<GameObject>();
    //   foreach (GameObject tile in Tiles.Where(t =>
    //   {
    //     TileProperties tp = T.Properties(t);
    //     if (tp.Terrain == null)
    //       return false;
    //     return tp.Terrain.GetComponent<TerrainProperties>().id == terrain.GetComponent<TerrainProperties>().id;
    //   }))
    //     terrainTiles.Add(tile);
    //   return terrainTiles;
    // }
  }
}
