using Schnoz;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOverView : MonoBehaviour {
  public StandardGameClient GameClient;
  public GameSettings GameSettings;
  public Button MainMenuButton;
  public TextMeshProUGUI Result;
  public void Render() {
    this.gameObject.SetActive(true);
  }

  public void OnMainMenuButton() {
    SchnozSceneManager.Instance.ChangeScene(SceneIndexes.GAME, SceneIndexes.MAIN_MENU, true);
    NetworkManager.Instance.client.Shutdown();
    NetworkManager.Instance.server.Shutdown();
  }
}
