using System;
using System.Collections;
using System.Collections.Generic;
namespace Schnoz {
  /// <summary>
  /// Contains information about an evaluation of a rule.
  /// Points are the points scored/lost in this evaluation of the rule.
  /// RelevantTiles hold either List or a nested List of Tiles.
  /// </summary>
  [Serializable]
  public class RuleEvaluation {
    public RuleNames RuleName { get; private set; }
    public PlayerIds PlayerId { get; private set; }
    public int Points { get; private set; }
    // public IList<object> RelevantTiles { get; private set; }
    public RuleEvaluation(RuleNames ruleName, PlayerIds playerId, int Points)//, List<object> relevantTiles)
    {
      this.RuleName = ruleName;
      this.Points = Points;
      // this.RelevantTiles = relevantTiles;
      this.PlayerId = playerId;
    }
  }
}
