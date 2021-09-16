using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using Schnoz;
using TypeAliases;
using UnityEngine;

namespace Schnoz
{
  [Serializable]
  public class Tile
  {
    private Guid id;
    public Guid Id
    {
      get => this.id;
    }
    public int Row
    {
      get => this.coordinate.row;
    }
    public int Col
    {
      get => this.coordinate.col;
    }
    private Coordinate coordinate;
    public Coordinate Coordinate
    {
      get => this.coordinate;
    }
    private TileArea area;
    public TileArea Area
    {
      get => this.Area;
    }
    public bool Visible { get => this.visible; }
    private bool visible = false;
    public List<Guid> AdjacentTilesIds;
    private bool placeable;
    public bool Placeable
    {
      get => this.terrain != null ? this.terrain.Placeable : false;
    }
    private Terrain terrain;
    public Terrain Terrain
    {
      get => terrain;
    }
    private List<Tile> adjacentTiles;
    public List<Tile> AdjacentTiles
    {
      get => adjacentTiles;
    }
    private List<Unit> adjacentEnemies;
    public List<Unit> AdjacentEnemies
    {
      get => adjacentEnemies;
    }
    private List<Unit> adjacentAllies;
    public List<Unit> AdjacentAllies
    {
      get => adjacentAllies;
    }
    private Unit unit = null;
    public Unit Unit
    {
      get => unit;
      set => unit = value;
    }

    public Tile(int row, int col)
    {
      this.id = Guid.NewGuid();
      this.coordinate = new Coordinate(row, col);
    }

    public Tile(int row, int col, Unit unit = null) : this(row, col)
    {
      this.unit = unit;
    }
    public Tile(int row, int col, Terrain terrain = null) : this(row, col)
    {
      this.terrain = terrain;
    }

    public void SetArea(TileArea area)
    {
      this.area = area;
    }
    public void SetTerrain(Terrain terrain)
    {
      this.terrain = terrain;
    }
    public void SetAdjacentEnemies(List<Unit> adjacentEnemies)
    {
      this.adjacentEnemies = adjacentEnemies;
    }
    public void SetAdjacentAllies(List<Unit> adjacentAllies)
    {
      this.adjacentAllies = adjacentAllies;
    }
    public void SetVisibility(bool value)
    {
      this.visible = value;
    }
  }
}
