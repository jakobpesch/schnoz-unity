using System;
using System.Linq;
// using System.ComponentModel;
using System.Collections.Generic;
using Utils;
using UnityEngine;
using TypeAliases;
namespace Schnoz
{
  [Serializable]
  public class Schnoz : Observable
  {
    public Schnoz(GameSettings gameSettings)
    {
      this.gameSettings = gameSettings;
    }
    public GameSettings gameSettings;
    private Map map;
    public Map Map
    {
      get => this.map;
      set
      {
        this.map = value;
      }
    }
    [SerializeField] private Deck deck;
    public Deck Deck
    {
      get => this.deck;
      set
      {
        this.deck = value;
      }
    }


    [SerializeField] private List<Card> openCards = new List<Card>();
    public List<Card> OpenCards
    {
      get => this.openCards;
      set
      {
        this.openCards = value;
      }
    }
    [SerializeField] private List<Player> players;
    [SerializeField] private int turn;
    [SerializeField] private int stage;
    public List<int> PlayersIds { get => this.gameSettings.PlayerIds; }
    public int ActivePlayerId { get; private set; }
    public Player ActivePlayer
    {
      get => this.gameSettings.IdToPlayerDict.ContainsKey(this.ActivePlayerId)
      ? this.gameSettings.IdToPlayerDict[this.ActivePlayerId]
      : null;
    }
    public Player NeutralPlayer;

    private void EvaluateRules(ref Player player, List<Rule> rules)
    {
      List<RuleEvaluation> turnEvaluation = new List<RuleEvaluation>();
      foreach (Rule rule in rules)
      {
        turnEvaluation.Add(rule.Evaluate(player, this.Map));
      }
      player.AddTurnEvaluation(turnEvaluation);
    }
    public void CreateMap()
    {
      Debug.Log("Map will be Created");
      this.Map = new Map(this.gameSettings.NRows, this.gameSettings.NCols);
      this.NeutralPlayer = new Player(3);
      this.PlaceUnit(2, this.Map.CenterTile.Coordinate);
    }

    public void CreateDeck()
    {
      Debug.Log("Deck will be Created");
      this.Deck = new Deck(this.gameSettings.DeckSize);
    }

    public void ShuffleDeck()
    {
      Debug.Log("Deck will be Shuffled");
      this.deck.Shuffle();
    }

    public void DrawCards()
    {
      for (int i = 0; i < gameSettings.NumberOfCardsPerTurn; i++)
      {
        this.Deck.Draw();
      }
    }

    public void PlaceUnit(int ownerId, Coordinate coord)
    {
      Debug.Log($"Placing Unit for player {ownerId}");
      Tile tile = this.map.Tiles.Find(tile => tile.Coordinate == coord);
      Unit unit = new Unit(ownerId, coord);
      tile.SetUnit(unit);
    }
    public void RemoveUnit(Coordinate coord)
    {
      Debug.Log("Removing Unit");
      Tile tile = this.map.Tiles.Find(tile => tile.Coordinate == coord);
      tile.SetUnit(null);
    }
    public void PlaceUnitFormation(int ownerId, Coordinate coord, UnitFormation unitFormation)
    {
      foreach (Coordinate offset in unitFormation.Arrangement)
      {
        int row = coord.row + offset.row;
        int col = coord.col + offset.col;
        Coordinate newCoord = coord + offset;
        if (newCoord.row < 0 ||
            newCoord.col < 0 ||
            newCoord.row > this.gameSettings.NRows - 1 ||
            newCoord.col > this.gameSettings.NCols - 1)
        {
          break;
        }
        this.PlaceUnit(ownerId, newCoord);
      }
    }
    public void Init()
    {
      // // Debug.Log("GameManager unlistens to: OnAllPlayersPresent");
      // this.eventManager.OnAllPlayersPresent -= this.Init;

      // this.turn = 0;

      // this.Map.ClearTiles();

      // foreach (Player player in gameSettings.Players)
      // {
      //   player.SetSinglePieces(gameSettings.NumberOfSinglePieces);
      // }
      // SpawnManager.I.SpawnUnit(this.capital, this.map.CenterTile, this.NeutralPlayer);

      // this.Map.Scan();



      // this.SetActivePlayer(0);

      // this.Map.GameStarted = true;
      // UIManager.I.Init();

      // M.I.UI_UpdateScore();

      // M.I.UI_PopulateProgressBar();

      // M.I.UI_UpdateSinglePieces();
      // M.I.UI_UpdateStones();

      // TakeStone();
      // M.I.Map_Scan();
      // this.map.GameStarted = true;
    }
    public void SetActivePlayer(int turn)
    {
      this.ActivePlayerId = gameSettings.TurnOrder[turn];
      Debug.Log($"{this.ActivePlayerId} is the current Player");
    }
    public void EndTurn()
    {
      this.SetActivePlayer(++this.turn);
      this.UpdateRules();
    }
    private void UpdateRules()
    {
      foreach (Player player in gameSettings.Players)
      {
        this.ApplyRules(player);
      }
    }
    private void ApplyRules(Player player)
    {
      List<RuleEvaluation> roundEvaluation = new List<RuleEvaluation>();
      foreach (Rule rule in gameSettings.Rules)
      {
        roundEvaluation.Add(rule.Evaluate(player, this.Map));
      }
      player.AddTurnEvaluation(roundEvaluation);
    }
  }

}
