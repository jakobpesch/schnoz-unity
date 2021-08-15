﻿using System;
using System.Numerics;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Schnoz;

namespace Schnoz
{
  public class Map
  {

    public List<Card> CurrentCards;
    public Deck Deck;
    private float tileGenerationInterval = 0f, terrainGenerationInterval = 0.4f;
    public List<Player> Players;
    public bool GameStarted;
    public bool LocalGame;
    public Tile CenterTile;
    private bool randomizeMap = false, increaseChanceOfAccumulation;
    private int nRows = 0, nCols = 0;
    private float tileSize = 1;
    public float HalfTileSize { get => tileSize / 2; }
    public Terrain terrainBush, terrainStone, terrainWater;
    private List<Tile> tiles, visibleTiles, bushTiles, waterTiles, stoneTiles;
    private List<TileArea> areas;
    private List<List<Tile>> diagonalsFromBottomLeftToTopRight, diagonalsFromTopLeftToBottomRight;
    private uint partsGrass, partsWater, partsBush, partsStone;
    private float chanceGrass, chanceWater, chanceBush, chanceStone;
    public int NRows { get => nRows; set => nRows = value; }
    public int NCols { get => nCols; set => nCols = value; }
    public float TileSize { get => tileSize; set => tileSize = value; }
    public List<Tile> Tiles
    {
      get => tiles;
    }
    public List<Tile> BushTiles { get => bushTiles; set => bushTiles = value; }
    public List<Tile> WaterTiles { get => waterTiles; set => waterTiles = value; }
    public List<Tile> StoneTiles { get => stoneTiles; set => stoneTiles = value; }
    public List<TileArea> Areas { get => areas; }
    public List<List<Tile>> DiagonalsFromBottomLeftToTopRight { get => DiagonalsFromBottomLeftToTopRight; }
    public List<List<Tile>> DiagonalsFromTopLeftToBottomRight { get => DiagonalsFromTopLeftToBottomRight; }
    public List<Tile> VisibleTiles { get => visibleTiles; }

    public Tile GetTile(Guid id)
    {
      return this.tiles.Find(t => t.Id == id);
    }
    public List<Tile> GetTiles(List<Guid> ids)
    {
      return this.tiles.FindAll(t => ids.Any(id => t.Id == id)).ToList();
    }
    public void UpdateFog(Tile baseTile)
    {
      foreach (Tile tile in this.GetTilesWithinRadius(baseTile, baseTile.Unit.Vision))
      {
        if (!tile.Visible)
        {
          tile.SetVisibility(true);
        }
      }
    }
    public List<Tile> GetTilesWithinRadius(Tile baseTile, float radius)
    {
      return this.tiles.Where(tile =>
      {
        float distance = Math.Abs(Vector2.Distance(new Vector2(baseTile.Row, baseTile.Col), new Vector2(tile.Row, tile.Col)));
        return distance <= radius + 0.2f;
      }).ToList();
    }
    public void Scan()
    {
      this.areas = new List<TileArea>();

      foreach (Tile tile in tiles)
      {

        // Reset Areas on Tiles
        tile.SetArea(null);

        // Update Fog
        if (tile.Unit != null)
        {
          this.UpdateFog(tile);
        }
      }

      // Update Adjacent Enemies and Allies
      foreach (Tile tile in tiles.Where(T.Visible))
      {
        // Get Components
        tile.SetAdjacentEnemies(new List<Unit>());
        tile.SetAdjacentAllies(new List<Unit>());
        if (tile.Unit != null)
        {
          tile.SetAdjacentEnemies(GetAdjacentEnemies(tile, GetTilesWithUnitsFrom(this.GetTiles(tile.AdjacentTilesIds)).ToList()).ToList());
          tile.SetAdjacentAllies(GetAdjacentAllies(tile, GetTilesWithUnitsFrom(this.GetTiles(tile.AdjacentTilesIds)).ToList()).ToList());
        }
        // Update Area
        UpdateArea(tile, new TileArea());
      }
      // UIManager.I.UpdateMaxZoomSize();
      // UpdateTerrainTiles();
      // UpdateTerrainTile("stone");
    }
    public void Generate()
    {
      tiles = new List<Tile>();
      for (int row = 0; row < nRows; row++)
      {
        for (int col = 0; col < nCols; col++)
        {
          Tile tile = new Tile(row, col);
          tiles.Add(tile);
        }
      }

      if (randomizeMap)
      {
        foreach (Tile tile in tiles)
        {
          this.Randomize(tile);
        }
      }

      diagonalsFromBottomLeftToTopRight = GetDiagonalsFromBottomLeftToTopRight();
      diagonalsFromTopLeftToBottomRight = GetDiagonalsFromTopLeftToBottomRight();

      // UpdateTerrainTiles();
    }

    public void ClearTiles()
    {
      this.tiles.Select(tile =>
      {
        if (tile == CenterTile)
        {
          return tile;
        }
        return new Tile(tile.Row, tile.Col, tile.Terrain);
      }
      );
    }

