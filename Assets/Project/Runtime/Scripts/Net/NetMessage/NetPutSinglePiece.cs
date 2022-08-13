using Unity.Networking.Transport;
using TypeAliases;
public class NetPutSinglePiece : NetMessage {
  public Coordinate coordinate;
  public NetPutSinglePiece() {
    this.Code = OpCode.PUT_SINGLE_PIECE;
  }

  public NetPutSinglePiece(DataStreamReader reader) {
    this.Code = OpCode.PUT_SINGLE_PIECE;
    this.Deserialize(reader);
  }

  public override void Serialize(ref DataStreamWriter writer) {
    writer.WriteByte((byte)this.Code);
    writer.WriteByte((byte)this.coordinate.row);
    writer.WriteByte((byte)this.coordinate.col);
  }

  public override void Deserialize(DataStreamReader reader) {
    int row = (int)reader.ReadByte();
    int col = (int)reader.ReadByte();
    this.coordinate = new Coordinate(row, col);
  }

  public override void ReceivedOnClient() {
    NetUtility.C_PUT_SINGLE_PIECE?.Invoke(this);
  }
  public override void ReceivedOnServer(NetworkConnection cnn) {
    NetUtility.S_PUT_SINGLE_PIECE?.Invoke(this, cnn);
  }
}
