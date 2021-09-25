using Unity.Networking.Transport;
using Unity.Collections;
using Schnoz;
public class NetDrawCard : NetMessage
{
  public byte cardType;

  public NetDrawCard()
  {
    this.Code = OpCode.DRAW_CARD;
  }
  public NetDrawCard(DataStreamReader reader)
  {
    this.Code = OpCode.DRAW_CARD;
    this.Deserialize(reader);
  }

  public override void Serialize(ref DataStreamWriter writer)
  {
    writer.WriteByte((byte)this.Code);
    writer.WriteByte(cardType);

  }

  public override void Deserialize(DataStreamReader reader)
  {
    this.cardType = reader.ReadByte();

  }

  public override void ReceivedOnClient()
  {
    NetUtility.C_DRAW_CARD?.Invoke(this);
  }
  public override void ReceivedOnServer(NetworkConnection cnn)
  {
    NetUtility.S_DRAW_CARD?.Invoke(this, cnn);
  }
}
