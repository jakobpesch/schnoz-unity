using System.Linq;
using System.Collections.Generic;
using UnityEngine;
namespace Schnoz {
  public static class Constants {
    public const ushort port = 8007;
    public const int numberOfStages = 4;
    public static Dictionary<RuleNames, RuleLogic> RuleNameToRuleLogicDict = new Dictionary<RuleNames, RuleLogic>() {
      {RuleNames.Water, RuleLogicMethods.Water},
      {RuleNames.DiagonalToTopRight, RuleLogicMethods.DiagonalToTopRight},
      {RuleNames.Holes, RuleLogicMethods.Holes},
    };
    public static Dictionary<PlayerIds, Color> PlayerColors = new Dictionary<PlayerIds, Color>() {
      {PlayerIds.Player1, new Color(0.6235294f,0.3137255f,0.3137255f)},
      {PlayerIds.Player2, new Color(0.1204165f,0.4365113f,0.6226415f)},
      {PlayerIds.NeutralPlayer, Color.gray},
    };
    public const int mapSize = 31, ratioGrass = 50, ratioStone = 1, ratioWater = 3, ratioBush = 3, SaveArea = 3;
  }
  public enum TerrainType {
    Grass, Water, Bush, Stone
  }
  public enum CardType {
    Single_1, Straight_2, Straight_3, Straight_4, Diagonal_2, Corner_3, L_4, Z_4, Horse_3, Paralell_4
  }
  public enum RenderTypes {
    Map, Turns, Highlight, OpenCards, SelectedCard, Rules, Score, SinglePieces, Timer, GameSettings
  }
  public enum InputEventNames {
    OnMouseUp, OnMouseEnter, OnMouseExit, RotateRightButton, RotateLeftButton, MirrorHorizontalButton, MirrorVerticalButton, SelectCard, OnCardViewClick, ShowRelevantTiles
  }

  public enum RuleNames {
    DiagonalToTopRight, Water, Holes
  }

  public enum PlayerRoles {
    ADMIN, PLAYER
  }

  public enum PlayerIds {
    Player1, Player2, NeutralPlayer
  }

  public static class RuleLogicMethods {
    public static RuleLogic DiagonalToTopRight =
    (Player player, Map map) => {
      List<List<Tile>> unitDiagonals = new List<List<Tile>>();
      foreach (List<Tile> diagonal in map.DiagonalsFromBottomLeftToTopRight) {
        List<Tile> unitDiagonal = new List<Tile>();
        foreach (Tile tile in diagonal) {
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
    public static RuleLogic Holes =
    (Player player, Map map) => {
      List<Tile> holes = new List<Tile>();
      // Debug.Log(map.VisibleTiles.Count);
      foreach (Tile visibleTile in map.VisibleTiles) {
        var placeable = visibleTile.Placeable;
        if (!placeable) continue;
        // Debug.Log($"HOLES {visibleTile.Coordinate}: Placeable");
        var atLeastOneAllyAdjacent = map.GetAdjacentAllies(visibleTile, player.Id).Count > 0;
        if (!atLeastOneAllyAdjacent) continue;
        // Debug.Log($"HOLES {visibleTile.Coordinate}: AllyNearby");
        var enclosed = map.GetAdjacentTiles(visibleTile).All(tile => !tile.Placeable);
        if (!enclosed) continue;
        // Debug.Log($"HOLES {visibleTile.Coordinate}: Enclosed");

        var enemyAdjacent = map.GetAdjacentEnemies(visibleTile, player.Id).Count > 0;
        if (enemyAdjacent) continue;
        // Debug.Log($"HOLES {visibleTile.Coordinate}: No enemy Adjacent");

        holes.Add(visibleTile);
      }
      int points = holes.Count;
      return new RuleEvaluation(RuleNames.Holes, player.Id, points);//, (List<object>)validUnitDiagonals);
    };

    public static RuleLogic Water =
    (Player player, Map map) => {
      List<Tile> waterTiles = map.Tiles.Where(tile => tile.Terrain.Type == TerrainType.Water).ToList();
      List<Tile> relevantTiles = new List<Tile>();
      foreach (Tile waterTile in waterTiles) {
        var hasAdjacentAlly = map.GetAdjacentTiles(waterTile).Any(tile =>
                tile != null && tile.Unit != null && tile.Unit.OwnerId == player.Id);
        if (hasAdjacentAlly) {
          relevantTiles.Add(waterTile);
        }
      }
      int points = relevantTiles.Count();
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
