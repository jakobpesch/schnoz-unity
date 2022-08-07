using System.Collections.Generic;
using Schnoz;
using Unity.Networking.Transport;
using TypeAliases;

public class NetGameOver : NetMessage {
  public PlayerIds winnerId;
  public NetGameOver() {
    this.Code = OpCode.GAME_OVER;
  }
  public NetGameOver(DataStreamReader reader) {
    this.Code = OpCode.GAME_OVER;
    this.Deserialize(reader);
  }
  public override void Serialize(ref DataStreamWriter writer) {
    writer.WriteByte((byte)this.Code);

    writer.WriteByte((byte)this.winnerId);
  }
  public override void Deserialize(DataStreamReader reader) {
    this.winnerId = (PlayerIds)reader.ReadByte();
  }
  public override void ReceivedOnClient() {
    NetUtility.C_GAME_OVER?.Invoke(this);
  }
  public override void ReceivedOnServer(NetworkConnection cnn) {
    NetUtility.S_GAME_OVER?.Invoke(this, cnn);
  }
}
