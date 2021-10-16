using UnityEngine;
using Unity.Networking.Transport;
using System;

public enum OpCode {
  KEEP_ALIVE = 1,
  WELCOME = 2,
  START_GAME = 3,
  MAKE_MOVE = 4,
  INITIALISE_MAP = 5,
  UPDATE_UNITS = 6,
  UPDATE_TERRAINS = 7,
  UPDATE_CARDS = 8,
  END_TURN = 9,
  UPDATE_SCORE = 10,
  RENDER = 11,
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
  public static Action<NetMessage> C_KEEP_ALIVE;
  public static Action<NetMessage> C_WELCOME;
  public static Action<NetMessage> C_START_GAME;
  public static Action<NetMessage> C_MAKE_MOVE;
  public static Action<NetMessage> C_INITIALISE_MAP;
  public static Action<NetMessage> C_UPDATE_UNITS;
  public static Action<NetMessage> C_UPDATE_TERRAINS;
  public static Action<NetMessage> C_UPDATE_CARDS;
  public static Action<NetMessage> C_END_TURN;
  public static Action<NetMessage> C_UPDATE_SCORE;
  public static Action<NetMessage> C_RENDER;
  public static Action<NetMessage, NetworkConnection> S_KEEP_ALIVE;
  public static Action<NetMessage, NetworkConnection> S_WELCOME;
  public static Action<NetMessage, NetworkConnection> S_START_GAME;
  public static Action<NetMessage, NetworkConnection> S_MAKE_MOVE;
  public static Action<NetMessage, NetworkConnection> S_INITIALISE_MAP;
  public static Action<NetMessage, NetworkConnection> S_UPDATE_UNITS;
  public static Action<NetMessage, NetworkConnection> S_UPDATE_TERRAINS;
  public static Action<NetMessage, NetworkConnection> S_UPDATE_CARDS;
  public static Action<NetMessage, NetworkConnection> S_END_TURN;
  public static Action<NetMessage, NetworkConnection> S_UPDATE_SCORE;
  public static Action<NetMessage, NetworkConnection> S_RENDER;
}
