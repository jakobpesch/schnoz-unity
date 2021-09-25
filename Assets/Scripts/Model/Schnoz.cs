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
    public List<Player> Players { get => this.gameSettings.Players; }
    [SerializeField] private Player activePlayer;
    public Player ActivePlayer { get => this.Players.Find(player => player.Active); }
    public Player NeutralPlayer;
    public void Start()
    {
      // this.eventManager = new EventManager();
      // Debug.Log("GameManager listens to: OnStartGame");
      // this.eventManager.OnStartGame += this.StartGame;
      // this.eventManager.OnClickCard += this.OnClickCardHandler;
      // this.eventManager.OnAbort += this.OnAbortHandler;

      // this.eventManager.StartGame();
    }
    private void StartGame()
    {
      // Debug.Log("Starting Game");
      // Debug.Log(this.Map);
      // this.Map = new Map(this.gameSettings.NRows, this.gameSettings.NCols);
      // Debug.Log(this.Map);
      // Debug.Log(this.Map.Tiles);

      // if (sender.GetType() != typeof(Menu)) { return; } TODO:SenderCheck

      // Debug.Log("GameManager listens to: OnEndTurn");
      // this.eventManager.OnEndTurn += this.EndTurn;

      // // Debug.Log("GameManager listens to: OnAllPlayersPresent");
      // this.eventManager.OnAllPlayersPresent += this.Init;

      // // Debug.Log("GameManager listens to: OnSpaceButton");
      // this.eventManager.OnSpaceButton += this.SpaceButton;

      // this.eventManager.AllPlayersPresent();
      // StartCoroutine(WaitForPlayers());

      // IEnumerator WaitForPlayers()
      // {
      //   // Debug.Log("Waiting for players");
      //   int nPresentPlayers = 0;
      //   List<GameObject> presentPlayers;
      //   while (nPresentPlayers < 2)
      //   {
      //     yield return new WaitForSeconds(0.1f);
      //     presentPlayers = GameObject.FindGameObjectsWithTag("Player").ToList();
      //     nPresentPlayers = presentPlayers.Count;
      //     // Debug.Log("Number of present players: " + nPresentPlayers);
      //   }
      //   this.em.OnAllPlayersPresent();
      // }
    }
    private void SpaceButton(object sender, Player player)
    {
      if (player == this.activePlayer)
      {
        // Debug.Log("test");
      }
    }
    private void OnClickCardHandler(object sender, ClickCardEventArgs e)
    {
      // here the GM should check what to do when the player clicks a card

      if (sender.GetType() != typeof(Player))
      {
        // Debug.Log("OnClickCardHandler only takes Player objects");
        return;
      }
      Player player = (Player)sender;

      if (player.Active)
      {
        if (player.IsHoldingCard())
        {
          player.DiscardCard();
        }
        player.TakeCard(e.Card);
      }
    }
    private void OnAbortHandler(object sender, Player player)
    // here the GM should check what to do when the player clicks a card
    {
      if (player.Active)
      {
        if (player.Placing)
        {
          player.DiscardCard();
        }
      }
    }

    private void EvaluateRules(Player player, List<Rule> rules)
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
      Map newMap = new Map(this.gameSettings.NRows, this.gameSettings.NCols);
      bool mapWasNull = this.Map == null;
      this.Map = newMap;
      this.NotifyPropertyChanged("Map");
    }

    public void CreateDeck()
    {
      Debug.Log("Deck will be Created");
      this.Deck = new Deck(this.gameSettings.DeckSize);
      this.NotifyPropertyChanged("Deck");
    }

    public void ShuffleDeck()
    {
      Debug.Log("Deck will be Shuffled");
      this.deck.Shuffle();
      this.NotifyPropertyChanged("Deck");
    }

    public void DrawCards()
    {
      for (int i = 0; i < gameSettings.NumberOfCardsPerTurn; i++)
      {
        this.Deck.Draw();
      }
    }

    public void PlaceUnit(Coordinate coord)
    {
      Debug.Log("Placing Unit");
      Tile tile = this.map.Tiles.Find(tile => tile.Coordinate == coord);
      Unit unit = new Unit(this.gameSettings.Players[0].Id, coord);
      tile.SetUnit(unit);
      // this.NotifyPropertyChanged("Map");
    }
    public void RemoveUnit(Coordinate coord)
    {
      Debug.Log("Removing Unit");
      Tile tile = this.map.Tiles.Find(tile => tile.Coordinate == coord);
      tile.SetUnit(null);
      this.NotifyPropertyChanged("Map");
    }
    public void PlaceUnitFormation(Coordinate coord, UnitFormation unitFormation)
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
        this.PlaceUnit(newCoord);
      }


      // Debug.Log("Placing Unit");
      // Unit unit = new Unit(this.gameSettings.Players[0], "Peter", 2);
      // Tile tile = this.map.Tiles.Find(tile => tile.Coordinate == pos);
      // tile.Unit = unit;
      this.NotifyPropertyChanged("Map");
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

      // this.NeutralPlayer = new Player(3, Color.white, this.capital.GetComponent<SpriteRenderer>().sprite, 0);
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
      Player player = gameSettings.TurnOrder[turn];
      player.SetActive(true);
      // this.eventManager.StartTurn(this, player);
      Debug.Log($"{player.Id} is the current Player");
    }
    private void EndTurn(object sender, Player player)
    {
      if (sender.GetType() != typeof(Player))
      {
        // Debug.Log($"Only senders of type 'Player' can end the turn. Sender was: '{sender.GetType()}'"); return;
      }
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
