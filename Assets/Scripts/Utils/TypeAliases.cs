
using System;
using System.Collections;
using System.Collections.Generic;

namespace TypeAliases {
  public struct Coordinate {
    public Coordinate(int row = 0, int col = 0) {
      this.row = row;
      this.col = col;
    }
    public readonly int col;
    public readonly int row;
    public static Coordinate operator -(Coordinate lhs, Coordinate rhs)
        => new Coordinate(lhs.row - rhs.row, lhs.col - rhs.col);
    public static Coordinate operator +(Coordinate lhs, Coordinate rhs)
        => new Coordinate(lhs.row + rhs.row, lhs.col + rhs.col);
    public static bool operator ==(Coordinate lhs, Coordinate rhs)
        => lhs.row == rhs.row && lhs.col == rhs.col;
    public static bool operator !=(Coordinate lhs, Coordinate rhs)
        => !(lhs == rhs);

    public override string ToString() {
      return $"({row}, {col})";
    }
    public override bool Equals(object o) {
      Coordinate rhs = (Coordinate)o;
      return row == rhs.row && col == rhs.col;
    }

    public override int GetHashCode() {
      unchecked {
        int hash = 17;
        hash = hash * 23 + row.GetHashCode();
        hash = hash * 23 + col.GetHashCode();
        return hash;
      }
    }
  }
  public class Arrangement : IEnumerable<Coordinate> {
    private readonly List<Coordinate> _coordinates;
    public Arrangement() {
      _coordinates = new List<Coordinate>();
    }
    public void Add((int row, int col) coordinate) {
      _coordinates.Add(new Coordinate(coordinate.row, coordinate.col));
    }
    public void Add(Coordinate coordinate) {
      _coordinates.Add(coordinate);
    }
    public IEnumerator<Coordinate> GetEnumerator() {
      return _coordinates.GetEnumerator();
    }
    IEnumerator IEnumerable.GetEnumerator() {
      return GetEnumerator();
    }
  }
}
