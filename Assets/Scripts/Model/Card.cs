using System;
using System.Collections.Generic;
using System.Linq;

namespace Schnoz
{
  public class Card
  {
    private Guid id;
    public Guid Id
    {
      get => this.id;
    }
    public CardType type;
    public CardType Type { get => this.type; }

    public Card(CardType type)
    {
      this.id = Guid.NewGuid();
      this.type = type;
    }
  }
}
