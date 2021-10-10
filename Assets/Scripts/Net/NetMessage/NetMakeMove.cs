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
  public int teamId;

  public NetMakeMove() {
    this.Code = OpCode.MAKE_MOVE;
  }
  public NetMakeMove(DataStreamReader reader) {
    this.Code = OpCode.MAKE_MOVE;
    this.Deserialize(reader);
  }
  public override void Serialize(ref DataStreamWriter writer) {
    writer.WriteByte((byte)this.Code);
    writer.WriteInt(this.row);
    writer.WriteInt(this.col);
    writer.WriteInt(this.unitFormationId);
    writer.WriteInt(this.rotation);
    writer.WriteInt(this.mirrorHorizontal);
    writer.WriteInt(this.mirrorVertical);
    writer.WriteInt(this.teamId);
  }
  public override void Deserialize(DataStreamReader reader) {
    this.row = reader.ReadInt();
    this.col = reader.ReadInt();
    this.unitFormationId = reader.ReadInt();
    this.rotation = reader.ReadInt();
    this.mirrorHorizontal = reader.ReadInt();
    this.mirrorVertical = reader.ReadInt();
    this.teamId = reader.ReadInt();
  }
  public override void ReceivedOnClient() {
    NetUtility.C_MAKE_MOVE?.Invoke(this);
  }
  public override void ReceivedOnServer(NetworkConnection cnn) {
    NetUtility.S_MAKE_MOVE?.Invoke(this, cnn);
  }
}
