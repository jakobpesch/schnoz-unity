using System.Collections.Generic;
using Unity.Networking.Transport;
using Schnoz;
using TypeAliases;
using UnityEngine;

public class NetStartGame : NetMessage {

  public NetStartGame() {
    this.Code = OpCode.START_GAME;
  }
  public NetStartGame(DataStreamReader reader) {
    this.Code = OpCode.START_GAME;
    this.Deserialize(reader);
  }
  public override void Serialize(ref DataStreamWriter writer) {
    writer.WriteByte((byte)this.Code);
  }
  public override void Deserialize(DataStreamReader reader) { }
  public override void ReceivedOnClient() {
    NetUtility.C_START_GAME?.Invoke(this);
  }
  public override void ReceivedOnServer(NetworkConnection cnn) {
    NetUtility.S_START_GAME?.Invoke(this, cnn);
  }
}
