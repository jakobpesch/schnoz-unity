using UnityEngine;
using Unity.Networking.Transport;

public class NetWelcome : NetMessage {
  public int AssinedTeam { get; set; }
  public NetWelcome() {
    this.Code = OpCode.WELCOME;
  }
  public NetWelcome(DataStreamReader reader) {
    this.Code = OpCode.WELCOME;
    this.Deserialize(reader);
  }
  public override void Serialize(ref DataStreamWriter writer) {
    writer.WriteByte((byte)this.Code);
    writer.WriteInt(AssinedTeam);
  }
  public override void Deserialize(DataStreamReader reader) {
    AssinedTeam = reader.ReadInt();
  }
  public override void ReceivedOnClient() {
    NetUtility.C_WELCOME?.Invoke(this);
  }
  public override void ReceivedOnServer(NetworkConnection cnn) {
    NetUtility.S_WELCOME?.Invoke(this, cnn);
  }
}
