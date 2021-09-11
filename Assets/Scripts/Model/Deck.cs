using System;
using System.Linq;
using System.Collections.Generic;
using Utils;
using UnityEngine;

namespace Schnoz
{
  [Serializable]
  public class Deck : Observable
  {
    [SerializeField] private List<Card> cards;
    public List<Card> Cards
    {
      get => this.cards;
      set
      {
        this.cards = value;
        this.NotifyPropertyChanged();
      }
    }
    public Deck(int size = Defaults.DeckSize)
    {
      this.cards = new List<Card>();
      int i = 0;
      List<CardType> cardTypes = Enum.GetValues(typeof(CardType)).Cast<CardType>().ToList();
      while (true)
      {
        foreach (CardType cardType in cardTypes)
        {
          this.cards.Add(new Card(cardType));
          if (++i == size)
          {
            return;
          }
        }
      }
    }

    public Card Draw()
    {
      Debug.Log("Drawing Card");
      if (this.Cards.Count == 0)
      {
        return null;
      }

      Card drawnCard = this.Cards[0];
      this.Cards.RemoveAt(0);
      this.NotifyPropertyChanged("Deck.Cards");
      return drawnCard;
    }

    public void Shuffle()
    {
      Debug.Log("Shuffling Deck");
      this.cards = this.cards.OrderBy(x => UnityEngine.Random.value).ToList();
      this.NotifyPropertyChanged("Deck.Cards");
    }
  }
}
