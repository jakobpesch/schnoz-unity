using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Schnoz {
  [Serializable]
  public class GameSettings {
    public List<Rule> Rules { get; private set; }
    public Dictionary<RuleNames, Rule> RuleNameToRuleDict { get; }
    [SerializeField] private List<PlayerIds> turnOrder = new List<PlayerIds>();
    public List<PlayerIds> TurnOrder { get => this.turnOrder; }
    public int NumberOfStages { get; private set; }
    public int NumberOfSinglePieces { get; private set; }
    public int NumberOfCardsPerTurn { get; private set; }
    public int DeckSize { get; private set; }
    public List<Player> Players { get; private set; }
    public Dictionary<PlayerIds, Player> PlayerIdToPlayerDict { get; private set; }
    public int MapSize { get; private set; }
    public int SecondsPerTurn { get; private set; }
    public int NRows { get => this.MapSize; }
    public int NCols { get => this.MapSize; }
    public int NumberOfTurnsPerStage { get; private set; }

    public GameSettings(
      int mapSize,
      int numberOfCardsPerTurn,
      int numberOfSinglePieces,
      int numberOfStages,
      int secondsPerTurn,
      List<RuleNames> ruleNames) {
      this.MapSize = mapSize;
      this.NumberOfCardsPerTurn = numberOfCardsPerTurn;
      this.SecondsPerTurn = secondsPerTurn;
      this.NumberOfSinglePieces = numberOfSinglePieces;
      this.NumberOfStages = numberOfStages;
      this.Players = new List<Player>() { new Player(PlayerIds.Player1, this.NumberOfSinglePieces), new Player(PlayerIds.Player2, this.NumberOfSinglePieces) };
      this.PlayerIdToPlayerDict = Players.ToDictionary(player => player.Id);
      this.Rules = ruleNames.Select(ruleName => new Rule(ruleName, Constants.RuleNameToRuleLogicDict[ruleName])).ToList();
      this.RuleNameToRuleDict = this.Rules.ToDictionary(rule => rule.RuleName);
      this.CreateStages();
    }
    private void CreateStages() {
      // if (this.players.Count == 2) ... TODO: Player count dependant stage
      List<PlayerIds> stage = new List<PlayerIds>() { PlayerIds.Player1, PlayerIds.Player2, PlayerIds.Player2, PlayerIds.Player1, PlayerIds.Player1, PlayerIds.Player2 };
      this.NumberOfTurnsPerStage = stage.Count;
      List<PlayerIds> reverseStage = new List<PlayerIds>(stage);
      reverseStage.Reverse();
      for (int i = 0; i < this.NumberOfStages; i++) {
        if (i % 2 == 0) {
          this.turnOrder = this.turnOrder.Concat(stage).ToList();
        } else {
          this.turnOrder = this.turnOrder.Concat(reverseStage).ToList();
        }
      }
    }
  }
}
