using System;
using System.Collections;
using System.Collections.Generic;
using Schnoz;

namespace Schnoz
{
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
    public Unit()
    {
      this.id = Guid.NewGuid();
    }
  }
}
