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
    writer.WriteInt(ScorePlayer1);
    writer.WriteInt(ScorePlayer2);
  }

  public override void Deserialize(DataStreamReader reader) {
    ScorePlayer1 = reader.ReadInt();
    ScorePlayer2 = reader.ReadInt();
  }

  public override void ReceivedOnClient() {
    NetUtility.C_UPDATE_SCORE?.Invoke(this);
  }
  public override void ReceivedOnServer(NetworkConnection cnn) {
    NetUtility.S_UPDATE_SCORE?.Invoke(this, cnn);
  }
}
