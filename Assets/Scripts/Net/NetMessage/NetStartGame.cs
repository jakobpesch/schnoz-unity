using UnityEngine;
using Unity.Networking.Transport;
using Unity.Collections;

public class NetStartGame : NetMessage
{
  public FixedString4096 netMapString;
  public NetStartGame()
  {
    this.Code = OpCode.START_GAME;
  }
  public NetStartGame(DataStreamReader reader)
  {
    this.Code = OpCode.START_GAME;
    this.Deserialize(reader);
  }
  public override void Serialize(ref DataStreamWriter writer)
  {
    writer.WriteByte((byte)this.Code);
    writer.WriteFixedString4096(this.netMapString);
  }
  public override void Deserialize(DataStreamReader reader)
  {
    this.netMapString = reader.ReadFixedString4096();
  }
  public override void ReceivedOnClient()
  {
    NetUtility.C_START_GAME?.Invoke(this);
  }
  public override void ReceivedOnServer(NetworkConnection cnn)
  {
    NetUtility.S_START_GAME?.Invoke(this, cnn);
  }
}
