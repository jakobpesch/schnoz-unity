using System.Collections.Generic;
using Schnoz;
using Unity.Networking.Transport;
using TypeAliases;

public class NetEndTurn : NetMessage {
  public NetEndTurn() {
    this.Code = OpCode.END_TURN;
  }
  public NetEndTurn(DataStreamReader reader) {
    this.Code = OpCode.END_TURN;
    this.Deserialize(reader);
  }
  public override void Serialize(ref DataStreamWriter writer) {
    writer.WriteByte((byte)this.Code);

  }
  public override void Deserialize(DataStreamReader reader) { }
  public override void ReceivedOnClient() {
    NetUtility.C_END_TURN?.Invoke(this);
  }
  public override void ReceivedOnServer(NetworkConnection cnn) {
    NetUtility.S_END_TURN?.Invoke(this, cnn);
  }
}
