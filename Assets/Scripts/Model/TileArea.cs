using System;
using System.Collections;
using System.Collections.Generic;
using Schnoz;
using UnityEngine;

namespace Schnoz
{
  [Serializable]
  public class TileArea : IEquatable<TileArea>
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

    public override bool Equals(object obj) => this.Equals(obj as TileArea);
    public bool Equals(TileArea tileArea)
    {

      if (tileArea is null)
      {
        return false;
      }

      // Optimization for a common success case.
      if (System.Object.ReferenceEquals(this, tileArea))
      {
        return true;
      }

      // If run-time types are not exactly the same, return false.
      if (this.GetType() != tileArea.GetType())
      {
        return false;
      }

      // Return true if the fields match.
      // Note that the base class is not invoked because it is
      // System.Object, which defines Equals as reference equality.
      return tileArea.id == this.id;
    }
    public static bool operator ==(TileArea lhs, TileArea rhs)
    {
      if (lhs is null)
      {
        if (rhs is null)
        {
          return true;
        }

        // Only the left side is null.
        return false;
      }
      // Equals handles case of null on right side.
      return lhs.Equals(rhs);
    }

    public static bool operator !=(TileArea lhs, TileArea rhs) => !(lhs == rhs);
  }
}
