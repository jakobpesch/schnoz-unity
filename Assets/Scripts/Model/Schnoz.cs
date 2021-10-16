using System;
using System.Linq;
// using System.ComponentModel;
using System.Collections.Generic;
using Utils;
using UnityEngine;
using TypeAliases;
namespace Schnoz {
  [Serializable]
  public class Schnoz : Observable {
    public Schnoz(GameSettings gameSettings) {
      this.GameSettings = gameSettings;
      this.Turn = 0;
    }
    public GameSettings GameSettings { get; private set; }
    public Map Map {
      get; private set;
    }

    [SerializeField] private Deck deck;
    public Deck Deck {
      get => this.deck;
      set {
        this.deck = value;
      }
    }

    public Dictionary<int, Standing> PlayerIdToCurrentStandingDict {
      get {
        return this.Players
          .Select(player => new Standing(player, this.GameSettings.Rules, this.Map))
          .ToDictionary(standing => standing.Player.Id);
      }
    }

    [SerializeField] private List<Card> openCards = new List<Card>();
    public List<Card> OpenCards {
      get => this.openCards;
      set {
        this.openCards = value;
      }
    }
    public List<Player> Players { get => this.GameSettings.Players; }
    public int Turn { get; private set; }
    [SerializeField] private int stage;
    public List<int> PlayersIds { get => this.GameSettings.PlayerIds; }
    public int ActivePlayerId { get => this.GameSettings.TurnOrder[this.Turn]; }
    public Player ActivePlayer {
      get => this.GameSettings.IdToPlayerDict.ContainsKey(this.ActivePlayerId)
      ? this.GameSettings.IdToPlayerDict[this.ActivePlayerId]
      : null;
    }
    public Player NeutralPlayer;

    public void InitialiseMap() {
      Debug.Log("Map will be Created");
      this.Map = new Map(this.GameSettings.NRows, this.GameSettings.NCols);
      this.NeutralPlayer = new Player(3);
      this.PlaceUnit(2, this.Map.CenterTile.Coordinate);
    }

    public void CreateDeck() {
      Debug.Log("Deck will be Created");
      this.Deck = new Deck(this.GameSettings.DeckSize);
    }

    public void ShuffleDeck() {
      Debug.Log("Deck will be Shuffled");
      this.deck.Shuffle();
    }

    public void DrawCards() {
      this.Deck.DiscardOpenCards();
      for (int i = 0; i < GameSettings.NumberOfCardsPerTurn; i++) {
        this.Deck.Draw();
      }
    }
    public void PlaceTerrain(TerrainType type, Coordinate coord) {
      Debug.Log($"Placing Terrain of type {type}");
      Tile tile = this.Map.CoordinateToTileDict[coord];
      tile.SetTerrain(type);
    }

    // Not needed, since TerrainType.Grass does the trick
    // public void RemoveTerrain(Coordinate coord) {
    //   Debug.Log("Removing Terrain");
    //   Tile tile = this.Map.CoordinateToTileDict[coord];
    //   tile.SetTerrain(TerrainType.Grass);
    // }

    public void PlaceUnit(int ownerId, Coordinate coord) {
      Debug.Log($"Placing Unit for player {ownerId}");
      Tile tile = this.Map.CoordinateToTileDict[coord];
      Unit unit = new Unit(ownerId, coord);
      tile.SetUnit(unit);
    }
    public void RemoveUnit(Coordinate coord) {
      Debug.Log("Removing Unit");
      Tile tile = this.Map.CoordinateToTileDict[coord];
      tile.SetUnit(null);
    }
    public void PlaceUnitFormation(int ownerId, Coordinate coord, UnitFormation unitFormation) {
      List<Coordinate> underlayingCoords = this.GetUnderlayingCoords(coord, unitFormation);
      foreach (Coordinate c in underlayingCoords) {
        this.PlaceUnit(ownerId, c);
      }
    }

