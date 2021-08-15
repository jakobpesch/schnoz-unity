using System;
using System.Collections;
using System.Collections.Generic;
namespace Schnoz
{
  /// <summary>
  /// Contains information about an evaluation of a rule.
  /// Points are the points scored/lost in this evaluation of the rule.
  /// RelevantTiles hold either List or a nested List of Tiles.
  /// </summary>
  [Serializable]
  public class RuleEvaluation
  {
    private int points;
    public int Points
    {
      get => points;
    }
    private IList<object> relevantTiles;
    public IList<object> RelevantTiles
    {
      get => relevantTiles;
      // set
      // {
      //   points = (value).Count * (name.Contains("min") ? -1 : 1);
      //   relevantTiles = value;
      // }
    }
    public RuleEvaluation(int Points, IList<object> RelevantTiles)
    {
      points = Points;
      relevantTiles = RelevantTiles;
    }
  }
}
