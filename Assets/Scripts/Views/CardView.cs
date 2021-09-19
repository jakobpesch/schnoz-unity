using System;
using System.Collections.Generic;
using UnityEngine;
using Schnoz;

public class CardView : MonoBehaviour
{
  public StandardGame game;
  public Guid cardId;
  private void Start()
  {
    this.gameObject.AddComponent<BoxCollider2D>();
  }
  private void OnMouseUp()
  {
    this.game.HandlePlayerInput(this, InputEventNames.OnMouseUp, this.cardId);
  }
}
