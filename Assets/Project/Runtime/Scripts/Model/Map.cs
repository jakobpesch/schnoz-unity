﻿using System;
using System.Collections.Generic;
using Unity.Collections;
using System.Linq;
using Utils;
using TypeAliases;
using UnityEngine;

namespace Schnoz {
  [Serializable]
  public class Map {
    [SerializeField] private int nRows;
    [SerializeField] private int nCols;
    [SerializeField] private int partsGrass;
    [SerializeField] private int partsStone;
    [SerializeField] private int partsWater;
    [SerializeField] private int partsBush;
    [SerializeField] private List<Unit> units;
    public List<Unit> Units {
      get {
        List<Unit> u = this.Tiles.Where(tile => tile.Unit != null).Select(tile => tile.Unit).ToList();
        return u;
      }
    }
    public List<Terrain> Terrains {
      get {
        List<Terrain> t = this.Tiles.Where(tile => tile.Terrain.Type != TerrainType.Grass).Select(tile => tile.Terrain).ToList();
        return t;
      }
    }
    public Tile CenterTile {
      get {
        int middleRow = (int)Math.Ceiling((float)(nRows / 2));
        int middleCol = (int)Math.Ceiling((float)(nCols / 2));
        return CoordinateToTileDict[new Coordinate(middleRow, middleCol)];
      }
    }
    public List<Tile> Tiles { get; }
    public List<Tile> VisibleTiles {
      get => this.Tiles.Where(tile => tile.Visible).ToList();
    }
    public Dictionary<Coordinate, Tile> CoordinateToTileDict { get; }
    public List<List<Tile>> DiagonalsFromBottomLeftToTopRight { get => GetDiagonalsFromBottomLeftToTopRight(); }
    public List<List<Tile>> DiagonalsFromTopLeftToBottomRight { get => GetDiagonalsFromTopLeftToBottomRight(); }
    public Map(int nRows, int nCols, bool randomize = true, int partsGrass = 0, int partsStone = 0, int partsWater = 0, int partsBush = 0) {
      Debug.Log($"Initialize map with {nRows} and {nCols}");
      this.nRows = nRows;
      this.nCols = nCols;
      this.partsGrass = partsGrass;
      this.partsStone = partsStone;
      this.partsWater = partsWater;
      this.partsBush = partsBush;
      this.Tiles = new List<Tile>();
      this.CoordinateToTileDict = new Dictionary<Coordinate, Tile>();
      for (int row = 0; row < nRows; row++) {
        for (int col = 0; col < nCols; col++) {
          Tile tile = new Tile(row, col);
          this.Tiles.Add(tile);
          this.CoordinateToTileDict.Add(tile.Coordinate, tile);
        }
      }

      if (randomize) {
        this.RandomizeTerrain();
        foreach (Tile t in this.GetTilesWithinRadius(CenterTile, Constants.SaveArea)) {
          t.SetTerrain(TerrainType.Grass);
        }
      }


    }

    private void RandomizeTerrain() {
      float total = this.partsGrass + this.partsWater + this.partsBush + this.partsStone;
      float chanceGrass = this.partsGrass / total;
      float chanceWater = this.partsWater / total;
      float chanceBush = this.partsBush / total;
      float chanceStone = this.partsStone / total;
      List<TerrainType> probabilityArray = new List<TerrainType>();
      TerrainType terrainGrass = TerrainType.Grass;
      TerrainType terrainBush = TerrainType.Bush;
      TerrainType terrainWater = TerrainType.Water;
      TerrainType terrainStone = TerrainType.Stone;
      probabilityArray.AddRange(Enumerable.Repeat(terrainGrass, (int)partsGrass).ToList());
      probabilityArray.AddRange(Enumerable.Repeat(terrainBush, (int)partsBush).ToList());
      probabilityArray.AddRange(Enumerable.Repeat(terrainWater, (int)partsWater).ToList());
      probabilityArray.AddRange(Enumerable.Repeat(terrainStone, (int)partsStone).ToList());

      foreach (Tile tile in this.Tiles) {

        // int rerollSucceeded = 0;
        // int rerollFailed = 0;

        int randomInt = UnityEngine.Random.Range(0, probabilityArray.Count);
        TerrainType terrainType = probabilityArray[randomInt];

        List<TerrainType> adjacentTerrainTypes = new List<TerrainType>();
        List<Tile> adjacentTiles = this.GetAdjacentTiles(tile);
        foreach (Tile at in adjacentTiles.Where(t => t != null && t.Terrain.Type != TerrainType.Grass)) {
          adjacentTerrainTypes.Add(at.Terrain.Type);
        }

        // Reroll for each adjacent special tile
        for (int i = 0; i < adjacentTerrainTypes.Count(); i++) {
          if (adjacentTerrainTypes.Contains(terrainType)) {
            break;
          }
          randomInt = UnityEngine.Random.Range(0, probabilityArray.Count);
          terrainType = probabilityArray[randomInt];
        }

        tile.SetTerrain(terrainType);
      }
    }