    public void CalculateScore() {
      var standingsPlayer1 = this.PlayerIdToCurrentStandingDict[0];
      var standingsPlayer2 = this.PlayerIdToCurrentStandingDict[1];

      this.GameSettings.RuleNameToRuleDict.Keys.ToList()
      .ForEach(ruleName => {
        var pointsPlayer1 = standingsPlayer1.RuleNameToRuleEvaluationDict[ruleName].Points;
        var pointsPlayer2 = standingsPlayer2.RuleNameToRuleEvaluationDict[ruleName].Points;
        if (pointsPlayer1 > pointsPlayer2) {
          standingsPlayer1.SetScore(standingsPlayer1.Score + 1);
        } else {
          standingsPlayer2.SetScore(standingsPlayer2.Score + 1);
        }
      });
    }


    private List<Coordinate> GetUnderlayingCoords(Coordinate coord, UnitFormation unitFormation) {
      List<Coordinate> underlayingCoords = new List<Coordinate>();
      foreach (Coordinate offset in unitFormation.Arrangement) {
        int row = coord.row + offset.row;
        int col = coord.col + offset.col;
        Coordinate newCoord = coord + offset;
        if (newCoord.row < 0 ||
            newCoord.col < 0 ||
            newCoord.row > this.GameSettings.NRows - 1 ||
            newCoord.col > this.GameSettings.NCols - 1) {
          break;
        }
        underlayingCoords.Add(newCoord);
      }
      return underlayingCoords;
    }
    private List<Tile> GetUnderlayingTiles(Coordinate coord, UnitFormation unitFormation) {
      return this.GetUnderlayingCoords(coord, unitFormation)
        .Select(c => this.Map.CoordinateToTileDict[c]).ToList();
    }

    public bool CanPlaceUnitFormation(int ownerId, Coordinate coord, UnitFormation unitFormation) {
      Tile tile = this.Map.CoordinateToTileDict[coord];

      Func<Tile, bool> IsAdjacentToNeutralOrAlly = (Tile tile) => this.Map
        .GetAdjacentTiles(tile)
        .Any(t => t != null && t.Unit != null && (t.Unit.OwnerId == ownerId || t.Unit.OwnerId == 2));

      List<Tile> underlayingTiles = this.GetUnderlayingTiles(coord, unitFormation);

      return underlayingTiles.All(tile => tile.Placeable)
        && underlayingTiles.Any(tile => IsAdjacentToNeutralOrAlly(tile));
    }

    public List<Coordinate> GetAllPossiblePlacements(int ownerId) {
      // WIP
      List<Coordinate> possibleCoordinates = new List<Coordinate>();
      Func<Unit, bool> unitIsAllyOrNeutral = (Unit unit) => unit.OwnerId == ownerId || unit.OwnerId == 2;
      foreach (Unit unit in this.Map.Units.Where(unitIsAllyOrNeutral)) {
        Func<Tile, bool> tileIsPlaceable = (Tile tile) => tile.Placeable;
        foreach (Coordinate c in this.Map
          .GetAdjacentTiles(this.Map.CoordinateToTileDict[unit.Coordinate])
          .Where(tileIsPlaceable)
          .Select(tile => tile.Coordinate)) {
          Debug.Log(c);
        }
      }
      return possibleCoordinates;
    }



    // public void SetActivePlayer(int turn) {
    //   this.ActivePlayerId = GameSettings.TurnOrder[turn];
    //   Debug.Log($"{this.ActivePlayerId} is the current Player");
    // }
    public void EndTurn() {
      this.Turn++;
    }
    private void ApplyRules(Player player) {
      List<RuleEvaluation> roundEvaluation = new List<RuleEvaluation>();
      foreach (Rule rule in GameSettings.Rules) {
        roundEvaluation.Add(rule.Evaluate(player, this.Map));
      }
      player.AddTurnEvaluation(roundEvaluation);
    }
    public Player DetermineRuleWinner(RuleNames ruleName) {
      return this.Players.Aggregate((Func<Player, Player, Player>)((player1, player2) => {
        Rule rule = this.GameSettings.RuleNameToRuleDict[ruleName];
        int pointsPlayer1 = rule.Evaluate(player1, (Map)this.Map).Points;
        int pointsPlayer2 = rule.Evaluate(player2, (Map)this.Map).Points;
        return pointsPlayer1 == pointsPlayer2 ? null : pointsPlayer1 > pointsPlayer2 ? player1 : player2;
      }));
    }
  }

}
