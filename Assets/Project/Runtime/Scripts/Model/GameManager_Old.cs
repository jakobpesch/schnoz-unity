// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Linq;
// using Random = System.Random;
// using Schnoz;using UnityEngine;

// namespace Schnoz
// {
//   public class GameManager
//   {
//     private GameSettings settings;
//     private int turn;
//     private int stage;
//     public List<Player> Players { get => settings.Players; }
//     private Player activePlayer;
//     public Player ActivePlayer { get => Players.Find(player => player.active); }
//     public Player NeutralPlayer;
//     private Deck deck;
//     private void Start()
//     {
//       // Debug.Log("GameManager listens to: OnStartGame");
//       EventManager.I.OnStartGame += this.StartGame;
//       EventManager.I.OnClickCard += this.OnClickCardHandler;
//       EventManager.I.OnAbort += this.OnAbortHandler;
//     }
//     private void StartGame(object sender, GameSettings Settings)
//     {
//       // if (sender.GetType() != typeof(Menu)) { return; } TODO:SenderCheck

//       this.settings = Settings;

//       // Debug.Log("GameManager listens to: OnEndTurn");
//       EventManager.I.OnEndTurn += this.EndTurn;

//       // Debug.Log("GameManager listens to: OnAllPlayersPresent");
//       EventManager.I.OnAllPlayersPresent += this.Init;

//       // Debug.Log("GameManager listens to: OnSpaceButton");
//       EventManager.I.OnSpaceButton += this.SpaceButton;

//       StartCoroutine(WaitForPlayers());

//       IEnumerator WaitForPlayers()
//       {
//         // Debug.Log("Waiting for players");
//         int nPresentPlayers = 0;
//         List<GameObject> presentPlayers;
//         while (nPresentPlayers < 2)
//         {
//           yield return new WaitForSeconds(0.1f);
//           presentPlayers = GameObject.FindGameObjectsWithTag("Player").ToList();
//           nPresentPlayers = presentPlayers.Count();
//           // Debug.Log("Number of present players: " + nPresentPlayers);
//         }
//         EventManager.I.OnAllPlayersPresent();
//       }
//     }
//     private void SpaceButton(object sender, Player player)
//     {
//       if (player == this.activePlayer)
//       {
//         // Debug.Log("test");
//       }
//     }
//     private void OnClickCardHandler(object sender, ClickCardEventArgs e)
//     {
//       // here the GM should check what to do when the player clicks a card

//       if (sender.GetType() != typeof(Player))
//       {
//         // Debug.Log("OnClickCardHandler only takes Player objects");
//         return;
//       }
//       Player player = (Player)sender;

//       if (player.active)
//       {
//         if (player.IsHoldingCard())
//         {
//           player.DiscardCard();
//         }
//         player.TakeCard(e.Card);
//       }
//     }
//     private void OnAbortHandler(object sender, Player player)
//     // here the GM should check what to do when the player clicks a card
//     {
//       if (player.active)
//       {
//         if (player.placing)
//         {
//           player.DiscardCard();
//         }
//       }
//     }

//     private void EvaluateRules(Player player, List<Rule> rules)
//     {
//       List<RuleEvaluation> turnEvaluation = new List<RuleEvaluation>();
//       foreach (Rule rule in rules)
//       {
//         turnEvaluation.Add(rule.Evaluate(player));
//       }
//       player.AddTurnEvaluation(turnEvaluation);
//     }
//     public void Init()
//     {
//       // Debug.Log("GameManager unlistens to: OnAllPlayersPresent");
//       EventManager.I.OnAllPlayersPresent -= this.Init;

//       Map.I.Deck = new Deck();
//       Map.I.Deck.Shuffle();

//       Map.I.OpenCards = new List<Card>();

//       this.turn = 0;

//       Map.I.ClearTiles();

//       foreach (Player player in this.settings.Players)
//       {
//         player.singlePieces = this.settings.NumberOfSinglePieces;
//       }

//       this.NeutralPlayer = new Player(3, Color.white, this.capital.GetComponent<SpriteRenderer>().sprite, 0);
//       // SpawnManager.I.SpawnUnit(this.capital, Map.I.CenterTile, this.NeutralPlayer);

//       M.I.Map_Scan();

//       Map.I.Deck.Draw(this.settings.NumberOfCardsPerTurn);

//       this.SetActivePlayer(0);

//       Map.I.GameStarted = true;
//       // UIManager.I.Init();

//       // M.I.UI_UpdateScore();

//       // M.I.UI_PopulateProgressBar();

//       // M.I.UI_UpdateSinglePieces();
//       // M.I.UI_UpdateStones();

//       // TakeStone();
//       // M.I.Map_Scan();
//       // Map.I.GameStarted = true;
//     }
//     private void SetActivePlayer(int turn)
//     {

//       Player player = this.settings.TurnOrder[turn];
//       player.active = true;
//       EventManager.I.StartTurn(this, player);
//     }
//     private void EndTurn(object sender, Player player)
//     {
//       if (sender.GetType() != typeof(Player))
//       {
//         // Debug.Log($"Only senders of type 'Player' can end the turn. Sender was: '{sender.GetType()}'"); return;
//       }
//       this.SetActivePlayer(++this.turn);
//       this.UpdateRules();
//     }
//     private void UpdateRules()
//     {
//       foreach (Player player in this.settings.Players)
//       {
//         this.ApplyRules(player);
//       }
//     }
//     private void ApplyRules(Player player)
//     {
//       List<RuleEvaluation> roundEvaluation = new List<RuleEvaluation>();
//       foreach (Rule rule in this.settings.Rules)
//       {
//         roundEvaluation.Add(rule.Evaluate(player));
//       }
//       player.AddTurnEvaluation(roundEvaluation);
//     }
//     // // Settings
//     // private bool discardRemainingCards = true;
//     // private int numberOfSinglePieces;
//     // private int numberOfCardsPerTurn = 3;
//     // private float timeLeft = 30;
//     // public float safeArea = 3;
//     // private int numberOfStages;
//     // public int DeckSize = 20;
//     // private bool isGameOver = false;
//     // public List<Player> Players { get => M.I.Map_Players(); }

