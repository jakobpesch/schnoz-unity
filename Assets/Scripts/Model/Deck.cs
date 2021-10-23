using System;
using System.Linq;
using System.Collections.Generic;
using Utils;
using UnityEngine;

namespace Schnoz {
  [Serializable]
  public class Deck {
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
    }

    public void DrawRandomCards(int quantity) {
      var random = new System.Random();
      List<CardType> allCardTypes = Enum.GetValues(typeof(CardType)).Cast<CardType>().ToList();
      List<CardType> cardTypes = new List<CardType>();
      while (cardTypes.Count < quantity) {
        int index = random.Next(allCardTypes.Count);
        CardType cardType = allCardTypes[index];

        if (cardTypes.Contains(cardType)) {
          continue;
        }

        cardTypes.Add(cardType);
      }
      this.OpenCards = cardTypes.Select(cardType => new Card(cardType)).ToList();
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
