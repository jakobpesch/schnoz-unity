using System;
using System.Collections;
using System.Collections.Generic;
using Schnoz;

namespace Schnoz
{

  abstract public class Terrain
  {

    private Guid id;
    public Guid Id
    {
      get => this.id;
    }
    private TerrainType type;
    public TerrainType Type
    {
      get => this.type;
    }
    private bool placeable;
    public bool Placeable
    {
      get => this.placeable;
    }
    public Terrain()
    {
      this.id = Guid.NewGuid();
    }

    Guid GetGuid()
    {
      return this.id;
    }
  }
}