//     // private Player currentPlayer;
//     // public Player CurrentPlayer
//     // {
//     //   get
//     //   {
//     //     currentPlayer = Players[stages[iStage][iPieces]];
//     //     return currentPlayer;
//     //   }
//     // }
//     // public int CurrentPlayerId;
//     // private int currentPlayerId { get => CurrentPlayer.id; }
//     // public Player TemporaryCurrentPlayer;
//     // public Player NeutralPlayer;
//     // public List<List<int>> stages;
//     // public List<int> stage;
//     // public int iStage;
//     // public int iPieces;
//     // private GameObject hoveredTile;
//     // private TileProperties hoveredTileProperties;
//     // private UnitFormationProperties unitFormationProperties;
//     // private GameObject selectedCard;
//     // private GameObject hoveringUnitFormation;
//     // public string phase = "build-phase";
//     // private int currentBuildPhaseTurn = 0;
//     // private GameObject capital;
//     // private bool snapFormation;
//     // public int SelectedCardIndex;
//     // public GameObject[] openCards;
//     // private Vector3[] openCardsPositions;

//     // public List<GameObject> PlacementPreventingTiles;
//     // public bool startNewGame = true;

//     // private GameObject[] presentPlayers;
//     // private int nPresentPlayers;

//     // public void DeselectCard()
//     // {
//     //   Destroy(hoveringUnitFormation);
//     //   for (int i = 0; i < openCards.Length; i++)
//     //     if (openCards[i] != null)
//     //       openCards[i].SetActive(true);
//     //   M.I.Spawn_selectedCard = null;
//     //   Cursor.visible = true;

//     //   hoveringUnitFormation = null;
//     //   unitFormationProperties = null;
//     // }

//     // void EndTurn(object sender, Player player)
//     // {
//     //   if (player == this.player)
//     //   {
//     //     EventManager.I.OnSelect += SelectCard;
//     //   }
//     // }

//     // private void Start() { StartCoroutine(Game()); }
//     // private IEnumerator Game()
//     // {

//     //   EventManager.I.StartTurn(this, currentPlayer);
//     //   EventManager.I.OnEndTurn += EndTurn;

//     //   yield return StartCoroutine(WaitForPlayers());

//     //   if (NetworkingManager.Singleton.IsHost || Map.I.LocalGame)
//     //   {
//     //     Init();
//     //    // Debug.Log("Initialised map");
//     //   }

//     //   if (NetworkingManager.Singleton.IsClient) // Not tested - for multiplayer
//     //    // Debug.Log("Waiting for server to initialise map.");

//     //   while (!M.I.Map_GameReady())
//     //     yield return null;

//     //   if (NetworkingManager.Singleton.IsClient)
//     //    // Debug.Log("Server initialised map.");

//     //   //############## GAME PLAY LOOP ##############
//     //   yield return StartCoroutine(GamePlayLoop());


//     //   //############## GAME OVER ##############
//     //   IEnumerator WaitForPlayers()
//     //   {
//     //    // Debug.Log("Waiting for players");
//     //     while (nPresentPlayers < 2)
//     //     {
//     //       yield return new WaitForSeconds(0.1f);
//     //       presentPlayers = GameObject.FindGameObjectsWithTag("Player");
//     //       nPresentPlayers = presentPlayers.Count();
//     //      // Debug.Log("Number of present players: " + nPresentPlayers);
//     //     }
//     //   }

//     //   IEnumerator GamePlayLoop()
//     //   {
//     //    // Debug.Log(RuleCollection.NullRule);
//     //     bool actionAborted = false;
//     //     // The Game will stay in this loop until the game is over
//     //     while (true)
//     //     {
//     //       // Array.ForEach(openCards, card => card.SetActive(true));

//     //       //############## Stays in this coroutine until the current player places a card.
//     //       yield return StartCoroutine(Turn());

//     //       // If the player chooses an action (card, stone ...) and then cancels it,
//     //       // then the coroutine Turn() starts over
//     //       if (actionAborted)
//     //       {
//     //         // Array.ForEach(openCards, card => card.SetActive(true));
//     //         actionAborted = false;
//     //         continue;
//     //       }
//     //       UIManager.I.UpdateCurrentPlayerCharacter();

//     //       M.I.Map_Scan();
//     //      // Debug.Log("Map scanned");
//     //     }

//     //     /// <summary>
//     //     /// Starts the turn of the current player. Here the game first waits for the player
//     //     /// to choose an action(card, stone...) and then waits for the player to
//     //     /// execute that action.
//     //     /// Breaks out of the coroutine either when:
//     //     /// -the action was executed fully or
//     //     /// - the action was aborted(in this case 'actionAborted' is set to true)
//     //     ///</summary>
//     //     IEnumerator Turn()
//     //     {

//     //       while (true)
//     //       {
//     //         switch (CurrentPlayer.Input.SelectedAction)
//     //         {
//     //           case "card":
//     //             // Get index of chosen card
//     //             int cardPositionIndex = openCards.Length - Array.IndexOf(CurrentPlayer.Input.SelectCardButtons, true) - 1;

