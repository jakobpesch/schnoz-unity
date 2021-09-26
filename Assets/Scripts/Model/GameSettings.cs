using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Schnoz
{
  [Serializable]
  public class GameSettings
  {
    public List<Rule> Rules = new List<Rule>() { new Rule(RuleLogicMethods.DiagonalToTopRight) };
    [SerializeField] private List<int> turnOrder = new List<int>();
    public List<int> TurnOrder { get => this.turnOrder; }
    [SerializeField] private int numberOfStages = 6;
    public int NumberOfStages
    {
      get => this.numberOfStages;
    }
    [SerializeField] private int numberOfSinglePieces = 1;
    public int NumberOfSinglePieces
    {
      get => this.numberOfSinglePieces;
    }
    [SerializeField] private int numberOfCardsPerTurn = 4;
    public int NumberOfCardsPerTurn
    {
      get => this.numberOfCardsPerTurn;
    }
    [SerializeField] private int deckSize = 30;
    public int DeckSize
    {
      get => this.deckSize;
    }
    [SerializeField] private List<int> playerIds;
    public List<int> PlayerIds
    {
      get => playerIds;
    }
    public List<Player> Players { get; private set; }
    public Dictionary<int, Player> IdToPlayerDict { get; private set; }
    [SerializeField] private int nRows = 11, nCols = 11;
    public int NRows { get => nRows; }
    public int NCols { get => nCols; }

    public GameSettings(int nRows, int nCols, int numberOfCardsPerTurn, int numberOfSinglePieces, int numberOfStages, int deckSize, List<int> playerIds)
    {
      this.nCols = nCols;
      this.nRows = nRows;
      this.numberOfCardsPerTurn = numberOfCardsPerTurn;
      this.numberOfSinglePieces = numberOfSinglePieces;
      this.numberOfStages = numberOfStages;
      this.SetPlayers(playerIds);
      this.CreateStages();
    }
    private void SetPlayers(List<int> playerIds)
    {
      this.playerIds = playerIds;
      this.Players = playerIds.Select(id => new Player(id)).ToList();
      this.IdToPlayerDict = Players.ToDictionary(player => player.Id);
    }
    private void CreateStages()
    {
      // if (this.players.Count == 2) ... TODO: Player count dependant stage
      List<int> stage = new List<int>() { 0, 1, 1, 0, 0, 1 };
      List<int> reverseStage = new List<int>(stage);
      reverseStage.Reverse();
      for (int i = 0; i < this.numberOfStages; i++)
      {
        if (i % 2 == 0)
        {
          this.turnOrder = this.turnOrder.Concat(stage).ToList();
        }
        else
        {
          this.turnOrder = this.turnOrder.Concat(reverseStage).ToList();
        }
      }
    }
  }
}
