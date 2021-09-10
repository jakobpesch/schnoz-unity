using System;
using System.Collections;
using System.Collections.Generic;
using Schnoz;
using UnityEngine;

namespace Schnoz
{
  public class TileArea
  {
    private Guid id;
    public Guid Id
    {
      get => this.id;
    }
    private List<Tile> tiles;
    public List<Tile> Tiles
    {
      get => this.tiles;
    }

    public TileArea(List<Tile> tiles = null)
    {
      tiles = tiles ?? new List<Tile>();
      this.id = Guid.NewGuid();
      this.tiles = tiles;
    }

    public void Add(Tile tile)
    {
      this.tiles.Add(tile);
    }
    public void Add(List<Tile> tiles)
    {
      this.tiles = tiles;
    }
  }
}