    public string Serialize() {
      NetMap netMap = new NetMap();
      netMap.r = this.nRows;
      netMap.c = this.nCols;
      netMap.u = this.Units.Select(unit => {
        NetUnit netUnit = new NetUnit();
        netUnit.i = (int)unit.OwnerId;
        netUnit.r = unit.Coordinate.row;
        netUnit.c = unit.Coordinate.col;
        return netUnit;
      }).ToList();
      netMap.t = this.Terrains.Select(terrain => {
        NetTerrain netTerrain = new NetTerrain();
        netTerrain.t = (int)terrain.Type;
        netTerrain.r = terrain.Coordinate.row;
        netTerrain.c = terrain.Coordinate.col;
        return netTerrain;
      }).ToList();
      return JsonUtility.ToJson(netMap);
    }
    public List<Tile> GetTiles(List<Guid> ids) {
      return this.Tiles.FindAll(t => ids.Any(id => t.Id == id)).ToList();
    }
    public void UpdateFog(Tile baseTile) {
      foreach (Tile tile in this.GetTilesWithinRadius(baseTile)) {
        if (!tile.Visible) {
          tile.SetVisibility(true);
        }
      }
    }
    public List<Tile> GetTilesWithinRadius(Tile baseTile, float radius = 3f) {
      return this.Tiles.Where(tile => {
        float distance = Math.Abs(System.Numerics.Vector2.Distance(new System.Numerics.Vector2(baseTile.Row, baseTile.Col), new System.Numerics.Vector2(tile.Row, tile.Col)));
        return distance <= radius + 0.2f;
      }).ToList();
    }
    public List<Unit> GetAdjacentEnemies(Tile centerTile, PlayerIds ownerId) {
      return this.GetAdjacentTiles(centerTile).Select(adjacentTile => {
        var unit = this.CoordinateToTileDict[adjacentTile.Coordinate].Unit;
        if (unit == null || unit.OwnerId == ownerId || unit.OwnerId == PlayerIds.NeutralPlayer) {
          return null;
        } else {
          return unit;
        }
      }).Where(unit => unit != null).ToList();
    }
    public List<Unit> GetAdjacentAllies(Tile centerTile, PlayerIds ownerId) {
      return this.GetAdjacentTiles(centerTile).Select(adjacentTile => {
        var unit = this.CoordinateToTileDict[adjacentTile.Coordinate].Unit;
        if (unit == null || unit.OwnerId != ownerId) {
          return null;
        } else {
          return unit;
        }
      }).Where(unit => unit != null).ToList();
    }
    public List<Tile> GetAdjacentTiles(Tile middleTile) {
      List<Coordinate> directions = new List<Coordinate>()
      {
        new Coordinate(0, 1),
        new Coordinate(0, -1),
        new Coordinate(1, 0),
        new Coordinate(-1, 0)
      };
      List<Tile> adjacentTiles = directions.Select(dir => {
        Coordinate adjacentCoordinate = middleTile.Coordinate + dir;
        if (!this.CoordinateToTileDict.ContainsKey(adjacentCoordinate)) {
          return null;
        }
        return this.CoordinateToTileDict[adjacentCoordinate];
      }).Where(tile => tile != null).ToList();
      return adjacentTiles.ToList();
    }
    public IEnumerable<Tile> GetTilesWithUnitsFrom(List<Tile> tiles) {
      return tiles.FindAll(tile =>
          tile != null &&
          tile.Unit != null);
    }
    private List<List<Tile>> GetDiagonalsFromBottomLeftToTopRight() {
      IEnumerable<Tile> bottomAndLeftBorderTiles = this.Tiles.Where(tile =>
        tile.Row == 0 || tile.Col == 0);

      List<List<Tile>> diagonals = new List<List<Tile>>();

      foreach (Tile borderTile in bottomAndLeftBorderTiles) {
        List<Tile> diagonal = new List<Tile>() { borderTile };
        int i = 1;
        while (true) {
          // Checks the next tile to the top right
          Coordinate nextCoordInDiagonal = borderTile.Coordinate + new Coordinate(i, i);
          Tile nextTileInDiagonal = this.Tiles.Find(tile => tile.Coordinate == nextCoordInDiagonal);
          if (nextTileInDiagonal == null) {
            break;
          }
          diagonal.Add(nextTileInDiagonal);
          i++;
        }
        diagonals.Add(diagonal);
      }
      return diagonals;
    }
    private List<List<Tile>> GetDiagonalsFromTopLeftToBottomRight() {
      IEnumerable<Tile> topAndLeftBorderTiles = this.Tiles.Where(tile =>
        tile.Row == this.nRows || tile.Col == 0);

      List<List<Tile>> diagonals = new List<List<Tile>>();

      foreach (Tile borderTile in topAndLeftBorderTiles) {
        List<Tile> diagonal = new List<Tile>() { borderTile };
        int i = 1;
        while (true) {
          // Checks the next tile to the top right
          Coordinate nextCoordInDiagonal = borderTile.Coordinate + new Coordinate(-i, i);
          Tile nextTileInDiagonal = this.Tiles.Find(tile => tile.Coordinate == nextCoordInDiagonal);
          if (nextTileInDiagonal == null) {
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
