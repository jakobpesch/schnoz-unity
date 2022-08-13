using System.Collections.Generic;
using System.Linq;
using Schnoz;
public class Standing {
  public Player Player { get; private set; }
  public int Score { get; private set; }
  public Dictionary<RuleNames, RuleEvaluation> RuleNameToRuleEvaluationDict { get; private set; }
  public Standing(Player player, List<Rule> rules, Map map) {
    this.Player = player;
    this.Score = 0;
    RuleNameToRuleEvaluationDict = rules.Select(rule => rule.Evaluate(player, map)).ToDictionary(rule => rule.RuleName);
  }
  public void SetScore(int score) {
    this.Score = score;
  }
}
