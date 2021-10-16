using Unity.Networking.Transport;
using Schnoz;
using System.Collections.Generic;
public class NetRender : NetMessage {
  public List<RenderTypes> renderTypes = new List<RenderTypes>();
  public NetRender() {
    this.Code = OpCode.RENDER;
  }

  public NetRender(DataStreamReader reader) {
    this.Code = OpCode.RENDER;
    this.Deserialize(reader);
  }

  public override void Serialize(ref DataStreamWriter writer) {
    writer.WriteByte((byte)this.Code);

    writer.WriteByte((byte)this.renderTypes.Count);
    for (int i = 0; i < this.renderTypes.Count; i++) {
      writer.WriteByte((byte)this.renderTypes[i]);
    }
  }

  public override void Deserialize(DataStreamReader reader) {
    this.renderTypes = new List<RenderTypes>();
    int renderTypesCount = (int)reader.ReadByte();
    for (int i = 0; i < renderTypesCount; i++) {
      this.renderTypes.Add((RenderTypes)reader.ReadByte());
    }
  }

  public override void ReceivedOnClient() {
    NetUtility.C_RENDER?.Invoke(this);
  }
  public override void ReceivedOnServer(NetworkConnection cnn) {
    NetUtility.S_RENDER?.Invoke(this, cnn);
  }
}
