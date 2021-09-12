using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Schnoz;

public class CardView : MonoBehaviour
{
  public StandardGame game;
  public Card card;
  private void Start()
  {
    this.gameObject.AddComponent<BoxCollider2D>();
  }
  private void OnMouseUp()
  {
    this.game.HandlePlayerInput(InputEventNames.OnMouseUp, this.card);
  }
}
