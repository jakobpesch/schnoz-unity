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
    public UnitFormation unitFormation;
    public Card(CardType type)
    {
      this.id = Guid.NewGuid();
      this.type = type;
      this.unitFormation = new UnitFormation(type);
    }
  }
}