//     //             // Do nothing when a card slot is selected where the card is not there
//     //             if (openCards[cardPositionIndex] == null)
//     //             {
//     //               CurrentPlayer.Input.ResetInput(); yield return null; continue;
//     //             }

//     //             // Select card
//     //             M.I.Spawn_SelectCard(openCards[cardPositionIndex].GetComponent<CardProperties>());
//     //             hoveringUnitFormation = M.I.Spawn_CreateUnitFormationFrom(M.I.Spawn_selectedCard);
//     //             unitFormationProperties = hoveringUnitFormation.GetComponent<UnitFormationProperties>();
//     //             Cursor.visible = false;
//     //             // Array.ForEach(openCards, card => card.SetActive(false));

//     //             Array.ForEach(openCards, c =>
//     //             {
//     //               CardProperties cp = c.GetComponent<CardProperties>();
//     //               if (cp.id != SpawnManager.I.selectedCardProperties.id)
//     //                 H.SetOpacity(c, 0.5f);
//     //               foreach (Transform child in c.transform)
//     //                 foreach (Transform grandchild in child)
//     //                   H.SetOpacity(grandchild.gameObject, 0.5f);
//     //             });

//     //             // Prevent further inputs to be processed this frame
//     //             CurrentPlayer.Input.ResetInput();

//     //             // Place card
//     //             yield return StartCoroutine(PlacingCard());


//     //             Array.ForEach(openCards, c =>
//     //             {
//     //               H.SetOpacity(c, 1f);
//     //               foreach (Transform child in c.transform)
//     //                 foreach (Transform grandchild in child)
//     //                   H.SetOpacity(grandchild.gameObject, 1f);
//     //             });


//     //             //// Debug.Log("Coroutine placing cards finished");

//     //             // Break out of switch statement
//     //             break; /* End case "card"*/


//     //           case "single":
//     //             //// Debug.Log("case single");

//     //             // No single pieces available
//     //             if (CurrentPlayer.singlePieces < 1)
//     //             {
//     //               CurrentPlayer.Input.ResetInput();
//     //               M.I.Audio_Play("meepmop");
//     //               yield return null;
//     //               continue;
//     //             }

//     //             // Spawn single piece
//     //             M.I.Spawn_SelectCard(M.I.Card_SinglePiece().GetComponent<CardProperties>());
//     //             hoveringUnitFormation = M.I.Spawn_CreateUnitFormationFrom(M.I.Spawn_selectedCard);
//     //             unitFormationProperties = hoveringUnitFormation.GetComponent<UnitFormationProperties>();
//     //             Cursor.visible = false;

//     //             // Start placing
//     //             M.I.UI_HighlightPossiblePlacementTiles(CurrentPlayer);

//     //             yield return StartCoroutine(PlacingSingle());

//     //             // Decrease single pieces if not aborted
//     //             if (!actionAborted)
//     //             {
//     //               CurrentPlayer.singlePieces--;
//     //               M.I.UI_UpdateSinglePieces();
//     //             }

//     //             // Prevent further inputs to be processed this frame
//     //             CurrentPlayer.Input.ResetInput();

//     //             // Break out of switch statement
//     //             break; /* End case "single"*/


//     //           case "stone":
//     //            // Debug.Log("case stone");

//     //             // No stones available
//     //             if (CurrentPlayer.stones < 1)
//     //             {
//     //               CurrentPlayer.Input.ResetInput();
//     //               M.I.Audio_Play("meepmop");
//     //               yield return null;
//     //               continue;
//     //             }

//     //             // Spawn stone
//     //             GameObject hoveringStone = M.I.Spawn_CreateTerrain(Map.I.terrainStone);
//     //             hoveringStone.GetComponent<SpriteRenderer>().sortingOrder = 10;
//     //             hoveringStone.GetComponent<SpriteRenderer>().sortingLayerName = "UI";
//     //             Cursor.visible = false;

//     //             // Start placing stone
//     //             yield return StartCoroutine(PlacingStone(hoveringStone));

//     //             // Decrease single pieces if not aborted
//     //             if (!actionAborted)
//     //             {
//     //               CurrentPlayer.stones--;
//     //               M.I.UI_UpdateStones();
//     //             }

//     //             // Prevent further inputs to be processed this frame
//     //             CurrentPlayer.Input.ResetInput();

//     //             // Break out of switch statement
//     //             break; /* End case "stone"*/


//     //           case null:
//     //             //// Debug.Log("no action selected");
//     //             yield return null; // Stay in while loop
//     //             continue; /* End case null*/

//     //           default:
//     //            // Debug.Log("Unexpected behaviour");
//     //             yield return null; // Stay in while loop
//     //             continue; /* End case default*/
//     //         }
//     //         break; // End coroutine
//     //       }

//     //       IEnumerator PlacingCard()
//     //       {
//     //         while (true)
//     //         {
//     //           // Cancel placing cards
//     //           bool cardSelectButtonPressed = Array.Find(CurrentPlayer.Input.SelectCardButtons, b => b == true);
//     //           if (CurrentPlayer.Input.Cancel || cardSelectButtonPressed)
//     //           {
//     //             DeselectCard();
//     //             actionAborted = true;
//     //             M.I.UI_ResetHighlights();
//     //             //// Debug.Log("Card placing cancelled");
//     //             break;
//     //           }

