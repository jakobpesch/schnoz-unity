using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Schnoz;
using UnityEngine;

namespace Schnoz
{
  [Serializable]
  public class Deck
  {
    private List<Card> cards = new List<Card>();
    public List<Card> Cards
    { get => cards; }

    public Deck(int size = Defaults.DeckSize)
    {
      int i = 0;
      var cardTypes = Enum.GetValues(typeof(CardType)).Cast<CardType>().ToList();
      while (true)
      {
        foreach (CardType cardType in cardTypes)
        {
          cards.Add(new Card(cardType));
          if (++i == size)
          {
            return;
          }
        }
      }

    }
    /// <summary>
    /// Draws cards. Fires an Event for each drawn card.
    /// </summary>
    public void Draw(int numberOfCards = 1)
    {
      for (int i = 0; i < numberOfCards; i++)
      {
        if (cards.Count == 0)
        {
          return;
        }
        Card drawnCard = cards[0];
        cards.RemoveAt(0);
        Schnoz.I.eventManager.DrawCard(this, drawnCard);
      }
    }
    public void Shuffle()
    {
      cards.OrderBy(x => UnityEngine.Random.value).ToList();
    }
  }
}
