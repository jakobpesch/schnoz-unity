using System.Collections.Generic;
using Schnoz;
using Unity.Networking.Transport;
using TypeAliases;

public class NetInitialiseMap : NetMessage {

  public int mapSize;
  public int numberOfStages;
  public int secondsPerTurn;
  public int partsGrass;
  public int partsStone;
  public int partsWater;
  public int partsBush;
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

    writer.WriteByte((byte)this.mapSize);
    writer.WriteByte((byte)this.numberOfStages);
    writer.WriteByte((byte)this.secondsPerTurn);

    writer.WriteByte((byte)this.partsGrass);
    writer.WriteByte((byte)this.partsStone);
    writer.WriteByte((byte)this.partsWater);
    writer.WriteByte((byte)this.partsBush);

    // Rules
    writer.WriteInt(this.ruleNames.Count);
    for (int i = 0; i < this.ruleNames.Count; i++) {
      writer.WriteByte((byte)this.ruleNames[i]);
    }
  }
  public override void Deserialize(DataStreamReader reader) {

    this.mapSize = (int)reader.ReadByte();
    this.numberOfStages = (int)reader.ReadByte();
    this.secondsPerTurn = (int)reader.ReadByte();

    this.partsGrass = (int)reader.ReadByte();
    this.partsStone = (int)reader.ReadByte();
    this.partsWater = (int)reader.ReadByte();
    this.partsBush = (int)reader.ReadByte();

    // Rules
    int ruleNamesCount = reader.ReadInt();
    this.ruleNames = new List<RuleNames>();
    for (int i = 0; i < ruleNamesCount; i++) {
      this.ruleNames.Add((RuleNames)(int)reader.ReadByte());
    }
  }
  public override void ReceivedOnClient() {
    NetUtility.C_INITIALISE_MAP?.Invoke(this);
  }
  public override void ReceivedOnServer(NetworkConnection cnn) {
    NetUtility.S_INITIALISE_MAP?.Invoke(this, cnn);
  }
}
