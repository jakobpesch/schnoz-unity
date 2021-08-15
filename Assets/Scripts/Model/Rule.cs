using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Schnoz;

namespace Schnoz
{
  public delegate RuleEvaluation RuleLogic(Player player);
  public class Rule
  {
    private string name { get; set; }
    /// <summary>
    /// Contains the rule logic. 
    /// </summary>
    private RuleLogic RuleLogic;
    public Rule(string Name, RuleLogic Logic)
    {
      name = Name;
      RuleLogic = Logic;
    }

    /// <summary>
    /// Evaluates the rule.
    /// </summary>
    /// <param name="player">The player for who the rule will be evaluated for.</param>
    /// <returns>A RuleEvaluation object containing information about the </returns>
    public RuleEvaluation Evaluate(Player player)
    {
      return RuleLogic(player);
    }

    // public void TerrainRule(string terrain, string minmax, Player player)
    // {
    //   string ruleName = terrain + minmax;

    //   int points = 0;

    //   List<GameObject> terrainTiles = M.I.Map_Tiles().Where(t =>
    //   {
    //     GameObject ter = t.Terrain;
    //     if (ter != null)
    //       return ter.GetComponent<TerrainProperties>().id == terrain;
    //     else return false;
    //   }).ToList();

    //   List<GameObject> validTerrainTiles = new List<GameObject>();
    //   foreach (GameObject terrainTile in terrainTiles)
    //   {
    //     bool playerIsAdjacentToTerrainTile =
    //       terrainTile.AdjacentTiles.Any(at =>
    //         at.UnitProperties?.Owner == player);

    //     if (playerIsAdjacentToTerrainTile)
    //       validTerrainTiles.Add(terrainTile);
    //   }

    //   points = validTerrainTiles.Count() * (minmax == "min" ? -1 : 1);

    //   AddRuleToPlayer(player, ruleName, validTerrainTiles);
    // }

    // public void GeometryRule(string geometry, string minmax, Player player)
    // {
    //   string ruleName = geometry + minmax;

    //   int points = 0;

    //   if (geometry == "diagonal")
    //   {
    //     List<List<GameObject>> unitDiagonals = new List<List<GameObject>>();
    //     foreach (List<GameObject> diagonal in M.I.Map_Diagonals())
    //     {
    //       List<GameObject> unitDiagonal = new List<GameObject>();
    //       foreach (GameObject tile in diagonal)
    //         if (tile.UnitProperties?.Owner != player)
    //         {
    //           if (unitDiagonal.Count() >= 3)
    //             unitDiagonals.Add(unitDiagonal);
    //           unitDiagonal = new List<GameObject>();
    //         }
    //         else
    //           unitDiagonal.Add(tile);
    //     }
    //     List<List<GameObject>> validUnitDiagonals = new List<List<GameObject>>();
    //     foreach (List<GameObject> unitDiagonal in unitDiagonals.Where(ud => ud.Count() >= 3))
    //       validUnitDiagonals.Add(unitDiagonal);

    //     points = validUnitDiagonals.Count() * (minmax == "min" ? -1 : 1);

    //     AddRuleToPlayer(player, ruleName, validUnitDiagonals);

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


  }

}





