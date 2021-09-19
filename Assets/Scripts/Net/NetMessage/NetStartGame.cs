using UnityEngine;
using Unity.Networking.Transport;

public class NetStartGame : NetMessage
{
  public int AssinedTeam { get; set; }
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
    writer.WriteInt(AssinedTeam);
  }
  public override void Deserialize(DataStreamReader reader)
  {
    AssinedTeam = reader.ReadInt();
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
