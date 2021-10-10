using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using TypeAliases;
using UnityEngine;

namespace Schnoz {
  public class UnitFormation {
    public CardType Type;
    public static Dictionary<int, CardType> unitFormationIdToTypeDict = new Dictionary<int, CardType>() {
      { 0, CardType.Single_1 },
      { 1, CardType.Straight_2 },
      { 2, CardType.Straight_3 },
      { 3, CardType.Straight_4 },
      { 4, CardType.Horse_3 },
      { 5, CardType.Corner_3 },
      { 6, CardType.Z_4 },
      { 7, CardType.L_4 },
      { 8, CardType.Paralell_4 },
      { 9, CardType.Diagonal_2 },
    };
    public static Dictionary<CardType, int> unitFormationTypeToIdDict = new Dictionary<CardType, int>() {
      { CardType.Single_1, 0 },
      { CardType.Straight_2, 1 },
      { CardType.Straight_3, 2 },
      { CardType.Straight_4, 3 },
      { CardType.Horse_3, 4 },
      { CardType.Corner_3, 5 },
      { CardType.Z_4, 6 },
      { CardType.L_4, 7 },
      { CardType.Paralell_4, 8 },
      { CardType.Diagonal_2, 9 },
    };
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

    public UnitFormation(CardType type) {
      this.Type = type;
      this.defaultArrangement = UnitFormation.arrangements[type];
    }
    private Arrangement defaultArrangement;
    public Arrangement DefaultArrangement {
      get => this.defaultArrangement;
    }
    public Arrangement Arrangement {
      get {
        Arrangement newArrangement = new Arrangement();
        foreach (Coordinate coordinate in this.defaultArrangement) {
          Coordinate tempCoord = new Coordinate(coordinate.row, coordinate.col);
          for (int i = 0; i < this.rotation; i++) {
            tempCoord = this.RotateRight(tempCoord);
          }
          if (this.mirrorHorizontal) {
            tempCoord = this.MirrorHorizontal(tempCoord);
          }
          if (this.mirrorVertical) {
            tempCoord = this.MirrorVertical(tempCoord);
          }
          newArrangement.Add(tempCoord);
        }
        return newArrangement;
      }
    }
    public int rotation = 0;
    public bool mirrorHorizontal = false;
    public bool mirrorVertical = false;
    private Coordinate RotateRight(Coordinate coordinate) {
      return new Coordinate(coordinate.col, -coordinate.row);
    }
    public Coordinate MirrorHorizontal(Coordinate coordinate) {
      return new Coordinate(coordinate.row, -coordinate.col);
    }
    public Coordinate MirrorVertical(Coordinate coordinate) {
      return new Coordinate(-coordinate.row, coordinate.col);
    }
    public void RotateRight() {
      this.rotation++;
      if (this.rotation > 3) {
        this.rotation = 0;
      }
    }
    public void RotateLeft() {
      this.rotation--;
      if (this.rotation < 0) {
        this.rotation = 3;
      }
    }
    public void MirrorHorizontal() {
      this.mirrorHorizontal = !this.mirrorHorizontal;
      Debug.Log($"MirrorHorizontal={this.mirrorHorizontal}");
    }
    public void MirrorVertical() {
      this.mirrorVertical = !this.mirrorVertical;
      Debug.Log($"MirrorVertical={this.mirrorVertical}");
    }
  }
}
