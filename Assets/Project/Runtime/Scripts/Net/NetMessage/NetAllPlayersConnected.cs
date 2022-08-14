using UnityEngine;
using Unity.Networking.Transport;
using Schnoz;

public class NetAllPlayersConnected : NetMessage {
  public NetAllPlayersConnected() {
    this.Code = OpCode.ALL_PLAYERS_CONNECTED;
  }
  public NetAllPlayersConnected(DataStreamReader reader) {
    this.Code = OpCode.ALL_PLAYERS_CONNECTED;
    this.Deserialize(reader);
  }
  public override void Serialize(ref DataStreamWriter writer) {
    writer.WriteByte((byte)this.Code);
  }
  public override void Deserialize(DataStreamReader reader) {
  }
  public override void ReceivedOnClient() {
    NetUtility.C_ALL_PLAYERS_CONNECTED?.Invoke(this);
  }
  public override void ReceivedOnServer(NetworkConnection cnn) {
    NetUtility.S_ALL_PLAYERS_CONNECTED?.Invoke(this, cnn);
  }
}
