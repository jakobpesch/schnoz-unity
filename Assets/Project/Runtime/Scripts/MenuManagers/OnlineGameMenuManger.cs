using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OnlineGameMenuManger : MonoBehaviour {

  [SerializeField] private TMP_InputField joinCodeInputField;
  [SerializeField] private TextMeshProUGUI networkStatus;
  [SerializeField] private Button retryButton;
  [SerializeField] private GameObject statusView;
  [SerializeField] private GameObject connectButtonsView;
  private bool signingUp = false;
  private bool signupError = false;

  private void Awake() {
    SignInAnonymously();
  }

  private async void SignInAnonymously() {
    signingUp = true;
    connectButtonsView.gameObject.SetActive(false);
    statusView.gameObject.SetActive(true);
    networkStatus.text = "Signing in ...";
    try {
      await RelayNetworking.Instance.Initialise();
      signingUp = false;
      statusView.gameObject.SetActive(false);
      connectButtonsView.gameObject.SetActive(true);
    } catch {
      signingUp = false;
      signupError = true;
      networkStatus.text = "Failed";
    }

  }
  public void OnHostButton() {
    if (signingUp) {
      return;
    }
    SchnozSceneManager.Instance.ChangeScene(SceneIndexes.ONLINE_GAME_MENU, SceneIndexes.GAME);
    RelayNetworking.Instance.Host();
  }

  public void OnConnectButton() {
    if (signingUp) {
      return;
    }
    RelayNetworking.Instance.Connect(this.joinCodeInputField.text);
    SchnozSceneManager.Instance.ChangeScene(SceneIndexes.ONLINE_GAME_MENU, SceneIndexes.GAME);
  }
  public void OnBackButton() {
    SchnozSceneManager.Instance.ChangeScene(SceneIndexes.ONLINE_GAME_MENU, SceneIndexes.MAIN_MENU);
  }
  public void OnRetryButton() {
    SignInAnonymously();
  }
}
