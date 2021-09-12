using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Schnoz;

public class TileView : MonoBehaviour
{
  public StandardGame game;
  public Tile tile;
  private void Start()
  {
    this.gameObject.AddComponent<BoxCollider2D>();
  }
  private void OnMouseUp()
  {
    this.game.HandlePlayerInput(InputEventNames.OnMouseUp, this.tile);
  }

  private void OnMouseEnter()
  {
    this.game.HandlePlayerInput(InputEventNames.OnMouseEnter, this.tile);
  }

  private void OnMouseExit()
  {
    this.game.HandlePlayerInput(InputEventNames.OnMouseExit, this.tile);
  }
}
