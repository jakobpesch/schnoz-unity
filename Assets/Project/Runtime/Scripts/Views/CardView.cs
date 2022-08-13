using System;
using System.Collections.Generic;
using UnityEngine;
using Schnoz;
using UnityEngine.UI;

public class CardView : MonoBehaviour {
  public StandardGameClient GameClient;
  private Guid cardId;
  public Guid CardId {
    get => this.cardId;
    set {
      if (this.cardId != value) {
        this.cardId = value;
        CardType type = this.GameClient.OpenCardsDict[CardId].Type;
        string fileName = type.ToString();
        Sprite sprite = Resources.Load<Sprite>($"Sprites/{fileName}");
        this.Image.sprite = sprite;
      }
    }
  }
  public Image Image;
  public KeyCode keyBind;
  public bool IsSelected {
    get => this.GameClient.SelectedCardId == this.CardId;
  }

  private void Awake() {
    this.Image = this.GetComponent<Image>();
  }

  public void SelectCard() {
    this.GameClient.HandlePlayerInput(this, InputEventNames.SelectCard, this.CardId);
  }
  private void Update() {
    if (Input.GetKeyDown(keyBind)) {
      this.SelectCard();
    }
  }
}
