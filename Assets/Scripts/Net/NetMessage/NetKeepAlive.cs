using Unity.Networking.Transport;

public class NetKeepAlive : NetMessage {
  public NetKeepAlive() {
    this.Code = OpCode.KEEP_ALIVE;
  }
  public NetKeepAlive(DataStreamReader reader) {
    this.Code = OpCode.KEEP_ALIVE;
    this.Deserialize(reader);
  }

  public override void Serialize(ref DataStreamWriter writer) {
    writer.WriteByte((byte)this.Code);
  }

  public override void Deserialize(DataStreamReader reader) {

  }

  public override void ReceivedOnClient() {
    NetUtility.C_KEEP_ALIVE?.Invoke(this);
  }
  public override void ReceivedOnServer(NetworkConnection cnn) {
    NetUtility.S_KEEP_ALIVE?.Invoke(this, cnn);
  }
}
