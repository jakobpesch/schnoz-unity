using System.Collections.Generic;
using Schnoz;
using Unity.Networking.Transport;
using TypeAliases;
using UnityEngine;

public class NetUpdateUnits : NetMessage {
  public List<Unit> addedUnits = new List<Unit>();
  public List<Coordinate> removedUnitsCoordinates = new List<Coordinate>();
  public NetUpdateUnits() {
    this.Code = OpCode.UPDATE_UNITS;
  }
  public NetUpdateUnits(DataStreamReader reader) {
    this.Code = OpCode.UPDATE_UNITS;
    this.Deserialize(reader);
  }
  public override void Serialize(ref DataStreamWriter writer) {
    writer.WriteByte((byte)this.Code);
    // Added Units
    writer.WriteInt(this.addedUnits.Count);
    for (int i = 0; i < this.addedUnits.Count; i++) {
      writer.WriteByte((byte)this.addedUnits[i].OwnerId);
      writer.WriteByte((byte)this.addedUnits[i].Coordinate.row);
      writer.WriteByte((byte)this.addedUnits[i].Coordinate.col);
    }
    // Removed Units
    writer.WriteInt(this.removedUnitsCoordinates.Count);
    for (int i = 0; i < this.removedUnitsCoordinates.Count; i++) {
      writer.WriteByte((byte)this.removedUnitsCoordinates[i].row);
      writer.WriteByte((byte)this.removedUnitsCoordinates[i].col);
    }
  }
  public override void Deserialize(DataStreamReader reader) {
    // Added Units
    int unitsCount = reader.ReadInt();
    this.addedUnits = new List<Unit>();
    for (int i = 0; i < unitsCount; i++) {
      PlayerIds ownerId = (PlayerIds)reader.ReadByte();
      int row = (int)reader.ReadByte();
      int col = (int)reader.ReadByte();
      Coordinate coordinate = new Coordinate(row, col);
      Unit unit = new Unit(ownerId, coordinate);
      this.addedUnits.Add(unit);
    }
    // Removed Units
    int removedUnitsCoordinatesCount = reader.ReadInt();
    this.removedUnitsCoordinates = new List<Coordinate>();
    for (int i = 0; i < removedUnitsCoordinatesCount; i++) {
      int row = (int)reader.ReadByte();
      int col = (int)reader.ReadByte();
      Coordinate coordinate = new Coordinate(row, col);
      this.removedUnitsCoordinates.Add(coordinate);
    }
  }
  public override void ReceivedOnClient() {
    NetUtility.C_UPDATE_UNITS?.Invoke(this);
  }
  public override void ReceivedOnServer(NetworkConnection cnn) {
    NetUtility.S_UPDATE_UNITS?.Invoke(this, cnn);
  }
}
