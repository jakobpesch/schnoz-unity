using Unity.Networking.Transport;
using Unity.Collections;
using Schnoz;
public class NetUpdateCards : NetMessage
{
  public FixedString4096 netOpenCardsString;

  public NetUpdateCards()
  {
    this.Code = OpCode.UPDATE_CARDS;
  }
  public NetUpdateCards(DataStreamReader reader)
  {
    this.Code = OpCode.UPDATE_CARDS;
    this.Deserialize(reader);
  }

  public override void Serialize(ref DataStreamWriter writer)
  {
    writer.WriteByte((byte)this.Code);
    writer.WriteFixedString4096(netOpenCardsString);

  }

  public override void Deserialize(DataStreamReader reader)
  {
    this.netOpenCardsString = reader.ReadFixedString4096();
  }

  public override void ReceivedOnClient()
  {
    NetUtility.C_UPDATE_CARDS?.Invoke(this);
  }
  public override void ReceivedOnServer(NetworkConnection cnn)
  {
    NetUtility.S_UPDATE_CARDS?.Invoke(this, cnn);
  }
}
