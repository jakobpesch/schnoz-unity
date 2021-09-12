using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Schnoz;
using TypeAliases;

public class TileView : MonoBehaviour
{
  public StandardGame game;
  public Coordinate coordinate;
  private void Start()
  {
    this.gameObject.AddComponent<BoxCollider2D>();
  }
  private void OnMouseUp()
  {
    this.game.HandlePlayerInput(InputEventNames.OnMouseUp, this.coordinate);
  }

  private void OnMouseEnter()
  {
    this.game.HandlePlayerInput(InputEventNames.OnMouseEnter, this.coordinate);
  }

  private void OnMouseExit()
  {
    this.game.HandlePlayerInput(InputEventNames.OnMouseExit, this.coordinate);
  }
}
