using UnityEngine;
using Unity.Networking.Transport;
using Schnoz;

public class NetWelcome : NetMessage {
  public PlayerIds AssignedPlayerId { get; set; }
  public PlayerRoles AssignedRole { get; set; }
  public NetWelcome() {
    this.Code = OpCode.WELCOME;
  }
  public NetWelcome(DataStreamReader reader) {
    this.Code = OpCode.WELCOME;
    this.Deserialize(reader);
  }
  public override void Serialize(ref DataStreamWriter writer) {
    writer.WriteByte((byte)this.Code);
    writer.WriteByte((byte)AssignedPlayerId);
    writer.WriteByte((byte)AssignedRole);
  }
  public override void Deserialize(DataStreamReader reader) {
    AssignedPlayerId = (PlayerIds)reader.ReadByte();
    AssignedRole = (PlayerRoles)reader.ReadByte();
  }
  public override void ReceivedOnClient() {
    NetUtility.C_WELCOME?.Invoke(this);
  }
  public override void ReceivedOnServer(NetworkConnection cnn) {
    NetUtility.S_WELCOME?.Invoke(this, cnn);
  }
}
