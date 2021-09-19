using System;
using System.Collections.Generic;
using UnityEngine;
using Schnoz;
using TypeAliases;

public class TileView : MonoBehaviour
{
  public StandardGame game;
  public Guid tileId;
  private void Start()
  {
    this.gameObject.AddComponent<BoxCollider2D>();
  }
  private void OnMouseUp()
  {
    this.game.HandlePlayerInput(this, InputEventNames.OnMouseUp, this.tileId);
  }

  private void OnMouseEnter()
  {
    this.game.HandlePlayerInput(this, InputEventNames.OnMouseEnter, this.tileId);
  }

  private void OnMouseExit()
  {
    this.game.HandlePlayerInput(this, InputEventNames.OnMouseExit, this.tileId);
  }
}