//     //           // Highlights
//     //           if (CurrentPlayer.Input.didMouseMove)
//     //           {
//     //             M.I.UI_ResetHighlights();
//     //             // M.I.UI_HighlightArea();
//     //             M.I.UI_HighlightPossiblePlacementTiles(CurrentPlayer);
//     //           }

//     //           // Follow the mouse
//     //           if (snapFormation) hoveringUnitFormation.transform.position = CurrentPlayer.Input.HoveredTile().transform.position;
//     //           else hoveringUnitFormation.transform.position = CurrentPlayer.Input.mousePositionWorldPoint;

//     //           // Get hovered tile properties
//     //           if (CurrentPlayer.Input.hoveredTile == null)
//     //           { yield return null; continue; }

//     //           hoveredTile = CurrentPlayer.Input.hoveredTile;
//     //           hoveredTileProperties = hoveredTile;

//     //           if (CurrentPlayer.Input.RotateLeft) unitFormationProperties.Rotate(direction: "right");
//     //           if (CurrentPlayer.Input.RotateRight) unitFormationProperties.Rotate(direction: "left");
//     //           if (CurrentPlayer.Input.FlipHorizontal) unitFormationProperties.Flip(direction: "vertical");
//     //           if (CurrentPlayer.Input.FlipVertical) unitFormationProperties.Flip(direction: "horizontal");

//     //           if (CurrentPlayer.Input.mouseUp) // TODO: ResetInputs() on Player class
//     //             if (CanBePlaced())
//     //             {
//     //               yield return StartCoroutine(PlaceCard(hoveringUnitFormation));
//     //               CurrentPlayer.Input.ResetInput();
//     //               //// Debug.Log("Card placed");
//     //               break;
//     //             }

//     //           yield return null;
//     //         }
//     //       }

//     //       IEnumerator PlacingSingle()
//     //       {
//     //         while (true)
//     //         {

//     //           //// Debug.Log("In placing single");
//     //           // Cancel placing cards
//     //           bool cardSelectButtonPressed = Array.Find(CurrentPlayer.Input.SelectCardButtons, b => b == true);
//     //           if (CurrentPlayer.Input.Cancel || cardSelectButtonPressed)
//     //           {
//     //             DeselectCard();
//     //             actionAborted = true;
//     //             M.I.UI_ResetHighlights();
//     //             //// Debug.Log("Single placing cancelled");
//     //             break;
//     //           }

//     //           // Highlights
//     //           if (CurrentPlayer.Input.didMouseMove)
//     //           {
//     //             M.I.UI_ResetHighlights();
//     //             // M.I.UI_HighlightArea();
//     //             M.I.UI_HighlightPossiblePlacementTiles(CurrentPlayer);
//     //           }

//     //           // Follow the mouse
//     //           if (snapFormation) hoveringUnitFormation.transform.position = CurrentPlayer.Input.HoveredTile().transform.position;
//     //           else hoveringUnitFormation.transform.position = CurrentPlayer.Input.mousePositionWorldPoint;

//     //           // Get hovered tile properties
//     //           if (CurrentPlayer.Input.hoveredTile == null)
//     //           { yield return null; continue; }

//     //           hoveredTile = CurrentPlayer.Input.hoveredTile;
//     //           hoveredTileProperties = hoveredTile;

//     //           if (CurrentPlayer.Input.RotateLeft) unitFormationProperties.Rotate(direction: "right");
//     //           if (CurrentPlayer.Input.RotateRight) unitFormationProperties.Rotate(direction: "left");
//     //           if (CurrentPlayer.Input.FlipHorizontal) unitFormationProperties.Flip(direction: "vertical");
//     //           if (CurrentPlayer.Input.FlipVertical) unitFormationProperties.Flip(direction: "horizontal");

//     //           if (CurrentPlayer.Input.mouseUp) // TODO: ResetInputs() on Player class
//     //             if (CanBePlaced())
//     //             {
//     //               yield return StartCoroutine(PlaceSingle(hoveringUnitFormation));
//     //               CurrentPlayer.Input.ResetInput();
//     //               //// Debug.Log("Card placed");
//     //               break;
//     //             }

//     //           yield return null;
//     //         }
//     //       }

//     //       IEnumerator PlacingStone(GameObject hoveringStone)
//     //       {
//     //         while (true)
//     //         {
//     //           if (timeLeft < 0)
//     //            // Debug.Log("Player ran out of time");
//     //           // ADAPT!!!!
//     //           // Cancel placing cards 
//     //           bool cardSelectButtonPressed = Array.Find(CurrentPlayer.Input.SelectCardButtons, b => b == true);
//     //           if (CurrentPlayer.Input.Cancel || cardSelectButtonPressed)
//     //           {
//     //             DeselectCard();
//     //             Destroy(hoveringStone);
//     //             actionAborted = true;
//     //             M.I.UI_ResetHighlights();
//     //             //// Debug.Log("Stone placing cancelled");
//     //             break;
//     //           }

//     //           // Follow the mouse
//     //           if (snapFormation) hoveringStone.transform.position = CurrentPlayer.Input.HoveredTile().transform.position;
//     //           else hoveringStone.transform.position = CurrentPlayer.Input.mousePositionWorldPoint;

//     //           // Get hovered tile properties
//     //           if (CurrentPlayer.Input.hoveredTile == null)
//     //           { yield return null; continue; }

//     //           hoveredTile = CurrentPlayer.Input.hoveredTile;
//     //           hoveredTileProperties = hoveredTile;

