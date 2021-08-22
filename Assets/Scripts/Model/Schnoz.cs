using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Schnoz
{
  [Serializable]
  public class Schnoz
  {
    private static readonly Schnoz instance = new Schnoz();
    static Schnoz()
    {
      Debug.Log("static Schnoz");
    }
    private Schnoz()
    {
      Debug.Log("private Schnoz");
      this.gameSettings = new GameSettings();
      this.eventManager = new EventManager();
    }
    public static Schnoz I { get => instance; }

    public Map map;
    public EventManager eventManager;
    public GameSettings gameSettings;
    [SerializeField] private Deck deck;
    [SerializeField] private List<Card> currentCards;
    [SerializeField] private List<Player> players;
    [SerializeField] private int turn;
    [SerializeField] private int stage;
    public List<Player> Players { get => gameSettings.Players; }
    [SerializeField] private Player activePlayer;
    public Player ActivePlayer { get => this.Players.Find(player => player.Active); }
    public Player NeutralPlayer;
    public void Start()
    {
      this.eventManager = new EventManager();
      Debug.Log("GameManager listens to: OnStartGame");
      this.eventManager.OnStartGame += this.StartGame;
      this.eventManager.OnClickCard += this.OnClickCardHandler;
      this.eventManager.OnAbort += this.OnAbortHandler;

      this.eventManager.StartGame();
    }
    private void StartGame()
    {
      Debug.Log("Starting Game");
      this.map = new Map();
      // if (sender.GetType() != typeof(Menu)) { return; } TODO:SenderCheck

      // Debug.Log("GameManager listens to: OnEndTurn");
      this.eventManager.OnEndTurn += this.EndTurn;

      // Debug.Log("GameManager listens to: OnAllPlayersPresent");
      this.eventManager.OnAllPlayersPresent += this.Init;

      // Debug.Log("GameManager listens to: OnSpaceButton");
      this.eventManager.OnSpaceButton += this.SpaceButton;

      this.eventManager.AllPlayersPresent();
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
        turnEvaluation.Add(rule.Evaluate(player, this.map));
      }
      player.AddTurnEvaluation(turnEvaluation);
    }
    public void Init()
    {
      // Debug.Log("GameManager unlistens to: OnAllPlayersPresent");
      this.eventManager.OnAllPlayersPresent -= this.Init;

      this.map.Generate();
      this.map.Deck = new Deck();
      this.map.Deck.Shuffle();
      this.gameSettings.SetPlayers(new List<Player>() { new Player(0), new Player(1) });
      this.gameSettings.CreateStages();

      this.currentCards = new List<Card>();

      this.turn = 0;

      this.map.ClearTiles();

      foreach (Player player in gameSettings.Players)
      {
        player.SetSinglePieces(gameSettings.NumberOfSinglePieces);
      }

      // this.NeutralPlayer = new Player(3, Color.white, this.capital.GetComponent<SpriteRenderer>().sprite, 0);
      // SpawnManager.I.SpawnUnit(this.capital, this.map.CenterTile, this.NeutralPlayer);

      this.map.Scan();

      this.map.Deck.Draw(gameSettings.NumberOfCardsPerTurn);

      this.SetActivePlayer(0);

      this.map.GameStarted = true;
      // UIManager.I.Init();

      // M.I.UI_UpdateScore();

      // M.I.UI_PopulateProgressBar();

      // M.I.UI_UpdateSinglePieces();
      // M.I.UI_UpdateStones();

      // TakeStone();
      // M.I.Map_Scan();
      // this.map.GameStarted = true;
    }
    private void SetActivePlayer(int turn)
    {

      Player player = gameSettings.TurnOrder[turn];
      player.SetActive(true);
      this.eventManager.StartTurn(this, player);
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
        roundEvaluation.Add(rule.Evaluate(player, this.map));
      }
      player.AddTurnEvaluation(roundEvaluation);
    }
  }

}
