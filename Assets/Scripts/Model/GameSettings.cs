using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Schnoz;
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
    [SerializeField] private List<Player> players;
    public List<Player> Players
    {
      get => players;
    }
    [SerializeField] private int nRows = 11, nCols = 11;
    public int NRows { get => nRows; }
    public int NCols { get => nCols; }

    public void SetPlayers(List<Player> players)
    {
      this.players = players;
    }
    public void CreateStages()
    {
      // if (this.players.Count == 2) ... TODO: Player count dependant stage
      Debug.Log(this.players.Count);
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
