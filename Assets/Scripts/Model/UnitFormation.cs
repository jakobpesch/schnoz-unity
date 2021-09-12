using System;
using System.Collections;
using System.Collections.Generic;
using TypeAliases;

namespace Schnoz
{
  public class UnitFormation
  {
    public static Dictionary<CardType, Arrangement> arrangements = new Dictionary<CardType, Arrangement>() {
      { CardType.Single_1, new Arrangement() { (0, 0) } },
      { CardType.Straight_2, new Arrangement() { (0, 0), (0, 1) } },
      { CardType.Straight_3, new Arrangement() { (0, 0), (0, 1), (0, 2) } },
      { CardType.Straight_4, new Arrangement() { (0, 0), (0, 1), (0, 2) , (0, 3) } },
      { CardType.Horse_3, new Arrangement() { (0, 0), (1, 0), (2, 1) } },
      { CardType.Corner_3, new Arrangement() { (0, 0), (0, 1), (1,0) } },
      { CardType.Z_4, new Arrangement() { (0, 0), (-1, 0), (-1, 1), (-2, 1) } },
      { CardType.L_4, new Arrangement() { (0, 0), (0, 1), (1, 0), (2, 0) } },
      { CardType.Paralell_4, new Arrangement() { (0, 0), (0, 2), (1, 0), (1, 2) } },
      { CardType.Diagonal_2, new Arrangement() { (0, 0), (1, 1)} },
    };
    public UnitFormation(Arrangement arrangement)
    {
      this.arrangement = arrangement;
    }
    public UnitFormation(CardType type)
    {
      this.arrangement = UnitFormation.arrangements[type];
    }
    private Arrangement arrangement;
    public Arrangement Arrangement
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
