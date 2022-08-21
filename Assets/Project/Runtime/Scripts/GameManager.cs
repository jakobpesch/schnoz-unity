using System;
using Schnoz;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour {
  public static GameManager Instance { set; get; }
  [SerializeField] private StandardGameServer gameServer;
  [SerializeField] private StandardGameClient gameClient;

  private void Awake() {
    Instance = this;
  }

  private void Start() {
    var gameScene = SceneManager.GetSceneByBuildIndex((int)SceneIndexes.GAME);
    switch (RelayNetworking.Instance.NI) {
      case RelayNetworking.NetworkIdentity.DEDICATED_SERVER: {
          throw new NotImplementedException();
          // Debug.Log("Loaded Scene as dedicated server");
          // gameServer = new GameObject("StandardGameServer").AddComponent<StandardGameServer>();
          // Server.Instance.Broadcast(new NetStartGame());
          // break;
        }
      case RelayNetworking.NetworkIdentity.HOST: {
          Debug.Log("Loaded Scene as host");
          this.gameServer = new GameObject("StandardGameServer").AddComponent<StandardGameServer>();
          this.gameClient = new GameObject("StandardGameClient").AddComponent<StandardGameClient>();
          SceneManager.MoveGameObjectToScene(this.gameServer.gameObject, gameScene);
          SceneManager.MoveGameObjectToScene(this.gameClient.gameObject, gameScene);
          break;
        }
      case RelayNetworking.NetworkIdentity.LOCAL: {
          Debug.Log("Loaded Scene as local game");
          this.gameServer = new GameObject("StandardGameServer").AddComponent<StandardGameServer>();
          this.gameClient = new GameObject("StandardGameClient").AddComponent<StandardGameClient>();
          SceneManager.MoveGameObjectToScene(this.gameServer.gameObject, gameScene);
          SceneManager.MoveGameObjectToScene(this.gameClient.gameObject, gameScene);
          break;
        }
      default: {
          Debug.Log("Loaded Scene as client");
          this.gameClient = new GameObject("StandardGameClient").AddComponent<StandardGameClient>();
          SceneManager.MoveGameObjectToScene(this.gameClient.gameObject, gameScene);
          break;
        }
    }
  }
}
