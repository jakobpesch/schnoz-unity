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


    public List<List<RuleEvaluation>> CurrentEvaluation
    {
      get
      {
        var playerEvals = new List<List<RuleEvaluation>>();
        Debug.Log(this.Players.Count);
        foreach (Player player in this.Players)
        {
          Debug.Log($"PlayerId: {player.Id}");
          var ruleEvals = new List<RuleEvaluation>();
          foreach (Rule rule in this.gameSettings.Rules)
          {
            RuleEvaluation eval = rule.Evaluate(player, this.Map);
            Debug.Log($"Adding Rule: {eval.RuleName}");
            ruleEvals.Add(eval);
          }
          playerEvals.Add(ruleEvals);
        }
        return playerEvals;
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
    public List<Player> Players { get => this.gameSettings.Players; }
    public int Turn { get; private set; }
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
      this.Deck.DiscardOpenCards();
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
      List<Coordinate> underlayingCoords = this.GetUnderlayingCoords(coord, unitFormation);
      foreach (Coordinate c in underlayingCoords)
      {
        this.PlaceUnit(ownerId, c);
      }
    }

    private List<Coordinate> GetUnderlayingCoords(Coordinate coord, UnitFormation unitFormation)
    {
      List<Coordinate> underlayingCoords = new List<Coordinate>();
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
        underlayingCoords.Add(newCoord);
      }
      return underlayingCoords;
    }
    private List<Tile> GetUnderlayingTiles(Coordinate coord, UnitFormation unitFormation)
    {
      return this.GetUnderlayingCoords(coord, unitFormation)
        .Select(c => this.Map.CoordinateToTileDict[c]).ToList();
    }

    public bool CanPlaceUnitFormation(int ownerId, Coordinate coord, UnitFormation unitFormation)
    {
      Tile tile = this.Map.CoordinateToTileDict[coord];

      Func<Tile, bool> IsAdjacentToNeutralOrAlly = (Tile tile) => this.Map
        .GetAdjacentTiles(tile)
        .Any(t => t != null && t.Unit != null && (t.Unit.OwnerId == ownerId || t.Unit.OwnerId == 2));

      List<Tile> underlayingTiles = this.GetUnderlayingTiles(coord, unitFormation);

      return underlayingTiles.All(tile => tile.Placeable)
        && underlayingTiles.Any(tile => IsAdjacentToNeutralOrAlly(tile));
    }

    public List<Coordinate> GetAllPossiblePlacements(int ownerId)
    {
      // WIP
      List<Coordinate> possibleCoordinates = new List<Coordinate>();
      Func<Unit, bool> unitIsAllyOrNeutral = (Unit unit) => unit.OwnerId == ownerId || unit.OwnerId == 2;
      foreach (Unit unit in this.Map.Units.Where(unitIsAllyOrNeutral))
      {
        Func<Tile, bool> tileIsPlaceable = (Tile tile) => tile.Placeable;
        foreach (Coordinate c in this.Map
          .GetAdjacentTiles(this.Map.CoordinateToTileDict[unit.Coordinate])
          .Where(tileIsPlaceable)
          .Select(tile => tile.Coordinate))
        {
          Debug.Log(c);
        }
      }
      return possibleCoordinates;
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
      this.SetActivePlayer(++this.Turn);
      // this.UpdateRules();
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
