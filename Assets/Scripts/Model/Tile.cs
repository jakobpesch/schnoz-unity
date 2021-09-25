using System;
using TypeAliases;
using UnityEngine;

namespace Schnoz
{
  [Serializable]
  public class Tile
  {
    public Guid Id { get; }
    public Coordinate Coordinate { get; private set; }
    public Terrain Terrain { get; private set; }
    public Unit Unit { get; private set; }
    public bool Visible { get; private set; }
    public int Row { get => this.Coordinate.row; }
    public int Col { get => this.Coordinate.col; }
    public bool Placeable
    {
      get => this.Terrain != null ? this.Terrain.Placeable : false;
    }
    public Tile(int row, int col)
    {
      this.Id = Guid.NewGuid();
      this.Coordinate = new Coordinate(row, col);
      this.Terrain = null;
      this.Unit = null;
      this.Visible = false;
    }

    public Tile(int row, int col, Unit unit = null) : this(row, col)
    {
      this.Unit = unit;
    }
    public Tile(int row, int col, Terrain terrain = null) : this(row, col)
    {
      this.Terrain = terrain;
    }
    public void SetUnit(Unit unit)
    {
      this.Unit = unit;
    }
    public void SetTerrain(Terrain terrain)
    {
      this.Terrain = terrain;
    }
    public void SetVisibility(bool visibility)
    {
      this.Visible = visibility;
    }
  }
}
