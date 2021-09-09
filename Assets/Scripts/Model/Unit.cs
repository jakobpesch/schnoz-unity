using System;
using System.Collections;
using System.Collections.Generic;
using Schnoz;
using UnityEngine;

namespace Schnoz
{
  [Serializable]
  public class Unit
  {
    private Guid id;
    public Guid Id
    {
      get => this.id;
    }
    private string unitName;
    public string UnitName
    {
      get => this.unitName;
    }
    private float vision;
    public float Vision
    {
      get => this.vision;
    }
    private Player owner;
    public Player Owner
    {
      get => owner;
    }
    public Unit(Player owner, string unitName = "Bob", float vision = 3f)
    {
      this.owner = owner;
      this.unitName = unitName;
      this.vision = vision;
      this.id = Guid.NewGuid();
    }
  }
}
