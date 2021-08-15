using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Schnoz
{
  [Serializable]
  public class Schnoz
  {
    [SerializeField] private Map map;
    [SerializeField] private Deck deck;
    [SerializeField] private List<Player> players;
    [SerializeField] private GameSettings settings;
    [SerializeField] private int turn;
    [SerializeField] private int stage;
    public List<Player> Players { get => settings.Players; }
    [SerializeField] private Player activePlayer;
    public Player ActivePlayer { get => Players.Find(player => player.Active); }
    public Player NeutralPlayer;
    private void Start()
    {
      // Debug.Log("GameManager listens to: OnStartGame");
      EventManager.I.OnStartGame += this.StartGame;
      EventManager.I.OnClickCard += this.OnClickCardHandler;
      EventManager.I.OnAbort += this.OnAbortHandler;
    }
    private void StartGame(object sender, GameSettings Settings)
    {
      // if (sender.GetType() != typeof(Menu)) { return; } TODO:SenderCheck

      this.settings = Settings;

      // Debug.Log("GameManager listens to: OnEndTurn");
      EventManager.I.OnEndTurn += this.EndTurn;

      // Debug.Log("GameManager listens to: OnAllPlayersPresent");
      EventManager.I.OnAllPlayersPresent += this.Init;

      // Debug.Log("GameManager listens to: OnSpaceButton");
      EventManager.I.OnSpaceButton += this.SpaceButton;

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
      //   EventManager.I.OnAllPlayersPresent();
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
        turnEvaluation.Add(rule.Evaluate(player));
      }
      player.AddTurnEvaluation(turnEvaluation);
    }
    public void Init()
    {
      // Debug.Log("GameManager unlistens to: OnAllPlayersPresent");
      EventManager.I.OnAllPlayersPresent -= this.Init;

      this.map.Deck = new Deck();
      this.map.Deck.Shuffle();

      this.map.CurrentCards = new List<Card>();

      this.turn = 0;

      this.map.ClearTiles();

      foreach (Player player in this.settings.Players)
      {
        player.SetSinglePieces(this.settings.NumberOfSinglePieces);
      }

      // this.NeutralPlayer = new Player(3, Color.white, this.capital.GetComponent<SpriteRenderer>().sprite, 0);
      // SpawnManager.I.SpawnUnit(this.capital, this.map.CenterTile, this.NeutralPlayer);

      this.map.Scan();

      this.map.Deck.Draw(this.settings.NumberOfCardsPerTurn);

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

      Player player = this.settings.TurnOrder[turn];
      player.SetActive(true);
      EventManager.I.StartTurn(this, player);
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
      foreach (Player player in this.settings.Players)
      {
        this.ApplyRules(player);
      }
    }
    private void ApplyRules(Player player)
    {
      List<RuleEvaluation> roundEvaluation = new List<RuleEvaluation>();
      foreach (Rule rule in this.settings.Rules)
      {
        roundEvaluation.Add(rule.Evaluate(player));
      }
      player.AddTurnEvaluation(roundEvaluation);
    }
  }
}
