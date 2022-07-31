using System;
using Schnoz;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour {
  public static GameManager Instance { set; get; }
  [SerializeField] private StandardGameServer gameServer;
  [SerializeField] private StandardGameClient gameClient;
  [SerializeField] private GameObject startGameButton;
  private void Awake() {
    Instance = this;
  }

  private void Start() {
    var gameScene = SceneManager.GetSceneByBuildIndex((int)SceneIndexes.GAME);
    switch (NetworkManager.Instance.NI) {
      case NetworkManager.NetworkIdentity.DEDICATED_SERVER: {
          throw new NotImplementedException();
          // Debug.Log("Loaded Scene as dedicated server");
          // gameServer = new GameObject("StandardGameServer").AddComponent<StandardGameServer>();
          // Server.Instance.Broadcast(new NetStartGame());
          // break;
        }
      case NetworkManager.NetworkIdentity.HOST: {
          Debug.Log("Loaded Scene as host");
          this.gameServer = new GameObject("StandardGameServer").AddComponent<StandardGameServer>();
          this.gameClient = new GameObject("StandardGameClient").AddComponent<StandardGameClient>();
          SceneManager.MoveGameObjectToScene(this.gameServer.gameObject, gameScene);
          SceneManager.MoveGameObjectToScene(this.gameClient.gameObject, gameScene);
          break;
        }
      case NetworkManager.NetworkIdentity.LOCAL: {
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
