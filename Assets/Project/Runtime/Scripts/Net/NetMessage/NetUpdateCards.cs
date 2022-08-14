using Unity.Networking.Transport;
using Schnoz;
using System.Collections.Generic;
public class NetUpdateCards : NetMessage {
  public List<Card> cards;
  public NetUpdateCards() {
    this.Code = OpCode.UPDATE_CARDS;
  }

  public NetUpdateCards(DataStreamReader reader) {
    this.Code = OpCode.UPDATE_CARDS;
    this.Deserialize(reader);
  }

  public override void Serialize(ref DataStreamWriter writer) {
    writer.WriteByte((byte)this.Code);

    // Open cards
    writer.WriteByte((byte)this.cards.Count);
    for (int i = 0; i < this.cards.Count; i++) {
      writer.WriteByte((byte)this.cards[i].Type);
    }
  }

  public override void Deserialize(DataStreamReader reader) {
    // Open cards
    int cardCount = reader.ReadByte();
    this.cards = new List<Card>();
    for (int i = 0; i < cardCount; i++) {
      CardType cardType = (CardType)(int)reader.ReadByte();
      Card card = new Card(cardType);
      this.cards.Add(card);
    }
  }

  public override void ReceivedOnClient() {
    NetUtility.C_UPDATE_CARDS?.Invoke(this);
  }
  public override void ReceivedOnServer(NetworkConnection cnn) {
    NetUtility.S_UPDATE_CARDS?.Invoke(this, cnn);
  }
}
