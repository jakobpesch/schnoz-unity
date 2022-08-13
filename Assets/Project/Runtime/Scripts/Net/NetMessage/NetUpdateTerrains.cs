using System.Collections.Generic;
using Schnoz;
using Unity.Networking.Transport;
using TypeAliases;

public class NetUpdateTerrains : NetMessage {
  public List<Unit> units;
  public List<Terrain> terrains;

  public NetUpdateTerrains() {
    this.Code = OpCode.UPDATE_TERRAINS;
  }
  public NetUpdateTerrains(DataStreamReader reader) {
    this.Code = OpCode.UPDATE_TERRAINS;
    this.Deserialize(reader);
  }
  public override void Serialize(ref DataStreamWriter writer) {
    writer.WriteByte((byte)this.Code);
    // Terrains
    writer.WriteInt(this.terrains.Count);
    for (int i = 0; i < this.terrains.Count; i++) {
      writer.WriteByte((byte)(int)this.terrains[i].Type);
      writer.WriteByte((byte)this.terrains[i].Coordinate.row);
      writer.WriteByte((byte)this.terrains[i].Coordinate.col);
    }
  }
  public override void Deserialize(DataStreamReader reader) {
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
    NetUtility.C_UPDATE_TERRAINS?.Invoke(this);
  }
  public override void ReceivedOnServer(NetworkConnection cnn) {
    NetUtility.S_UPDATE_TERRAINS?.Invoke(this, cnn);
  }
}
