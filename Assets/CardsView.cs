using System;
using System.Collections.Generic;
using UnityEngine;
using Schnoz;

public class CardsView : MonoBehaviour {
  public CardView Prefab;
  public List<CardView> CardViews = new List<CardView>();
  private StandardGameClient gameClient;
  public StandardGameClient GameClient {
    get => this.gameClient;
    set {
      this.gameClient = value;
      Debug.Log($"set GameClient to {value}");
    }
  }
  public const int Alpha1 = 49; // Enum value for: KeyCode.Alpha1
  public void Render() {
    List<Card> cards = this.GameClient.GameClient.OpenCards;

    if (this.CardViews.TrueForAll(cardView => cardView.GameClient != null) && this.CardViews.Count == this.GameClient.GameClient.OpenCards.Count) {
      this.UpdateCards();
      return;
    }

    if (cards.Count > 9) {
      throw new Exception("Can't render more than 9 cards.");
    }

    for (int i = 0; i < this.CardViews.Count; i++) {
      CardView cardView = this.CardViews[i];
      Destroy(cardView.gameObject);
    }

    this.CardViews = new List<CardView>();
    for (int i = 0; i < cards.Count; i++) {
      Card card = cards[i];
      CardView cardView = GameObject.Instantiate<CardView>(this.Prefab);
      cardView.transform.SetParent(this.transform);
      RectTransform rect = cardView.GetComponent<RectTransform>();
      rect.localScale = Vector3.one;
      cardView.GameClient = this.GameClient;
      cardView.CardId = card.Id;
      cardView.keyBind = (KeyCode)(Alpha1 + i);
      cardView.Image.color = cardView.IsSelected ? Color.green : Color.white;
      this.CardViews.Add(cardView);
    }
  }

  public void UpdateCards() {
    List<Card> cards = this.GameClient.GameClient.OpenCards;
    for (int i = 0; i < cards.Count; i++) {
      Card card = cards[i];
      CardView cardView = this.CardViews[i];
      if (cardView.IsSelected) {
        Debug.Log($"Card {card.Id} is selected.");
      }
      cardView.Image.color = cardView.IsSelected ? Color.green : Color.white;
      cardView.CardId = card.Id;
    }
  }

}
