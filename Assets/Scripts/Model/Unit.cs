using System;
using System.Collections;
using System.Collections.Generic;
using Schnoz;
using UnityEngine;
using TypeAliases;

namespace Schnoz
{
  [Serializable]
  public class Unit
  {
    public int OwnerId { get; private set; }
    [SerializeField] private int ownerId;
    public Coordinate Coordinate { get; private set; }
    public Unit(int ownerId, Coordinate coordinate)
    {
      this.OwnerId = ownerId;
      this.ownerId = ownerId;
      this.Coordinate = coordinate;
    }
    public Unit(NetUnit netUnit) : this(netUnit.i, new Coordinate(netUnit.r, netUnit.c)) { }
  }
}
