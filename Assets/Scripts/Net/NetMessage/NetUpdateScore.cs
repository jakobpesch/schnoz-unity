using Unity.Networking.Transport;
using Unity.Collections;
using Schnoz;
public class NetUpdateScore : NetMessage {
  public int ScorePlayer1 { get; set; }
  public int ScorePlayer2 { get; set; }

  public NetUpdateScore() {
    this.Code = OpCode.UPDATE_SCORE;
  }
  public NetUpdateScore(DataStreamReader reader) {
    this.Code = OpCode.UPDATE_SCORE;
    this.Deserialize(reader);
  }

  public override void Serialize(ref DataStreamWriter writer) {
    writer.WriteByte((byte)this.Code);
    writer.WriteByte((byte)ScorePlayer1);
    writer.WriteByte((byte)ScorePlayer2);
  }

  public override void Deserialize(DataStreamReader reader) {
    this.ScorePlayer1 = (int)reader.ReadByte();
    this.ScorePlayer2 = (int)reader.ReadByte();
  }

  public override void ReceivedOnClient() {
    NetUtility.C_UPDATE_SCORE?.Invoke(this);
  }
  public override void ReceivedOnServer(NetworkConnection cnn) {
    NetUtility.S_UPDATE_SCORE?.Invoke(this, cnn);
  }
}