//     //           if (CurrentPlayer.Input.RotateLeft) unitFormationProperties.Rotate(direction: "right");
//     //           if (CurrentPlayer.Input.RotateRight) unitFormationProperties.Rotate(direction: "left");
//     //           if (CurrentPlayer.Input.FlipHorizontal) unitFormationProperties.Flip(direction: "vertical");
//     //           if (CurrentPlayer.Input.FlipVertical) unitFormationProperties.Flip(direction: "horizontal");

//     //           if (CurrentPlayer.Input.mouseUp) // TODO: ResetInputs() on Player class
//     //             if (T.Visible(CurrentPlayer.Input.hoveredTile)
//     //             && T.Empty(CurrentPlayer.Input.hoveredTile)
//     //             && T.Placeable(CurrentPlayer.Input.hoveredTile)
//     //             && !T.AdjacentTiles(CurrentPlayer.Input.hoveredTile).Any(at => T.IsStone(at)))
//     //             {
//     //               yield return StartCoroutine(PlaceTerrain(hoveringStone));
//     //               CurrentPlayer.Input.ResetInput();
//     //               break;
//     //             }

//     //           yield return null;
//     //         }
//     //       }

//     //     }

//     //     // yield return StartCoroutine(WaitForOtherPlayersMoves()); // NOT IMPLEMENTED
//     //     // IEnumerator WaitForOtherPlayersMoves() // NOT IMPLEMENTED
//     //     // {
//     //     //   // if (CurrentPlayer == LocalPlayer) 
//     //     //   while (false)
//     //     //     yield return null;
//     //     // }
//     //   }

//     // }
//     // public void Init()
//     // {
//     //   foreach (var player in presentPlayers)
//     //     M.I.Map_Players().Add(player.GetComponent<PlayerProperties>().player);

//     //   Map.I.Deck = new Deck();
//     //   Map.I.Deck.Shuffle();



//     //   iStage = 0;
//     //   iPieces = 0;

//     //   // Create turn order
//     //   stage = new List<int>() { 0, 1, 1, 0, 0, 1 };
//     //   List<int> reverseStage = new List<int>(stage);
//     //   reverseStage.Reverse();

//     //   stages = new List<List<int>>();
//     //   for (int i = 0; i < numberOfStages; i++)
//     //     if (i % 2 == 0)
//     //       stages.Add(stage);
//     //     else
//     //       stages.Add(reverseStage);

//     //   /*Create turn order*/

//     //   foreach (GameObject tile in M.I.Map_Tiles())
//     //   {
//     //     if (tile == M.I.Map_CenterTile()) continue;
//     //     T.Properties(tile).Visible = false;
//     //     GameObject unit = T.Properties(tile).Unit;
//     //     if (unit != null) Destroy(unit);
//     //     T.Properties(tile).Unit = null;
//     //     T.Properties(tile).UnitProperties = null;
//     //   }
//     //   foreach (Player player in M.I.Map_Players())
//     //   {
//     //     player.singlePieces = numberOfSinglePieces;
//     //     // player.rules = new List<Rule>(); TODO: Evaluation History
//     //   }

//     //   NeutralPlayer = new Player(3, Color.white, capital.GetComponent<SpriteRenderer>().sprite, 0);
//     //   GameObject spawnedUnit = M.I.Spawn_Unit(capital, M.I.Map_CenterTile(), NeutralPlayer);
//     //   M.I.Map_Scan();

//     //   openCards = new GameObject[numberOfCardsPerTurn];
//     //   openCardsPositions = new Vector3[numberOfCardsPerTurn];

//     //   foreach (Transform cardSlot in GameObject.Find("Current Cards").transform)
//     //     foreach (Transform card in cardSlot)
//     //     {
//     //       Destroy(card.gameObject);
//     //     }
//     //   // Draw and position initial cards
//     //   for (int i = 0; i < numberOfCardsPerTurn; i++)
//     //   {
//     //     // openCards[i] = M.I.Map_Deck().Draw();
//     //     // openCards[i].transform.position += new Vector3(0, 3 * i, 0);
//     //     // openCardsPositions[i] = openCards[i].transform.localPosition;
//     //   }
//     //   for (int i = 0; i < numberOfCardsPerTurn; i++)
//     //   {
//     //     Map.I.OpenCards.Add(M.I.Map_Deck().Draw());
//     //   }
//     //   // // InitializeRules();
//     //   //// Debug.Assert(true, UIManager.I);

//     //   UIManager.I.Init();

//     //   M.I.UI_UpdateScore();

//     //   M.I.UI_PopulateProgressBar();

//     //   M.I.UI_UpdateSinglePieces();
//     //   M.I.UI_UpdateStones();

//     //   TakeStone();
//     //   M.I.Map_Scan();
//     //   Map.I.GameStarted = true;
//     // }

//     // private void LateUpdate()
//     // {
//     //   timeLeft -= Time.deltaTime;
//     //   return;

//     //   // Game flow





//     //   // selectedCard = M.I.Spawn_selectedCard;

//     //   // bool isCardSelected = selectedCard != null;
//     //   // bool selectCardButtonPressed = Array.Find(CurrentPlayer.Input.SelectCardButtons, b => b == true);
//     //   // bool isCardNewlySelected = isCardSelected && hoveringUnitFormation == null;

//     //   // int cardPositionIndex = openCards.Length - Array.IndexOf(CurrentPlayer.Input.SelectCardButtons, true) - 1;


//     //   // if (selectCardButtonPressed && !isCardSelected)
//     //   // {
//     //   //   if (openCards[cardPositionIndex] != null)
//     //   //   {
//     //   //     DeselectCard();
//     //   //     M.I.Spawn_SelectCard(openCards[cardPositionIndex].GetComponent<CardProperties>());
//     //   //   }
//     //   //   return;
//     //   // }

