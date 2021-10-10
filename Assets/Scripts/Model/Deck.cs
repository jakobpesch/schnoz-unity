using System;
using System.Linq;
using System.Collections.Generic;
using Utils;
using UnityEngine;

namespace Schnoz {
  [Serializable]
  public class Deck : Observable {
    public List<Card> Cards { get; private set; }
    public List<Card> OpenCards { get; private set; }
    public Deck(int size = Defaults.DeckSize) {
      this.Cards = new List<Card>();
      this.OpenCards = new List<Card>();
      int i = 0;
      List<CardType> cardTypes = Enum.GetValues(typeof(CardType)).Cast<CardType>().ToList();
      while (true) {
        foreach (CardType cardType in cardTypes) {
          this.Cards.Add(new Card(cardType));
          if (++i == size) {
            return;
          }
        }
      }
    }

    public void DiscardOpenCards() {
      this.OpenCards = new List<Card>();
    }

    public void Draw() {
      Debug.Log("Drawing Card");
      if (this.Cards.Count == 0) {
        return;
      }
      Card drawnCard = this.Cards[0];
      this.Cards.RemoveAt(0);
      this.OpenCards.Add(drawnCard);
    }

    public void Shuffle() {
      Debug.Log("Shuffling Deck");
      this.Cards = this.Cards.OrderBy(x => UnityEngine.Random.value).ToList();
      this.NotifyPropertyChanged("Deck.Cards");
    }

    public string SerializeOpenCards() {
      NetOpenCards netOpenCards = new NetOpenCards();
      netOpenCards.o = this.OpenCards.Select(card => {
        NetCard netCard = new NetCard();
        netCard.t = (int)card.Type;
        return netCard;
      }).ToList();
      return JsonUtility.ToJson(netOpenCards); ;

    }
  }
}
