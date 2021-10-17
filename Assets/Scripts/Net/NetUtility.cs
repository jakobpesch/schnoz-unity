using UnityEngine;
using Unity.Networking.Transport;
using System;

public enum OpCode {
  KEEP_ALIVE,
  WELCOME,
  START_GAME,
  MAKE_MOVE,
  INITIALISE_MAP,
  UPDATE_UNITS,
  UPDATE_TERRAINS,
  UPDATE_CARDS,
  UPDATE_SINGLE_PIECES,
  PUT_SINGLE_PIECE,
  END_TURN,
  UPDATE_SCORE,
  RENDER,
}


public static class NetUtility {
  public static void OnData(DataStreamReader stream, NetworkConnection cnn, Server server = null) {
    NetMessage msg = null;
    var opCode = (OpCode)stream.ReadByte();
    switch (opCode) {
      case OpCode.KEEP_ALIVE: msg = new NetKeepAlive(stream); break;
      case OpCode.WELCOME: msg = new NetWelcome(stream); break;
      case OpCode.START_GAME: msg = new NetStartGame(stream); break;
      case OpCode.MAKE_MOVE: msg = new NetMakeMove(stream); break;
      case OpCode.INITIALISE_MAP: msg = new NetInitialiseMap(stream); break;
      case OpCode.UPDATE_UNITS: msg = new NetUpdateUnits(stream); break;
      case OpCode.UPDATE_TERRAINS: msg = new NetUpdateTerrains(stream); break;
      case OpCode.UPDATE_CARDS: msg = new NetUpdateCards(stream); break;
      case OpCode.UPDATE_SINGLE_PIECES: msg = new NetUpdateSinglePieces(stream); break;
      case OpCode.PUT_SINGLE_PIECE: msg = new NetPutSinglePiece(stream); break;
      case OpCode.END_TURN: msg = new NetEndTurn(stream); break;
      case OpCode.UPDATE_SCORE: msg = new NetUpdateScore(stream); break;
      case OpCode.RENDER: msg = new NetRender(stream); break;
      default:
        Debug.Log("Message received had no opCode");
        break;
    }

    if (server != null) {
      msg.ReceivedOnServer(cnn);
    } else {
      msg.ReceivedOnClient();
    }
  }
  public static Action<NetMessage> C_KEEP_ALIVE, C_WELCOME, C_START_GAME, C_MAKE_MOVE, C_INITIALISE_MAP, C_UPDATE_UNITS, C_UPDATE_TERRAINS, C_UPDATE_CARDS, C_UPDATE_SINGLE_PIECES, C_PUT_SINGLE_PIECE, C_END_TURN, C_UPDATE_SCORE, C_RENDER;
  public static Action<NetMessage, NetworkConnection> S_KEEP_ALIVE, S_WELCOME, S_START_GAME, S_MAKE_MOVE, S_INITIALISE_MAP, S_UPDATE_UNITS, S_UPDATE_TERRAINS, S_UPDATE_CARDS, S_UPDATE_SINGLE_PIECES, S_PUT_SINGLE_PIECE, S_END_TURN, S_UPDATE_SCORE, S_RENDER;
}
