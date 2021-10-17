using System;
using UnityEngine;
using Schnoz;

public class SinglePieceView : MonoBehaviour {
  public StandardGameClient game;
  public Guid singlePieceId;

  private void Start() {
    this.singlePieceId = Guid.NewGuid();
  }
  private void OnMouseUp() {
    this.game.HandlePlayerInput(this, InputEventNames.OnMouseUp, this.singlePieceId);
  }
}
