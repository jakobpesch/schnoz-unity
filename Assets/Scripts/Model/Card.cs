using System;
using UnityEngine;

namespace Schnoz
{
  [Serializable]
  public class Card
  {
    private Guid id;
    public Guid Id
    {
      get => this.id;
    }
    [SerializeField] private CardType type;
    public CardType Type { get => this.type; }

    public Card(CardType type)
    {
      this.id = Guid.NewGuid();
      this.type = type;
    }
  }
}
