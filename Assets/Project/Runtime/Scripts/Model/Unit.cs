using System;
using System.Collections;
using System.Collections.Generic;
using Schnoz;
using UnityEngine;
using TypeAliases;

namespace Schnoz {
  [Serializable]
  public class Unit {
    public PlayerIds OwnerId { get; private set; }
    [SerializeField] private PlayerIds ownerId;
    public Coordinate Coordinate { get; private set; }
    public Unit(PlayerIds ownerId, Coordinate coordinate) {
      this.OwnerId = ownerId;
      this.ownerId = ownerId;
      this.Coordinate = coordinate;
    }
    public Unit(NetUnit netUnit) : this((PlayerIds)netUnit.i, new Coordinate(netUnit.r, netUnit.c)) { }
  }
}
