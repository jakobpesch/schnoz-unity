using System;
using System.Collections;
using System.Collections.Generic;

namespace Schnoz
{
  public class UnitFormation
  {
    public UnitFormation()
    {
      this.arrangement = new List<(int, int)>();
      this.arrangement.Add((0, 0));
      this.arrangement.Add((0, 1));
    }
    private List<(int, int)> arrangement;
    public List<(int, int)> Arrangement
    {
      get => this.arrangement;
    }
    private int rotation = 0;
    private bool mirrorHorizontal = false;
    private bool mirrorVertical = false;
    public void RotateRight()
    {
      this.rotation++;
      if (this.rotation > 3)
      {
        this.rotation = 0;
      }
    }
    public void RotateLeft()
    {
      this.rotation--;
      if (this.rotation < 0)
      {
        this.rotation = 3;
      }
    }
    public void MirrorHorizontal()
    {
      this.mirrorHorizontal = !this.mirrorHorizontal;
    }
    public void MirrorVertical()
    {
      this.mirrorVertical = !this.mirrorVertical;
    }
  }
}
