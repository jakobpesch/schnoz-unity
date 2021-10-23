using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Schnoz;

public class RulesView : MonoBehaviour {

  public RuleView Prefab;
  public List<RuleView> RuleViews = new List<RuleView>();
  public List<Rule> Rules { get => this.GameClient.GameClient.GameSettings.Rules; }
  public StandardGameClient GameClient;

  public void Render() {
    if (this.RuleViews.Count == this.Rules.Count) {
      this.UpdateRules();
      return;
    }

    for (int i = 0; i < this.RuleViews.Count; i++) {
      RuleView ruleView = this.RuleViews[i];
      Destroy(ruleView.gameObject);
    }

    this.RuleViews = new List<RuleView>();
    for (int i = 0; i < this.Rules.Count; i++) {
      Rule rule = this.Rules[i];
      RuleView ruleView = GameObject.Instantiate<RuleView>(this.Prefab);
      ruleView.gameObject.name = rule.RuleName.ToString();
      ruleView.RuleName = rule.RuleName; // updates the image
      ruleView.RuleWinner = this.GameClient.GameClient.DetermineRuleWinner(rule.RuleName).Id;
      ruleView.Image.color = Constants.PlayerColors[ruleView.RuleWinner];

      for (int j = 0; j < this.GameClient.GameClient.Players.Count; j++) {
        Player player = this.GameClient.GameClient.Players[i];
        ruleView.PlayerIdToTextField[player.Id].text = rule.Evaluate(player, this.GameClient.GameClient.Map).Points.ToString();
      }

      this.RuleViews.Add(ruleView);
    }
  }

  public void UpdateRules() {
    for (int i = 0; i < this.Rules.Count; i++) {
      Rule rule = Rules[i];
      RuleView ruleView = this.RuleViews[i];
      var ruleWinnerPlayer = this.GameClient.GameClient.DetermineRuleWinner(rule.RuleName);
      ruleView.RuleWinner = ruleWinnerPlayer != null ? ruleWinnerPlayer.Id : PlayerIds.NeutralPlayer;
      ruleView.Image.color = Constants.PlayerColors[ruleView.RuleWinner];

      for (int j = 0; j < this.GameClient.GameClient.Players.Count; j++) {
        Player player = this.GameClient.GameClient.Players[j];
        ruleView.PlayerIdToTextField[player.Id].text = rule.Evaluate(player, this.GameClient.GameClient.Map).Points.ToString();
      }
    }
  }

}
