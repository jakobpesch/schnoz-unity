using System;
using Schnoz;
using UnityEngine;
public class GameManager : MonoBehaviour
{
  public static GameManager Instance { set; get; }
  [SerializeField] private StandardGameServer gameServer;
  [SerializeField] private StandardGameClient gameClient;
  [SerializeField] private GameObject startGameButton;
  private void Awake()
  {
    Instance = this;
  }

  private void Start()
  {
    switch (NetworkManager.Instance.NI)
    {
      case NetworkManager.NetworkIdentity.DEDICATED_SERVER:
        {
          Debug.Log("Loaded Scene as dedicated server");
          gameServer = new GameObject("StandardGameServer").AddComponent<StandardGameServer>();
          Server.Instance.Broadcast(new NetStartGame());
          break;
        }
      case NetworkManager.NetworkIdentity.HOST:
        {
          Debug.Log("Loaded Scene as host");
          gameServer = new GameObject("StandardGameServer").AddComponent<StandardGameServer>();
          gameClient = new GameObject("StandardGameClient").AddComponent<StandardGameClient>();
          break;
        }
      case NetworkManager.NetworkIdentity.LOCAL:
        {
          Debug.Log("Loaded Scene as local game");
          gameServer = new GameObject("StandardGameServer").AddComponent<StandardGameServer>();
          gameClient = new GameObject("StandardGameClient").AddComponent<StandardGameClient>();
          break;
        }
      default:
        {
          Debug.Log("Loaded Scene as client");
          gameClient = new GameObject("StandardGameClient").AddComponent<StandardGameClient>();
          break;
        }
    }
  }

}
