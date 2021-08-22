using System.Linq;
using System.Collections.Generic;
namespace Schnoz
{
  public enum TerrainType
  {
    Bush, Water, Stone
  }
  public enum CardType
  {
    Single_1, Straight_2, Straight_3, Straight_4, Diagonal_2, Corner_3, L_4, Z_4, Horse_3, Paralell_4,
  }

  public static class RuleLogicMethods
  {
    public static RuleLogic DiagonalToTopRight =
    (Player player, Map map) =>
    {
      List<List<Tile>> unitDiagonals = new List<List<Tile>>();
      foreach (List<Tile> diagonal in map.DiagonalsFromBottomLeftToTopRight)
      {
        List<Tile> unitDiagonal = new List<Tile>();
        foreach (Tile tile in diagonal)
        {
          if (tile.Unit.Owner != player)
          {
            if (unitDiagonal.Count >= 3)
            {
              unitDiagonals.Add(unitDiagonal);
            }
            unitDiagonal = new List<Tile>();
          }
          else
          {
            unitDiagonal.Add(tile);
          }
        }
      }
      List<List<Tile>> validUnitDiagonals = new List<List<Tile>>();
      foreach (List<Tile> unitDiagonal in unitDiagonals.Where(ud => ud.Count() >= 3))
      {
        validUnitDiagonals.Add(unitDiagonal);
      }
      int points = validUnitDiagonals.Count;
      return new RuleEvaluation(points, (IList<object>)validUnitDiagonals);
    };

    public static RuleLogic Water =
    (Player player, Map map) =>
    {
      return new RuleEvaluation(1, (IList<object>)(new List<Tile>() { }));
    };
  }
  public enum RuleType
  {
    DiagonalsToTopRight, Water, Stone, Holes
  }

  public class Defaults
  {
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
