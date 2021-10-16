using System.Collections.Generic;
using Schnoz;
using Unity.Networking.Transport;
using TypeAliases;

public class NetUpdateMap : NetMessage {
  public List<Unit> units;
  public List<Terrain> terrains;

  public NetUpdateMap() {
    this.Code = OpCode.UPDATE_MAP;
  }
  public NetUpdateMap(DataStreamReader reader) {
    this.Code = OpCode.UPDATE_MAP;
    this.Deserialize(reader);
  }
  public override void Serialize(ref DataStreamWriter writer) {
    writer.WriteByte((byte)this.Code);
    // Units
    writer.WriteInt(this.units.Count);
    for (int i = 0; i < this.units.Count; i++) {
      writer.WriteByte((byte)this.units[i].OwnerId);
      writer.WriteByte((byte)this.units[i].Coordinate.row);
      writer.WriteByte((byte)this.units[i].Coordinate.col);
    }

    // Terrains
    writer.WriteInt(this.terrains.Count);
    for (int i = 0; i < this.terrains.Count; i++) {
      writer.WriteByte((byte)(int)this.terrains[i].Type);
      writer.WriteByte((byte)this.terrains[i].Coordinate.row);
      writer.WriteByte((byte)this.terrains[i].Coordinate.col);
    }
  }
  public override void Deserialize(DataStreamReader reader) {
    // Units
    int unitsCount = reader.ReadInt();
    this.units = new List<Unit>();
    for (int i = 0; i < unitsCount; i++) {
      int ownerId = (int)reader.ReadByte();
      int row = (int)reader.ReadByte();
      int col = (int)reader.ReadByte();
      Coordinate coordinate = new Coordinate(row, col);
      Unit unit = new Unit(ownerId, coordinate);
      this.units.Add(unit);
    }

    // Terrains
    int terrainCount = reader.ReadInt();
    this.terrains = new List<Terrain>();
    for (int i = 0; i < terrainCount; i++) {
      TerrainType terrainType = (TerrainType)(int)reader.ReadByte();
      int row = (int)reader.ReadByte();
      int col = (int)reader.ReadByte();
      Coordinate coordinate = new Coordinate(row, col);
      Terrain terrain = new Terrain(terrainType, coordinate);
      this.terrains.Add(terrain);
    }
  }
  public override void ReceivedOnClient() {
    NetUtility.C_UPDATE_MAP?.Invoke(this);
  }
  public override void ReceivedOnServer(NetworkConnection cnn) {
    NetUtility.S_UPDATE_MAP?.Invoke(this, cnn);
  }
}
