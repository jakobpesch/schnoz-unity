using System;
using System.Collections.Generic;
using System.Linq;
using Schnoz;
using UnityEngine;

namespace Schnoz {
  // public class PlayerState {
  //   public bool active;
  //   public bool placing;
  //   public bool inMenu;
  // }
  [Serializable]
  public class Player {
    [SerializeField] private string playerName;
    public string PlayerName {
      get => this.playerName;
    }
    [SerializeField] private int id;
    public int Id {
      get => this.id;
    }
    [SerializeField] private int score = 0;
    public int Score {
      get => this.score;
    }
    public List<List<RuleEvaluation>> EvaluationHistory { get; private set; }

    [SerializeField] private int singlePieces;
    public int SinglePieces {
      get => this.singlePieces;
    }
    [SerializeField] private int stones;
    public int Stones {
      get => this.stones;
    }
    [SerializeField] private string soundName;
    public string SoundName {
      get => this.soundName;
    }
    [SerializeField] private Card selectedCard;
    public Card SelectedCard {
      get => this.selectedCard;
    }

    // public PlayerInput Input;

    [SerializeField] private bool active;
    public bool Active {
      get => this.active;
    }
    public bool Placing {
      get => this.selectedCard != null;
    }
    // [SerializeField] private PlayerState state;

    [SerializeField] private List<Rule> rules = new List<Rule>();
    public List<Rule> Rules {
      get => this.rules;
    }

    public Player(int id) {
      this.id = id;
      this.playerName = "Player " + id;
      this.rules = new List<Rule>();
    }
    public Player(int id, int singlePieces, string soundName = "") {
      this.id = id;
      this.singlePieces = singlePieces;
      this.soundName = soundName;
      this.rules = new List<Rule>();
    }
    public void TakeCard(Card card) {
      this.selectedCard = card;
    }
    public void DiscardCard() {
      if (this.selectedCard == null) {
        // Debug.Log("Nothing to discard.");
        return;
      }
      // Debug.Log($"Player {this.id} discards card: {this.selectedCard.id}");
      this.selectedCard = null;
    }
    public void AddTurnEvaluation(List<RuleEvaluation> turnEvaluation) {
      EvaluationHistory.Add(turnEvaluation);
      foreach (RuleEvaluation eval in turnEvaluation) {
        score += eval.Points;
      }
    }
    public bool IsHoldingCard() {
      return this.selectedCard != null;
    }
    // public void ClickCard(Card card)
    // {
    //   EventManager.I.ClickCard(this, new ClickCardEventArgs(card));
    // }
    // public void ClickTile(Tile tile)
    // {
    //   EventManager.I.ClickTile(this, new ClickTileEventArgs(tile));
    // }

    public void SetActive(bool value) {
      this.active = value;
    }
    public void SetSinglePieces(int singlePieces) {
      this.singlePieces = singlePieces;
    }

    //   [System.Serializable]
    //   public class PlayerInput
    //   {
    //     public Vector3 mousePositionWorldPoint;
    //     public Vector3 mousePositionScreenPoint;
    //     public bool mouseDown;
    //     public Vector3? mouseDownPositionWorldPoint
    //     {
    //       get => mouseDown ? mouseDownPositionWorldPoint : null;
    //     }
    //     public bool mouseUp;
    //     public bool mouseDrag;
    //     public float scroll;
    //     public GameObject hoveredTile;
    //     public bool RotateLeft;
    //     public bool RotateRight;
    //     public bool FlipHorizontal;
    //     public bool FlipVertical;
    //     public bool Cancel;
    //     private float x0, y0, x1, y1;
    //     public Vector3 lastMousePositionScreenPoint;
    //     public Vector3 panStart;
    //     public Card SelectedCard = null;
    //     public bool ClickedStone = false;
    //     public bool ClickedSingle = false;
    //     public bool[] SelectCardButtons = new bool[3] { false, false, false };
    //     public string SelectedAction;
    //     public bool didMouseMove
    //     {
    //       get => lastMousePositionScreenPoint != mousePositionScreenPoint; // WP or SP doesn't matter here
    //     }
    //     // public GameObject HoveredTile()
    //     // {
    //     //   if (!didMouseMove)
    //     //     return hoveredTile;

    //     //   foreach (Tile tile in Map.I.Tiles)
    //     //   {
    //     //     x0 = tile.transform.position.x - Map.I.HalfTileSize;
    //     //     y0 = tile.transform.position.y - Map.I.HalfTileSize;
    //     //     x1 = tile.transform.position.x + Map.I.HalfTileSize;
    //     //     y1 = tile.transform.position.y + Map.I.HalfTileSize;

    //     //     if (mousePositionWorldPoint.x > x0 && mousePositionWorldPoint.x < x1 &&
    //     //         mousePositionWorldPoint.y > y0 && mousePositionWorldPoint.y < y1)
    //     //       return tile;
    //     //   }
    //     //   return null;
    //     // }

    //     public void ResetInput()
    //     {
    //       H.ClearArray(SelectCardButtons);
    //       SelectedCard = null;
    //       SelectedAction = null;
    //       ClickedStone = false;
    //       ClickedSingle = false;
    //       mouseDown = false;
    //       mouseDrag = false;
    //       mouseUp = false;
    //     }
    //   }


  }
}
