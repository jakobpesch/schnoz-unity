using System;
using System.Collections.Generic;
using UnityEngine;
using Schnoz;
public class RulesView : MonoBehaviour {

  public RuleView Prefab;
  public List<RuleView> RuleViews = new List<RuleView>();
  public List<Rule> Rules { get => this.GameClient.GameClient.GameSettings.Rules; }
  public StandardGameClient GameClient;

  private void Awake() {

  }

  public void Render() {
    if (this.RuleViews.Count == this.Rules.Count) {
      this.UpdateRules();
      return;
    }

    int ruleViewCount = this.RuleViews.Count;
    for (int i = 0; i < ruleViewCount; i++) {
      Debug.Log(this.RuleViews.Count);
      RuleView ruleView = this.RuleViews[0];
      Destroy(ruleView.gameObject);
    }

    this.RuleViews = new List<RuleView>();
    for (int i = 0; i < this.Rules.Count; i++) {
      Rule rule = this.Rules[i];
      RuleView ruleView = GameObject.Instantiate<RuleView>(this.Prefab);
      ruleView.transform.SetParent(this.transform);
      RectTransform rect = ruleView.GetComponent<RectTransform>();
      rect.localScale = Vector3.one;
      ruleView.GameClient = this.GameClient;
      ruleView.gameObject.name = rule.RuleName.ToString();
      ruleView.RuleName = rule.RuleName;
      string fileName = ruleView.ruleName.ToString();
      if (fileName == "Water") {
        fileName = "terrain_water";
      }
      if (fileName == "Holes") {
        fileName = "rule_holes";
      }
      if (fileName == "DiagonalToTopRight") {
        fileName = "rule_diagonals-to-top-right";
      }
      if (fileName == "DiagonalToTopLeft") {
        fileName = "rule_diagonals-to-top-left";
      }
      Sprite sprite = Array.Find(Resources.LoadAll<Sprite>("Sprites/Tiles"), s => s.name == fileName);
      ruleView.Icon.sprite = sprite;
      var ruleWinnerPlayer = this.GameClient.GameClient.DetermineRuleWinner(rule.RuleName);
      ruleView.RuleWinner = ruleWinnerPlayer != null ? ruleWinnerPlayer.Id : PlayerIds.NeutralPlayer;
      ruleView.Background.color = Constants.PlayerColors[ruleView.RuleWinner];

      foreach (Player player in this.GameClient.GameClient.Players) {
        ruleView.PlayerIdToTextField[player.Id].text = rule.Evaluate(player, this.GameClient.GameClient.Map).Points.ToString();
      }

      this.RuleViews.Add(ruleView);
    }
  }

  public void UpdateRules() {
    for (int i = 0; i < this.Rules.Count; i++) {
      Rule rule = Rules[i];
      RuleView ruleView = this.RuleViews[i];
      ruleView.GameClient = this.GameClient;
      var ruleWinnerPlayer = this.GameClient.GameClient.DetermineRuleWinner(rule.RuleName);
      ruleView.RuleWinner = ruleWinnerPlayer != null ? ruleWinnerPlayer.Id : PlayerIds.NeutralPlayer;
      ruleView.Background.color = Constants.PlayerColors[ruleView.RuleWinner];

      foreach (Player player in this.GameClient.GameClient.Players) {
        ruleView.PlayerIdToTextField[player.Id].text = rule.Evaluate(player, this.GameClient.GameClient.Map).Points.ToString();
      }
    }
  }

}
