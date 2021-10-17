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
    [SerializeField] private int numberOfStages = 6;
    public int NumberOfStages {
      get => this.numberOfStages;
    }
    [SerializeField] private int numberOfSinglePieces = 1;
    public int NumberOfSinglePieces {
      get => this.numberOfSinglePieces;
    }
    [SerializeField] private int numberOfCardsPerTurn = 4;
    public int NumberOfCardsPerTurn {
      get => this.numberOfCardsPerTurn;
    }
    [SerializeField] private int deckSize = 30;
    public int DeckSize {
      get => this.deckSize;
    }

    public List<Player> Players { get; private set; }
    public Dictionary<PlayerIds, Player> PlayerIdToPlayerDict { get; private set; }
    [SerializeField] private int nRows = 11, nCols = 11;
    public int NRows { get => nRows; }
    public int NCols { get => nCols; }
    public int NumberOfTurnsPerStage { get; private set; }

    public GameSettings(
      int nRows,
      int nCols,
      int numberOfCardsPerTurn,
      int numberOfSinglePieces,
      int numberOfStages,
      int deckSize,
      List<RuleNames> ruleNames) {
      this.nCols = nCols;
      this.nRows = nRows;
      this.numberOfCardsPerTurn = numberOfCardsPerTurn;
      this.numberOfSinglePieces = numberOfSinglePieces;
      this.numberOfStages = numberOfStages;
      this.Players = new List<Player>() { new Player(PlayerIds.Player1, this.numberOfSinglePieces), new Player(PlayerIds.Player2, this.numberOfSinglePieces) };
      this.PlayerIdToPlayerDict = Players.ToDictionary(player => player.Id);
      this.Rules = ruleNames.Select(ruleName => new Rule(ruleName, Constants.RuleNameToRuleLogicDict[ruleName])).ToList();
      this.RuleNameToRuleDict = this.Rules.ToDictionary(rule => {
        Debug.Log(rule.RuleName);
        return rule.RuleName;
      });
      this.CreateStages();
    }
    private void CreateStages() {
      // if (this.players.Count == 2) ... TODO: Player count dependant stage
      List<PlayerIds> stage = new List<PlayerIds>() { PlayerIds.Player1, PlayerIds.Player2, PlayerIds.Player2, PlayerIds.Player1, PlayerIds.Player1, PlayerIds.Player2 };
      this.NumberOfTurnsPerStage = stage.Count;
      List<PlayerIds> reverseStage = new List<PlayerIds>(stage);
      reverseStage.Reverse();
      for (int i = 0; i < this.numberOfStages; i++) {
        if (i % 2 == 0) {
          this.turnOrder = this.turnOrder.Concat(stage).ToList();
        } else {
          this.turnOrder = this.turnOrder.Concat(reverseStage).ToList();
        }
      }
    }
  }
}