    private void Randomize(Tile tile)
    {
      Random rnd = new Random();
      List<Terrain> terrainProbabilityList = new List<Terrain>();

      float total = partsGrass + partsWater + partsBush + partsStone;
      chanceGrass = partsGrass / total;
      chanceWater = partsWater / total;
      chanceBush = partsBush / total;
      chanceStone = partsStone / total;

      terrainProbabilityList.AddRange(Enumerable.Repeat((Terrain)null, (int)partsGrass).ToList());
      terrainProbabilityList.AddRange(Enumerable.Repeat(terrainBush, (int)partsBush).ToList());
      terrainProbabilityList.AddRange(Enumerable.Repeat(terrainWater, (int)partsWater).ToList());
      terrainProbabilityList.AddRange(Enumerable.Repeat(terrainStone, (int)partsStone).ToList());

      // int rerollSucceeded = 0;
      // int rerollFailed = 0;

      int randomInt = rnd.Next(0, terrainProbabilityList.Count);
      Terrain terrain = terrainProbabilityList[randomInt];

      if (terrain != null)
      {
        // Reroll for each adjacent special tile
        if (increaseChanceOfAccumulation)
        {
          List<Guid> adjacentTerrainIds = new List<Guid>();
          foreach (Tile at in tile.AdjacentTiles.Where(at => at.Terrain != null))
          {
            adjacentTerrainIds.Add(at.Terrain.Id);
          }

          for (int i = 0; i < adjacentTerrainIds.Count(); i++)
          {
            if (adjacentTerrainIds.Contains(terrain.Id))
            {
              break;
            }

            randomInt = rnd.Next(0, terrainProbabilityList.Count);
            terrain = terrainProbabilityList[randomInt];
          }

          // if (adjacentTerrainIds.Contains(terrain?.Id))
          //   rerollSucceeded++;
          // else
          //   rerollFailed++;
        }
      }
      tile.SetTerrain(terrain); ;


      this.CenterTile.SetTerrain(null);

      //// Debug.Log("Rerolls successful: " + rerollSucceeded + "\nReroll failed: " + rerollFailed);
      foreach (Tile t in GetTilesWithinRadius(this.CenterTile, 2))//GameManager.I.safeArea))
        t.SetTerrain(null);

      // Scan();
    }

    public IEnumerable<Unit> GetAdjacentEnemies(Tile centerTile, List<Tile> adjacentTiles)
    {
      return adjacentTiles.FindAll(
          at => at.Unit.Owner
          != centerTile.Unit.Owner).Select(atWithEnemies => atWithEnemies.Unit);
    }
    public IEnumerable<Unit> GetAdjacentAllies(Tile centerTile, List<Tile> adjacentTiles)
    {
      return adjacentTiles.FindAll(
          at => at.Unit.Owner
          == centerTile.Unit.Owner).Select(atWithAllies => atWithAllies.Unit); ;
    }
    public IEnumerable<Tile> GetTilesWithUnitsFrom(List<Tile> tiles)
    {
      return tiles.FindAll(tile =>
          tile != null &&
          tile.Unit != null);
    }
    public IEnumerable<Tile> GetAdjacentTiles(Tile middleTile)
    {
      List<(int x, int y)> directions = new List<(int x, int y)>() { (0, 1), (0, -1), (1, 0), (-1, 0) };
      return this.tiles.Where(tile =>
         directions.Any(dir =>
          tile.Row - dir.x == middleTile.Row && tile.Col - dir.y == middleTile.Col
        )
      );
    }
    private void UpdateArea(Tile tile, TileArea area)
    {
      // Check current tile

      bool isPartOfAnotherArea = tile.Area != null;
      bool isVisible = tile.Visible;
      bool hasTerrain = tile.Terrain != null;

      if (!isVisible || !hasTerrain || isPartOfAnotherArea)
      {
        return;
      }

      if (!this.Areas.Contains(area))
      {
        this.Areas.Add(area);
      }

      // Add Area and Tile
      tile.SetArea(area);
      area.Add(tile);

      // Check neighbors
      foreach (Tile at in tile.AdjacentTiles)
      {
        if (at.Terrain == null)
        {
          continue;
        }

        bool sameTerrainType = tile.Terrain.Type == at.Terrain.Type; // todo: custom class equality
        if (sameTerrainType)
        {
          UpdateArea(at, area);
        }
      }
    }
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
    private List<List<Tile>> GetDiagonalsFromBottomLeftToTopRight()
    {
      IEnumerable<Tile> bottomAndLeftBorderTiles = this.tiles.Where(tile =>
        tile.Row == 0 || tile.Col == 0);

      List<List<Tile>> diagonals = new List<List<Tile>>();

      foreach (Tile borderTile in bottomAndLeftBorderTiles)
      {
        List<Tile> diagonal = new List<Tile>() { borderTile };

        (int borderTileRow, int borderTileCol) = borderTile.Pos;
        int i = 1;
        while (true)
        {
          // Checks the next tile to the top right
          (int row, int col) nextPosInDiagonal = ((borderTileRow + i), (borderTileCol + i));
          Tile nextTileInDiagonal = this.tiles.Find(tile => tile.Pos == nextPosInDiagonal);
          if (nextTileInDiagonal == null)
            break;

          diagonal.Add(nextTileInDiagonal);
          i++;
        }
        diagonals.Add(diagonal);
      }
      return diagonals;
    }
    private List<List<Tile>> GetDiagonalsFromTopLeftToBottomRight()
    {
      IEnumerable<Tile> topAndLeftBorderTiles = this.tiles.Where(tile =>
        tile.Row == this.nRows || tile.Col == 0);

      List<List<Tile>> diagonals = new List<List<Tile>>();

      foreach (Tile borderTile in topAndLeftBorderTiles)
      {
        List<Tile> diagonal = new List<Tile>() { borderTile };

        (int borderTileRow, int borderTileCol) = borderTile.Pos;
        int i = 1;
        while (true)
        {
          // Checks the next tile to the top right
          (int row, int col) nextPosInDiagonal = ((borderTileRow - i), (borderTileCol + i));
          Tile nextTileInDiagonal = this.tiles.Find(tile => tile.Pos == nextPosInDiagonal);
          if (nextTileInDiagonal == null)
            break;

          diagonal.Add(nextTileInDiagonal);
          i++;
        }
        diagonals.Add(diagonal);
      }
      return diagonals;
    }
  }
}
