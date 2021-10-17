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
    public string PlayerName { get; private set; }
    public PlayerIds Id { get; private set; }
    public int Score { get; private set; }
    public List<List<RuleEvaluation>> EvaluationHistory { get; private set; }
    public int SinglePieces { get; private set; }
    public int Stones { get; private set; }
    public string SoundName { get; private set; }

    public Player(PlayerIds id) {
      this.Id = id;
      this.PlayerName = "Player " + id;
    }
    public Player(PlayerIds id, int singlePieces, string soundName = "") {
      this.Id = id;
      this.SinglePieces = singlePieces;
      this.SoundName = soundName;
    }
    public void AddTurnEvaluation(List<RuleEvaluation> turnEvaluation) {
      EvaluationHistory.Add(turnEvaluation);
      foreach (RuleEvaluation eval in turnEvaluation) {
        Score += eval.Points;
      }
    }

    public void SetSinglePieces(int singlePieces) {
      this.SinglePieces = singlePieces;
    }

    public void SetScore(int score) {
      this.Score = score;
    }

  }
}