//     //   // if (selectCardButtonPressed && isCardSelected)
//     //   // {
//     //   //   if (openCards[cardPositionIndex] == null)
//     //   //     return;

//     //   //   DeselectCard();
//     //   //   M.I.Spawn_SelectCard(openCards[cardPositionIndex].GetComponent<CardProperties>());
//     //   //   return;
//     //   // }

//     //   // if (isCardNewlySelected)
//     //   // {
//     //   //   hoveringUnitFormation = M.I.Spawn_CreateUnitFormationFrom(selectedCard);
//     //   //   unitFormationProperties = hoveringUnitFormation.GetComponent<UnitFormationProperties>();
//     //   //   Cursor.visible = false;
//     //   //   return;
//     //   // }

//     //   // if (isCardSelected)
//     //   // {
//     //   //   M.I.UI_HighlightPossiblePlacementTiles(CurrentPlayer);

//     //   //   if (snapFormation) hoveringUnitFormation.transform.position = CurrentPlayer.Input.HoveredTile().transform.position;
//     //   //   else hoveringUnitFormation.transform.position = CurrentPlayer.Input.mousePositionWorldPoint;

//     //   //   if (CurrentPlayer.Input.RotateLeft) unitFormationProperties.Rotate(direction: "right");
//     //   //   if (CurrentPlayer.Input.RotateRight) unitFormationProperties.Rotate(direction: "left");
//     //   //   if (CurrentPlayer.Input.FlipHorizontal) unitFormationProperties.Flip(direction: "vertical");
//     //   //   if (CurrentPlayer.Input.FlipVertical) unitFormationProperties.Flip(direction: "horizontal");

//     //   //   if (CurrentPlayer.Input.Cancel)
//     //   //   {
//     //   //     DeselectCard();
//     //   //     return;
//     //   //   }

//     //   //   if (CurrentPlayer.Input.mouseUp && CanBePlaced())
//     //   //     PlaceCard(hoveringUnitFormation);
//     //   // }



//     // }
//     // private void EvaluateStage(int iStage)
//     // {
//     //   M.I.UI_UpdateRuleValues();

//     //   M.I.Audio_PlayDelayed("evaluation");
//     //   for (int i = 0; i < M.I.Map_Players()[0].rules.Count(); i++)
//     //   {
//     //     int p2Score = 0;//M.I.Map_Players()[1].rules[i].Points;
//     //     int p1Score = 0;//M.I.Map_Players()[0].rules[i].Points;

//     //     if (p1Score == p2Score) continue;

//     //     if (p1Score > p2Score)
//     //       M.I.Map_Players()[0].score++;
//     //     else
//     //       M.I.Map_Players()[1].score++;
//     //   }
//     //  // Debug.Log($"Bob {M.I.Map_Players()[0].score} : {M.I.Map_Players()[1].score} Ulf");

//     //   phase = "build-phase";

//     //   M.I.UI_UpdateScore();

//     //   // SwapPlayer();
//     //   return;
//     // }
//     // private IEnumerator PlaceCard(GameObject unitFormation)
//     // {

//     //   // Spawn and destroy unit formation
//     //   GameObject unitPrefab = unitFormation.GetComponent<UnitFormationProperties>().unitPrefab;
//     //   foreach (GameObject tile in GetHoveredOverTiles())
//     //     M.I.Spawn_Unit(unitPrefab, tile, CurrentPlayer);
//     //   Destroy(unitFormation);

//     //   // Play sound
//     //   M.I.Audio_Play(CurrentPlayer.soundName);

//     //   // Destroy placed card
//     //   // for (int i = 0; i < openCards.Length; i++)
//     //   //   if (openCards[i] != null && openCards[i].GetComponent<CardProperties>().id == SelectedCardIndex)
//     //   //     Destroy(openCards[i]);

//     //   yield return StartCoroutine(FinishUpAction("card"));
//     // }
//     // private IEnumerator PlaceTerrain(GameObject terrain)
//     // {
//     //   terrain.GetComponent<SpriteRenderer>().sortingOrder = 3;
//     //   terrain.GetComponent<SpriteRenderer>().sortingLayerName = "Default";
//     //   M.I.Spawn_Terrain(terrain, CurrentPlayer.Input.hoveredTile);
//     //   Destroy(terrain);
//     //   yield return StartCoroutine(FinishUpAction("terrain"));
//     // }
//     // private IEnumerator PlaceSingle(GameObject unitFormation)
//     // {

//     //   // Spawn and destroy unit formation
//     //   GameObject unitPrefab = unitFormation.GetComponent<UnitFormationProperties>().unitPrefab;
//     //   foreach (GameObject tile in GetHoveredOverTiles())
//     //     M.I.Spawn_Unit(unitPrefab, tile, CurrentPlayer);
//     //   Destroy(unitFormation);

//     //   // Play sound
//     //   M.I.Audio_Play(CurrentPlayer.soundName);

//     //   // Destroy placed card
//     //   // for (int i = 0; i < openCards.Length; i++)
//     //   //   if (openCards[i] != null && openCards[i].GetComponent<CardProperties>().id == SelectedCardIndex)
//     //   //     Destroy(openCards[i]);

//     //   yield return StartCoroutine(FinishUpAction("single"));
//     // }

//     // private IEnumerator FinishUpAction(string action)
//     // {

//     //   hoveringUnitFormation = null;
//     //   unitFormationProperties = null;
//     //   M.I.Spawn_selectedCard = null;

