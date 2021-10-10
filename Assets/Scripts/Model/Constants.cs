using System.Linq;
using System.Collections.Generic;
using UnityEngine;
namespace Schnoz {
  public static class Constants {
    public const ushort port = 8007;
    public static Dictionary<RuleNames, RuleLogic> RuleNameToRuleLogicDict = new Dictionary<RuleNames, RuleLogic>() {
      {RuleNames.Water, RuleLogicMethods.Water},
      {RuleNames.DiagonalToTopRight, RuleLogicMethods.DiagonalToTopRight},
    };
  }
  public enum TerrainType {
    Grass = 0, Water = 1, Bush = 2, Stone = 3
  }
  public enum CardType {
    Single_1 = 0, Straight_2 = 1, Straight_3 = 2, Straight_4 = 3, Diagonal_2 = 4, Corner_3 = 5, L_4 = 6, Z_4 = 7, Horse_3 = 8, Paralell_4 = 9
  }
  public enum InputEventNames {
    OnMouseUp, OnMouseEnter, OnMouseExit, RotateRightButton, RotateLeftButton, MirrorHorizontalButton, MirrorVerticalButton
  }

  public enum RuleNames {
    DiagonalToTopRight, Water
  }

  public static class RuleLogicMethods {
    public static RuleLogic DiagonalToTopRight =
    (Player player, Map map) => {
      Debug.Log($"RuleLogic: DiagonalToTopRight, Player: {player.Id}");

      List<List<Tile>> unitDiagonals = new List<List<Tile>>();
      Debug.Log($"MapDiagonals: {map.DiagonalsFromBottomLeftToTopRight.Count}");
      foreach (List<Tile> diagonal in map.DiagonalsFromBottomLeftToTopRight) {
        List<Tile> unitDiagonal = new List<Tile>();
        foreach (Tile tile in diagonal) {
          Debug.Log(tile.Coordinate);
          if (tile.Unit != null && tile.Unit.OwnerId == player.Id) {
            unitDiagonal.Add(tile);
          } else {
            if (unitDiagonal.Count >= 3) {
              unitDiagonals.Add(unitDiagonal);
            }
            unitDiagonal = new List<Tile>();
          }
        }
      }
      List<List<Tile>> validUnitDiagonals = new List<List<Tile>>();
      foreach (List<Tile> unitDiagonal in unitDiagonals.Where(ud => ud.Count() >= 3)) {
        validUnitDiagonals.Add(unitDiagonal);
      }
      int points = validUnitDiagonals.Count;
      return new RuleEvaluation(RuleNames.DiagonalToTopRight, player.Id, points);//, (List<object>)validUnitDiagonals);
    };

    public static RuleLogic Water =
    (Player player, Map map) => {

      var unitsNearWater = map.Units.Where(unit => {
        Tile tile = map.CoordinateToTileDict[unit.Coordinate];
        List<Tile> at = map.GetAdjacentTiles(tile);
        return unit.OwnerId == player.Id && at.Any(t => t != null && t.Terrain.Type == TerrainType.Water);
      });
      Debug.Log($"RuleLogic: Water, Player: {player.Id}");
      int points = unitsNearWater.Count();
      return new RuleEvaluation(RuleNames.Water, player.Id, points);//, (List<object>)(new List<Tile>() { }));
    };
  }

  public class Defaults {
    public const int DeckSize = 30;
  }
}

// public void GeometryRule(string geometry, string minmax, Player player)
// {
//   string ruleName = geometry + minmax;

//   int points = 0;

//   if (geometry == "diagonal")
//   {
//     

//   }

//   if (geometry == "holes")
//   {
//     List<GameObject> holes = new List<GameObject>();
//     Func<GameObject, bool> emptyAndPlaceableAndAtLeasteOnePlayer = H.And(T.Empty, H.And(T.Placeable, T.AdjacentUnitPlayer));
//     foreach (GameObject emptyPlaceableTile in M.I.Map_Tiles().Where(emptyAndPlaceableAndAtLeasteOnePlayer))
//     {
//       List<GameObject> adjacentTiles = T.AdjacentTiles(emptyPlaceableTile);
//       Func<GameObject, bool> allyOrTerrain = H.Or(T.UnitIsAllied, H.Not(T.Placeable));
//       if (adjacentTiles.All(allyOrTerrain)) holes.Add(emptyPlaceableTile);
//     }

//     points = holes.Count() * (minmax == "min" ? -1 : 1);

//     AddRuleToPlayer(player, ruleName, holes);
//   }
// }
