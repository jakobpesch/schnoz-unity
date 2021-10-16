using System.Collections.Generic;
using Schnoz;
using Unity.Networking.Transport;
using TypeAliases;

public class NetInitialiseMap : NetMessage {

  public int nRows;
  public int nCols;

  public NetInitialiseMap() {
    this.Code = OpCode.INITIALISE_MAP;
  }
  public NetInitialiseMap(DataStreamReader reader) {
    this.Code = OpCode.INITIALISE_MAP;
    this.Deserialize(reader);
  }
  public override void Serialize(ref DataStreamWriter writer) {
    writer.WriteByte((byte)this.Code);

    // Map size
    writer.WriteByte((byte)this.nRows);
    writer.WriteByte((byte)this.nCols);
  }
  public override void Deserialize(DataStreamReader reader) {
    // Map size
    this.nRows = reader.ReadByte();
    this.nCols = reader.ReadByte();
  }
  public override void ReceivedOnClient() {
    NetUtility.C_INITIALISE_MAP?.Invoke(this);
  }
  public override void ReceivedOnServer(NetworkConnection cnn) {
    NetUtility.S_INITIALISE_MAP?.Invoke(this, cnn);
  }
}