//     //   TakeStone();
//     //   UpdateRules();

//     //   M.I.Map_Scan();
//     //   M.I.UI_UpdateRuleValues();


//     //   if (iPieces != stages[iStage].Count() && action == "card")
//     //   {
//     //     M.I.UI_UpdateTurn();
//     //   }

//     //   if (action == "card")
//     //     iPieces++;

//     //   if (iPieces == stages[iStage].Count())
//     //   {
//     //     M.I.UI_UpdateTurn();
//     //     iPieces = 0;
//     //     EvaluateStage(iStage);
//     //     foreach (Player player in M.I.Map_Players())
//     //     {
//     //       player.singlePieces++;
//     //       M.I.UI_UpdateSinglePieces();
//     //     }
//     //     iStage++;
//     //     if (iStage == stages.Count())
//     //     {
//     //       isGameOver = true;
//     //       startNewGame = false;
//     //       iStage = 0;
//     //       M.I.UI_UpdateSinglePieces();
//     //       M.I.UI_UpdateStones();
//     //       M.I.UI_ShowGameOver();
//     //       Cursor.visible = true;
//     //     }
//     //   }

//     //   if (!isGameOver)
//     //   {
//     //     M.I.UI_UpdateSinglePieces();
//     //     M.I.UI_UpdateStones();
//     //     M.I.UI_ResetHighlights();

//     //     if (iPieces % 2 == 0 && action == "card")
//     //       yield return StartCoroutine(RedrawCards());

//     //     Cursor.visible = true;
//     //   }
//     // }
//     // private bool CanBePlaced()
//     // {
//     //   PlacementPreventingTiles = new List<GameObject>();

//     //   // Get underlaying tiles
//     //   List<GameObject> underlayingTiles = GetHoveredOverTiles();

//     //   if (underlayingTiles == null) {// Debug.Log("UnderlayingTiles is null");// Debug.Break(); return false; }

//     //   // Define booleans
//     //   bool anyAdjacentUnitAllied = underlayingTiles.Any(T.AdjacentUnitAllied);
//     //   bool placingSingleUnit = underlayingTiles.Count() == 1; //BUG: When placed at border out of bounds tiles are not registered

//     //   foreach (GameObject tile in T.PreventingPlacement(underlayingTiles))
//     //     PlacementPreventingTiles.Add(tile);

//     //   if (PlacementPreventingTiles.Count() > 0 || (!anyAdjacentUnitAllied && !placingSingleUnit))
//     //   {
//     //     M.I.UI_HighlightPlacementPreventingTiles();
//     //     return false;
//     //   }

//     //   return true;
//     // }
//     // private void UpdateRules()
//     // {
//     //   foreach (Player player in M.I.Map_Players())
//     //   {
//     //     ApplyRules(player);
//     //   }
//     //   // foreach (var player in M.I.Map_Players())
//     //   // {
//     //   //   TemporaryCurrentPlayer = player;
//     //   //   // player.rules = new List<(string ruleName, int points, object validTiles)>();
//     //   //   TerrainRule("water", "max", player);
//     //   //   TerrainRule("stone", "min", player);
//     //   //   GeometryRule("diagonal", "max", player);
//     //   //   GeometryRule("holes", "max", player);
//     //   // }
//     // }

//     // private void ApplyRules(Player player)
//     // {
//     //   List<Rule> rules = new List<Rule>() { RuleCollection.NullRule };
//     //   List<RuleEvaluation> roundEvaluation = new List<RuleEvaluation>();
//     //   foreach (Rule rule in rules)
//     //   {
//     //     roundEvaluation.Add(rule.Evaluate(player));
//     //   }
//     //   player.AddEvaluation(roundEvaluation);
//     // }
//     // private List<GameObject> GetHoveredOverTiles()
//     // {
//     //   if (unitFormationProperties.arrangement == null) {// Debug.Log("unitFormationProperties.arrangement is null."); return null; }
//     //   List<GameObject> underlayingTiles = new List<GameObject>();
//     //   foreach ((int row, int col) in unitFormationProperties.arrangement)
//     //   {
//     //     GameObject ut = M.I.Map_Tiles().Find(tile =>
//     //       (tile.Coordinate.row - row == hoveredTileProperties?.Coordinate.row &&
//     //        tile.Coordinate.col - col == hoveredTileProperties?.Coordinate.col)
//     //     );
//     //     if (ut != null) underlayingTiles.Add(ut);
//     //   }
//     //   return underlayingTiles;
//     // }

//     // public void TakeStone()
//     // {
//     //   TemporaryCurrentPlayer = CurrentPlayer;
//     //   List<GameObject> tilesWithStone = new List<GameObject>();

//     //   foreach (GameObject tileWithStone in M.I.Map_Tiles().Where(T.IsStone))
//     //     tilesWithStone.Add(tileWithStone);

//     //   foreach (GameObject potentialTile in tilesWithStone.Where(T.AdjacentUnitPlayer))
//     //   {
//     //     List<GameObject> adjacentTiles = T.AdjacentTiles(potentialTile);
//     //     Func<GameObject, bool> allyOrTerrainAndNotStone = H.Or(T.UnitIsAllied, H.And(H.Not(T.Placeable), H.Not(T.IsStone)));
//     //     if (adjacentTiles.All(allyOrTerrainAndNotStone))
//     //     {
//     //       CurrentPlayer.stones++;
//     //       Destroy(potentialTile.Terrain);
//     //       potentialTile.Terrain = null;
//     //     }
//     //   }
//     // }

