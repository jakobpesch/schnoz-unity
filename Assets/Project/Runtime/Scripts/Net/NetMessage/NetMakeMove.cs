using Schnoz;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Networking.Transport;

public class NetMakeMove : NetMessage {
  public int row;
  public int col;
  public int unitFormationId;
  public int rotation;
  public int mirrorHorizontal;
  public int mirrorVertical;
  public PlayerIds playerId;

  public NetMakeMove() {
    this.Code = OpCode.MAKE_MOVE;
  }
  public NetMakeMove(DataStreamReader reader) {
    this.Code = OpCode.MAKE_MOVE;
    this.Deserialize(reader);
  }
  public override void Serialize(ref DataStreamWriter writer) {
    writer.WriteByte((byte)this.Code);
    writer.WriteByte((byte)this.row);
    writer.WriteByte((byte)this.col);
    writer.WriteByte((byte)this.unitFormationId);
    writer.WriteByte((byte)this.rotation);
    writer.WriteByte((byte)this.mirrorHorizontal);
    writer.WriteByte((byte)this.mirrorVertical);
    writer.WriteByte((byte)this.playerId);
  }
  public override void Deserialize(DataStreamReader reader) {
    this.row = (int)reader.ReadByte();
    this.col = (int)reader.ReadByte();
    this.unitFormationId = (int)reader.ReadByte();
    this.rotation = (int)reader.ReadByte();
    this.mirrorHorizontal = (int)reader.ReadByte();
    this.mirrorVertical = (int)reader.ReadByte();
    this.playerId = (PlayerIds)reader.ReadByte();
  }
  public override void ReceivedOnClient() {
    NetUtility.C_MAKE_MOVE?.Invoke(this);
  }
  public override void ReceivedOnServer(NetworkConnection cnn) {
    NetUtility.S_MAKE_MOVE?.Invoke(this, cnn);
  }
}
