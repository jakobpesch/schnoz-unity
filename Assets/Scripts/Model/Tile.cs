using System;
using TypeAliases;
using UnityEngine;

namespace Schnoz {
  [Serializable]
  public class Tile {
    public Guid Id { get; }
    public Coordinate Coordinate { get; private set; }
    public Terrain Terrain { get; private set; }
    public Unit Unit { get; private set; }
    public bool Visible { get; private set; }
    public int Row { get => this.Coordinate.row; }
    public int Col { get => this.Coordinate.col; }
    public bool Placeable { get => this.Terrain.Placeable && this.Unit == null; }
    public Tile(int row, int col) {
      this.Id = Guid.NewGuid();
      this.Coordinate = new Coordinate(row, col);
      this.Terrain = new Terrain(TerrainType.Grass, this.Coordinate);
      this.Unit = null;
      this.Visible = false;
    }

    public Tile(int row, int col, Unit unit = null, TerrainType terrainType = TerrainType.Grass) : this(row, col) {
      this.Unit = unit;
      this.Terrain = new Terrain(terrainType, this.Coordinate);
    }
    public Tile(int row, int col, TerrainType terrainType = TerrainType.Grass) : this(row, col) {
      this.Terrain = new Terrain(terrainType, this.Coordinate);
    }
    public void SetUnit(Unit unit) {
      this.Unit = unit;
    }
    public void SetTerrain(TerrainType terrainType) {
      this.Terrain = new Terrain(terrainType, this.Coordinate);
    }
    public void SetVisibility(bool visibility) {
      this.Visible = visibility;
    }
  }
}