//     // public void SelectStone()
//     // {
//     //   CurrentPlayer.Input.ClickedStone = true;
//     // }


//     // // private void AddRuleToPlayer(Player player, string ruleId, object validTiles)
//     // // {
//     // //   var ruleIndex = player.rules.FindIndex(r => r.ruleName == ruleId);

//     // //   if (ruleIndex == -1)
//     // //     player.rules.Add(new PlayerRule(ruleId, validTiles));
//     // //   else
//     // //     player.rules[ruleIndex].validTiles = validTiles;
//     // // }

//     // // private void TerrainRule(string terrain, string minmax, Player player)
//     // // {
//     // //   string ruleName = terrain + minmax;

//     // //   int points = 0;

//     // //   List<GameObject> terrainTiles = M.I.Map_Tiles().Where(t =>
//     // //   {
//     // //     GameObject ter = t.Terrain;
//     // //     if (ter != null)
//     // //       return ter.GetComponent<TerrainProperties>().id == terrain;
//     // //     else return false;
//     // //   }).ToList();

//     // //   List<GameObject> validTerrainTiles = new List<GameObject>();
//     // //   foreach (GameObject terrainTile in terrainTiles)
//     // //   {
//     // //     bool playerIsAdjacentToTerrainTile =
//     // //       terrainTile.AdjacentTiles.Any(at =>
//     // //         at.UnitProperties?.Owner == player);

//     // //     if (playerIsAdjacentToTerrainTile)
//     // //       validTerrainTiles.Add(terrainTile);
//     // //   }

//     // //   points = validTerrainTiles.Count() * (minmax == "min" ? -1 : 1);

//     // //   AddRuleToPlayer(player, ruleName, validTerrainTiles);
//     // // }

//     // // private void GeometryRule(string geometry, string minmax, Player player)
//     // // {
//     // //   string ruleName = geometry + minmax;

//     // //   int points = 0;

//     // //   if (geometry == "diagonal")
//     // //   {
//     // //     List<List<GameObject>> unitDiagonals = new List<List<GameObject>>();
//     // //     foreach (List<GameObject> diagonal in M.I.Map_Diagonals())
//     // //     {
//     // //       List<GameObject> unitDiagonal = new List<GameObject>();
//     // //       foreach (GameObject tile in diagonal)
//     // //         if (tile.UnitProperties?.Owner != player)
//     // //         {
//     // //           if (unitDiagonal.Count() >= 3)
//     // //             unitDiagonals.Add(unitDiagonal);
//     // //           unitDiagonal = new List<GameObject>();
//     // //         }
//     // //         else
//     // //           unitDiagonal.Add(tile);
//     // //     }
//     // //     List<List<GameObject>> validUnitDiagonals = new List<List<GameObject>>();
//     // //     foreach (List<GameObject> unitDiagonal in unitDiagonals.Where(ud => ud.Count() >= 3))
//     // //       validUnitDiagonals.Add(unitDiagonal);

//     // //     points = validUnitDiagonals.Count() * (minmax == "min" ? -1 : 1);

//     // //     AddRuleToPlayer(player, ruleName, validUnitDiagonals);

//     // //   }

//     // //   if (geometry == "holes")
//     // //   {
//     // //     List<GameObject> holes = new List<GameObject>();
//     // //     Func<GameObject, bool> emptyAndPlaceableAndAtLeasteOnePlayer = H.And(T.Empty, H.And(T.Placeable, T.AdjacentUnitPlayer));
//     // //     foreach (GameObject emptyPlaceableTile in M.I.Map_Tiles().Where(emptyAndPlaceableAndAtLeasteOnePlayer))
//     // //     {
//     // //       List<GameObject> adjacentTiles = T.AdjacentTiles(emptyPlaceableTile);
//     // //       Func<GameObject, bool> allyOrTerrain = H.Or(T.UnitIsAllied, H.Not(T.Placeable));
//     // //       if (adjacentTiles.All(allyOrTerrain)) holes.Add(emptyPlaceableTile);
//     // //     }

//     // //     points = holes.Count() * (minmax == "min" ? -1 : 1);

//     // //     AddRuleToPlayer(player, ruleName, holes);
//     // //   }
//     // // }


//     // private IEnumerator RedrawCards()
//     // {
//     //   foreach (GameObject card in openCards)
//     //   {
//     //     StartCoroutine(DiscardAnimation(card));
//     //     yield return new WaitForSeconds(0.2f);
//     //   }
//     //   M.I.Map_NewDeck();
//     //   M.I.Map_Deck().Shuffle();

//     //   for (int iC = 0; iC < openCards.Length; iC++)
//     //   {
//     //     StartCoroutine(DrawAnimation(iC));
//     //     yield return new WaitForSeconds(0.2f);
//     //   }
//     // }


//     // private IEnumerator DiscardAnimation(GameObject card)
//     // {
//     //   yield return StartCoroutine(H.MoveTo(card.transform, card.transform.localPosition - new Vector3(150f, 0, 0), 300f));
//     //   Destroy(card);
//     // }
//     // private IEnumerator DrawAnimation(int indexCard)
//     // {
//     //   // openCards[indexCard] = M.I.Card_DrawCard();
//     //   // openCards[indexCard].transform.localPosition = openCardsPositions[indexCard] - new Vector3(150f, 0, 0);
//     //   // openCards[indexCard].transform.localPosition += Vector3.left * 5;
//     //   yield return StartCoroutine(H.MoveTo(openCards[indexCard].transform, openCards[indexCard].transform.localPosition + new Vector3(150f, 0, 0), 300f));
//     // }
//   }
// }
