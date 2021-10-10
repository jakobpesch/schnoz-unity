using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class OnlineGameMenuManger : MonoBehaviour {
  [SerializeField] private string ipAddress = "127.0.0.1";
  [SerializeField] private TMP_InputField inputField;
  private void Awake() {
    this.inputField.text = ipAddress;
  }
  public void OnHostButton() {
    Debug.Log("Hosting...");
    NetworkManager.Instance.Host();
    SchnozSceneManager.Instance.ChangeScene(SceneIndexes.ONLINE_GAME_MENU, SceneIndexes.GAME);
  }
  public void OnConnectButton() {
    Debug.Log("Connecting...");
    Debug.Log(this.inputField.text);
    NetworkManager.Instance.Connect(this.inputField.text);
    SchnozSceneManager.Instance.ChangeScene(SceneIndexes.ONLINE_GAME_MENU, SceneIndexes.GAME);
  }
  public void OnBackButton() {
    SchnozSceneManager.Instance.ChangeScene(SceneIndexes.ONLINE_GAME_MENU, SceneIndexes.MAIN_MENU);
  }
}
