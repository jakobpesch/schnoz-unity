using System;
using Unity.Collections;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Schnoz;
using TypeAliases;
// using ParrelSync;

public enum SceneIndexes {
  MANAGER = 0,
  MAIN_MENU = 1,
  ONLINE_GAME_MENU = 2,
  WAITING_FOR_OPPONENT = 3,
  CONNECTING = 4,
  GAME = 5,
  LOCAL_GAME_MENU = 6,
}
public class SchnozSceneManager : MonoBehaviour {
  public static SchnozSceneManager Instance { set; get; }
  private void Awake() {
    Instance = this;
  }
  private void Start() {
    // if (ClonesManager.IsClone()) {
    //   string customArgument = ClonesManager.GetArgument();
    //   if (customArgument == "server") {
    //     NetworkManager.Instance.NI = NetworkManager.NetworkIdentity.DEDICATED_SERVER;
    //     Server.Instance.Init(Constants.port);
    //     SceneManager.LoadSceneAsync((int)SceneIndexes.GAME, LoadSceneMode.Additive);
    //   }
    //   if (customArgument == "client") {
    //     NetworkManager.Instance.NI = NetworkManager.NetworkIdentity.CLIENT;
    //     SceneManager.LoadSceneAsync((int)SceneIndexes.MAIN_MENU, LoadSceneMode.Additive);
    //   }
    // } else {
    NetworkManager.Instance.NI = NetworkManager.NetworkIdentity.CLIENT;
    SceneManager.LoadSceneAsync((int)SceneIndexes.MAIN_MENU, LoadSceneMode.Additive);
    // }
  }
  public GameObject loadingScreen;
  private List<AsyncOperation> scenesLoading = new List<AsyncOperation>();

  public void LoadOnlineGameMenu() {
    this.loadingScreen.gameObject.SetActive(true);
    this.scenesLoading.Add(SceneManager.UnloadSceneAsync((int)SceneIndexes.MAIN_MENU));
    this.scenesLoading.Add(SceneManager.LoadSceneAsync((int)SceneIndexes.ONLINE_GAME_MENU, LoadSceneMode.Additive));
  }
  public void LoadLocalGame() {
    this.loadingScreen.gameObject.SetActive(true);
    this.scenesLoading.Add(SceneManager.UnloadSceneAsync((int)SceneIndexes.MAIN_MENU));
    this.scenesLoading.Add(SceneManager.LoadSceneAsync((int)SceneIndexes.GAME, LoadSceneMode.Additive));
  }
  public void ChangeScenes(List<SceneIndexes> fromSceneIndexes, List<SceneIndexes> toSceneIndexes, bool withLoadingScreen = false) {
    foreach (SceneIndexes oldSceneIndex in fromSceneIndexes) {
      this.UnloadSceneAsync(oldSceneIndex);
    }
    foreach (SceneIndexes newSceneIndex in toSceneIndexes) {
      this.UnloadSceneAsync(newSceneIndex);
    }
    if (withLoadingScreen) {
      StartCoroutine(GetSceneLoadProgress());
    }
  }
  public void ChangeSceneAsync(SceneIndexes fromSceneIndex, SceneIndexes toSceneIndex, bool withLoadingScreen = false) {
    this.UnloadSceneAsync(fromSceneIndex, withLoadingScreen);
    this.LoadSceneAsync(toSceneIndex, withLoadingScreen);
    if (withLoadingScreen) {
      StartCoroutine(GetSceneLoadProgress());
    }
  }
  public void ChangeScene(SceneIndexes fromSceneIndex, SceneIndexes toSceneIndex, bool withLoadingScreen = false) {
    this.UnloadSceneAsync(fromSceneIndex, withLoadingScreen);
    this.LoadScene(toSceneIndex, LoadSceneMode.Additive);
  }
  public void LoadScene(SceneIndexes sceneIndex, LoadSceneMode loadScreenMode = LoadSceneMode.Single) {
    SceneManager.LoadScene((int)sceneIndex, loadScreenMode);
  }
  public void LoadSceneAsync(SceneIndexes sceneIndex, bool withLoadingScreen = false) {
    this.scenesLoading.Add(SceneManager.LoadSceneAsync((int)sceneIndex, LoadSceneMode.Additive));
    if (withLoadingScreen) {
      StartCoroutine(GetSceneLoadProgress());
    }
  }
  public void UnloadSceneAsync(SceneIndexes sceneIndex, bool withLoadingScreen = false) {
    this.scenesLoading.Add(SceneManager.UnloadSceneAsync((int)sceneIndex));
    if (withLoadingScreen) {
      StartCoroutine(GetSceneLoadProgress());
    }
  }

  public IEnumerator GetSceneLoadProgress() {
    Debug.Log("Loading");
    this.loadingScreen.gameObject.SetActive(true);
    float timeLoading = 0f;
    for (int i = 0; i < this.scenesLoading.Count; i++) {
      while (!scenesLoading[i].isDone) {
        if (timeLoading < 5) {
          timeLoading += Time.deltaTime;
          yield return null;
        } else {
          break;
        }
      }
    }
    this.loadingScreen.gameObject.SetActive(false);
  }
}
