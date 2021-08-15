using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Schnoz;

namespace Schnoz
{
  public class GameSettings
  {
    public List<Rule> Rules = new List<Rule>() { RuleCollection.NullRule, RuleCollection.NullRule };
    private List<Player> turnOrder = new List<Player>();
    public List<Player> TurnOrder { get => this.turnOrder; }
    private int numberOfStages = 6;
    public int NumberOfStages
    {
      get => this.numberOfStages;
    }
    private int numberOfSinglePieces = 1;
    public int NumberOfSinglePieces
    {
      get => this.numberOfSinglePieces;
    }
    private int numberOfCardsPerTurn = 4;
    public int NumberOfCardsPerTurn
    {
      get => this.numberOfCardsPerTurn;
    }
    private List<Player> players;
    public List<Player> Players
    {
      get => players;
    }

    public void SetPlayers(List<Player> players)
    {
      this.players = players;
    }
    public void CreateStages()
    {
      // if (this.players.Count == 2) ... TODO: Player count dependant stage
      List<Player> stage = new List<Player>() { Players[0], Players[1], Players[1], Players[0], Players[0], Players[1] };
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
