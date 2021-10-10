using System;
using System.Collections.Generic;
using UnityEngine;
using Schnoz;
using TypeAliases;

public class TileView : MonoBehaviour {
  public StandardGameClient game;
  public StandardGameViewManager viewManager;
  public Coordinate coordinate;
  private void Start() {
    this.gameObject.AddComponent<BoxCollider2D>();
  }
  private void OnMouseUp() {
    Debug.Log("Pan: Place Card");
    if (!this.viewManager.IsPanning) {
      this.game.HandlePlayerInput(this, InputEventNames.OnMouseUp, this.coordinate);
    }
  }

  private void OnMouseEnter() {
    this.game.HandlePlayerInput(this, InputEventNames.OnMouseEnter, this.coordinate);
  }

  private void OnMouseExit() {
    this.game.HandlePlayerInput(this, InputEventNames.OnMouseExit, this.coordinate);
  }
}
