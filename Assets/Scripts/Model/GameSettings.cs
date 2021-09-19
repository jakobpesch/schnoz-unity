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
    [SerializeField] private List<Player> turnOrder = new List<Player>();
    public List<Player> TurnOrder { get => this.turnOrder; }
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
    [SerializeField] private List<Player> players;
    public List<Player> Players
    {
      get => players;
    }
    [SerializeField] private int nRows = 11, nCols = 11;
    public int NRows { get => nRows; }
    public int NCols { get => nCols; }

    public GameSettings(int nRows, int nCols, int numberOfCardsPerTurn, int numberOfSinglePieces, int numberOfStages, int deckSize, List<Player> players)
    {
      this.nCols = nCols;
      this.nRows = nRows;
      this.numberOfCardsPerTurn = numberOfCardsPerTurn;
      this.numberOfSinglePieces = numberOfSinglePieces;
      this.numberOfStages = numberOfStages;
      this.SetPlayers(players);
      this.CreateStages();
    }
    private void SetPlayers(List<Player> players)
    {
      this.players = players;
    }
    private void CreateStages()
    {
      // if (this.players.Count == 2) ... TODO: Player count dependant stage
      List<Player> stage = new List<Player>() { this.players[0], this.players[1], this.players[1], this.players[0], this.players[0], this.players[1] };
      List<Player> reverseStage = new List<Player>(stage);
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
