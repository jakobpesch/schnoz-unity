using System.Collections.Generic;
using Unity.Networking.Transport;
using Schnoz;
using TypeAliases;
using UnityEngine;

public class NetStartGame : NetMessage {
  public int mapSize;
  public int numberOfStages;
  public int secondsPerTurn;
  public int partsGrass;
  public int partsStone;
  public int partsWater;
  public int partsBush;

  public List<RuleNames> ruleNames = new List<RuleNames>();

  public NetStartGame() {
    this.Code = OpCode.START_GAME;
  }
  public NetStartGame(DataStreamReader reader) {
    this.Code = OpCode.START_GAME;
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

    writer.WriteByte((byte)this.ruleNames.Count);
    for (int i = 0; i < this.ruleNames.Count; i++) {
      Debug.Log($"Write rulename: {this.ruleNames[i]}");
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

    int ruleCount = reader.ReadByte();
    for (int i = 0; i < ruleCount; i++) {
      var ruleName = (RuleNames)reader.ReadByte();
      Debug.Log($"Read rulename: {ruleName}");

      this.ruleNames.Add(ruleName);
    }
  }
  public override void ReceivedOnClient() {
    NetUtility.C_START_GAME?.Invoke(this);
  }
  public override void ReceivedOnServer(NetworkConnection cnn) {
    NetUtility.S_START_GAME?.Invoke(this, cnn);
  }
}
