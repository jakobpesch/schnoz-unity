using Unity.Networking.Transport;
public class NetUpdateSinglePieces : NetMessage {
  public int SinglePiecesPlayer1 { get; set; }
  public int SinglePiecesPlayer2 { get; set; }
  public NetUpdateSinglePieces() {
    this.Code = OpCode.UPDATE_SINGLE_PIECES;
  }

  public NetUpdateSinglePieces(DataStreamReader reader) {
    this.Code = OpCode.UPDATE_SINGLE_PIECES;
    this.Deserialize(reader);
  }

  public override void Serialize(ref DataStreamWriter writer) {
    writer.WriteByte((byte)this.Code);
    writer.WriteByte((byte)this.SinglePiecesPlayer1);
    writer.WriteByte((byte)this.SinglePiecesPlayer2);
  }

  public override void Deserialize(DataStreamReader reader) {
    this.SinglePiecesPlayer1 = (int)reader.ReadByte();
    this.SinglePiecesPlayer2 = (int)reader.ReadByte();
  }

  public override void ReceivedOnClient() {
    NetUtility.C_UPDATE_SINGLE_PIECES?.Invoke(this);
  }
  public override void ReceivedOnServer(NetworkConnection cnn) {
    NetUtility.S_UPDATE_SINGLE_PIECES?.Invoke(this, cnn);
  }
}
