using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class OnlineGameMenuManger : MonoBehaviour {
  [SerializeField] private string joinCode = "";
  [SerializeField] private TMP_InputField inputField;
  private void Awake() {
    this.inputField.text = joinCode;
  }
  public void OnHostButton() {
    Debug.Log("Hosting...");
    NetworkManager.Instance.Host();
    SchnozSceneManager.Instance.ChangeScene(SceneIndexes.ONLINE_GAME_MENU, SceneIndexes.GAME);
  }
  public void OnConnectButton() {
    Debug.Log("Connecting...");
    NetworkManager.Instance.Connect(this.inputField.text);
    SchnozSceneManager.Instance.ChangeScene(SceneIndexes.ONLINE_GAME_MENU, SceneIndexes.GAME);
  }
  public void OnBackButton() {
    SchnozSceneManager.Instance.ChangeScene(SceneIndexes.ONLINE_GAME_MENU, SceneIndexes.MAIN_MENU);
  }
}
