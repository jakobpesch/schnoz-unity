using System;
using System.Collections;
using System.Collections.Generic;
using Schnoz;
using TypeAliases;
using UnityEngine;

namespace Schnoz
{
  [Serializable]
  public class Terrain
  {
    public Guid Id { get; private set; }
    public TerrainType Type { get; private set; }
    public bool Placeable { get; private set; }
    public Coordinate Coordinate { get; private set; }
    public Terrain(TerrainType terrainType, Coordinate coordinate)
    {
      this.Id = Guid.NewGuid();
      this.Type = terrainType;
      this.Placeable = this.Type == TerrainType.Grass;
      this.Coordinate = coordinate;
    }
    public Terrain(NetTerrain netTerrain) : this((TerrainType)netTerrain.t, new Coordinate(netTerrain.r, netTerrain.c))
    {

    }
  }
}
