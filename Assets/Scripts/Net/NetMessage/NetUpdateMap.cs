using UnityEngine;
using Unity.Networking.Transport;
using Unity.Collections;

public class NetUpdateMap : NetMessage {
  public FixedString4096 netMapString;
  public NetUpdateMap() {
    this.Code = OpCode.UPDATE_MAP;
  }
  public NetUpdateMap(DataStreamReader reader) {
    this.Code = OpCode.UPDATE_MAP;
    this.Deserialize(reader);
  }
  public override void Serialize(ref DataStreamWriter writer) {
    writer.WriteByte((byte)this.Code);
    writer.WriteFixedString4096(this.netMapString);
  }
  public override void Deserialize(DataStreamReader reader) {
    this.netMapString = reader.ReadFixedString4096();
  }
  public override void ReceivedOnClient() {
    NetUtility.C_UPDATE_MAP?.Invoke(this);
  }
  public override void ReceivedOnServer(NetworkConnection cnn) {
    NetUtility.S_UPDATE_MAP?.Invoke(this, cnn);
  }
}
