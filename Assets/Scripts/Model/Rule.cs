using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Schnoz;
using UnityEngine;

namespace Schnoz {
  /// </summary>
  /// A function delegate that contains the rules logic.
  /// </summary>
  public delegate RuleEvaluation RuleLogic(Player player, Map map);
  [Serializable]
  public class Rule {
    public RuleNames RuleName { get; private set; }
    /// <summary>
    /// Contains the rule logic. 
    /// </summary>
    [SerializeField] private RuleLogic RuleLogic;
    public Rule(RuleLogic ruleLogic) {
      this.RuleLogic = ruleLogic;
    }

    /// <summary>
    /// Evaluates the rule.
    /// </summary>
    /// <param name="player">The player for who the rule will be evaluated for.</param>
    /// <returns>A RuleEvaluation object containing information about the </returns>
    public RuleEvaluation Evaluate(Player player, Map map) {
      RuleEvaluation eval = RuleLogic(player, map);
      return eval;
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



  }

}





