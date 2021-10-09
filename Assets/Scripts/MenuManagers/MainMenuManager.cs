using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
  public void OnOnlineGameButton()
  {
    Debug.Log("Online Game");
    SchnozSceneManager.Instance.ChangeScene(SceneIndexes.MAIN_MENU, SceneIndexes.ONLINE_GAME_MENU, false);
  }
  public void OnLocalGameButton()
  {
    Debug.Log("Local Game");
    NetworkManager.Instance.LocalGame();
    SchnozSceneManager.Instance.ChangeScene(SceneIndexes.MAIN_MENU, SceneIndexes.GAME);
  }
  public void OnQuitButton()
  {
    Application.Quit();
  }
}
