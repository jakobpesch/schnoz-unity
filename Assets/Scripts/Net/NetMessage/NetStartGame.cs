using System.Collections.Generic;
using Unity.Networking.Transport;
using Schnoz;
using TypeAliases;
using UnityEngine;

public class NetStartGame : NetMessage {
  public int nRows;
  public int nCols;
  public List<Unit> units;
  public List<Schnoz.Terrain> terrains;
  public List<Card> cards;
  public NetStartGame() {
    this.Code = OpCode.START_GAME;
  }
  public NetStartGame(DataStreamReader reader) {
    this.Code = OpCode.START_GAME;
    this.Deserialize(reader);
  }
  public override void Serialize(ref DataStreamWriter writer) {
    writer.WriteByte((byte)this.Code);

    // Map size
    writer.WriteByte((byte)this.nRows);
    writer.WriteByte((byte)this.nCols);

    // Units
    writer.WriteByte((byte)this.units.Count);
    for (int i = 0; i < this.units.Count; i++) {
      writer.WriteByte((byte)this.units[i].OwnerId);
      writer.WriteByte((byte)this.units[i].Coordinate.row);
      writer.WriteByte((byte)this.units[i].Coordinate.col);
    }

    // Terrains
    writer.WriteByte((byte)this.terrains.Count);
    Debug.Log($"Count: Terrain:{writer.Length}");

    for (int i = 0; i < this.terrains.Count; i++) {
      writer.WriteByte((byte)this.terrains[i].Type);
      writer.WriteByte((byte)this.terrains[i].Coordinate.row);
      writer.WriteByte((byte)this.terrains[i].Coordinate.col);
    }

    // Open cards
    writer.WriteByte((byte)this.cards.Count);
    for (int i = 0; i < this.cards.Count; i++) {

      Debug.Log($"Card type:{(byte)this.cards[i].Type}");
      writer.WriteByte((byte)this.cards[i].Type);
    }
    Debug.Log($"Writer length:{writer.Length}");
  }
  public override void Deserialize(DataStreamReader reader) {
    // Map size
    this.nRows = reader.ReadByte();
    this.nCols = reader.ReadByte();

    // Units
    int unitsCount = reader.ReadByte();
    this.units = new List<Unit>();
    for (int i = 0; i < unitsCount; i++) {
      int OwnerId = reader.ReadByte();
      int row = reader.ReadByte();
      int col = reader.ReadByte();
      Coordinate coordinate = new Coordinate(row, col);
      Unit unit = new Unit(OwnerId, coordinate);
      this.units.Add(unit);
    }

    // Terrains
    int terrainCount = reader.ReadByte();
    this.terrains = new List<Schnoz.Terrain>();
    for (int i = 0; i < terrainCount; i++) {
      TerrainType terrainType = (TerrainType)reader.ReadByte();
      int row = reader.ReadByte();
      int col = reader.ReadByte();
      Coordinate coordinate = new Coordinate(row, col);
      Schnoz.Terrain terrain = new Schnoz.Terrain(terrainType, coordinate);
      this.terrains.Add(terrain);
    }

    // Open cards
    int cardCount = reader.ReadByte();
    this.cards = new List<Card>();
    for (int i = 0; i < cardCount; i++) {
      CardType cardType = (CardType)(int)reader.ReadByte();
      Debug.Log($"Card type Deser: {cardType}");
      Card card = new Card(cardType);
      this.cards.Add(card);
    }
    Debug.Log($"Reader length:{reader.Length}");

  }
  public override void ReceivedOnClient() {
    NetUtility.C_START_GAME?.Invoke(this);
  }
  public override void ReceivedOnServer(NetworkConnection cnn) {
    NetUtility.S_START_GAME?.Invoke(this, cnn);
  }
}
