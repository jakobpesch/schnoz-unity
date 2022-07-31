using System.Collections.Generic;
using Schnoz;
using Unity.Networking.Transport;
using TypeAliases;

public class NetInitialiseMap : NetMessage {

  public int mapSize;
  public List<RuleNames> ruleNames = new List<RuleNames>();

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
    writer.WriteByte((byte)this.mapSize);

    // Rules
    writer.WriteInt(this.ruleNames.Count);
    for (int i = 0; i < this.ruleNames.Count; i++) {
      writer.WriteByte((byte)this.ruleNames[i]);
    }
  }
  public override void Deserialize(DataStreamReader reader) {
    // Map size
    this.mapSize = reader.ReadByte();

    // Rules
    int ruleNamesCount = reader.ReadInt();
    this.ruleNames = new List<RuleNames>();
    for (int i = 0; i < ruleNamesCount; i++) {
      this.ruleNames.Add((RuleNames)reader.ReadByte());
    }
  }
  public override void ReceivedOnClient() {
    NetUtility.C_INITIALISE_MAP?.Invoke(this);
  }
  public override void ReceivedOnServer(NetworkConnection cnn) {
    NetUtility.S_INITIALISE_MAP?.Invoke(this, cnn);
  }
}
