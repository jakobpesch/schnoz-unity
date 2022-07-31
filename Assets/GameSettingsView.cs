using System.Collections.Generic;
using Schnoz;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System;

public class GameSettingsView : MonoBehaviour {
  public StandardGameClient GameClient;
  public GameSettings GameSettings;
  public SliderControlView mapSizeSlider;
  public SliderControlView numberOfStagesSlider;
  public SliderControlView secondsPerTurnSlider;
  public SliderControlView partsGrassSlider;
  public SliderControlView partsStoneSlider;
  public SliderControlView partsWaterSlider;
  public SliderControlView partsBushSlider;
  private int mapSize { get => (int)mapSizeSlider.slider.value; }
  private int numberOfStages { get => (int)numberOfStagesSlider.slider.value; }
  private int secondsPerTurn { get => (int)secondsPerTurnSlider.slider.value; }
  public Button StartGameButton;
  public TextMeshProUGUI StartGameButtonTextField;
  public void Render() {
    this.gameObject.SetActive(true);
    if (this.GameClient.AssignedRole == PlayerRoles.ADMIN) {
      this.StartGameButtonTextField.text = "Start Game";
    } else {
      this.StartGameButton.interactable = false;
      this.StartGameButtonTextField.text = "Waiting game to start ...";
    }

    this.mapSizeSlider.UpdateText();
    this.numberOfStagesSlider.UpdateText();
    this.secondsPerTurnSlider.UpdateText();
    this.StartGameButton.gameObject.SetActive(this.GameClient.ReadyToStartGame);
  }

  public void OnStartGameButton() {
    Debug.Log("OnStartGameButton");

    NetStartGame sg = new NetStartGame();
    sg.mapSize = this.mapSize;
    sg.numberOfStages = this.numberOfStages;
    sg.secondsPerTurn = this.secondsPerTurn;

    // TODO
    List<RuleNames> ruleNames = new List<RuleNames>();
    // ruleNames.Add(RuleNames.Water);
    ruleNames.Add(RuleNames.DiagonalToTopRight);
    ruleNames.Add(RuleNames.Holes);
    sg.ruleNames = ruleNames;

    Client.Instance.SendToServer(sg);
    this.gameObject.SetActive(false);
  }
  private void Awake() {
    this.mapSizeSlider.slider.onValueChanged.AddListener(delegate { this.Render(); });
    this.numberOfStagesSlider.slider.onValueChanged.AddListener(delegate { this.Render(); });
    this.secondsPerTurnSlider.slider.onValueChanged.AddListener(delegate { this.Render(); });
    // this.partsGrassSlider.slider.onValueChanged.AddListener(delegate { this.Render(); });
    // this.partsStoneSlider.slider.onValueChanged.AddListener(delegate { this.Render(); });
    // this.partsWaterSlider.slider.onValueChanged.AddListener(delegate { this.Render(); });
    // this.partsBushSlider.slider.onValueChanged.AddListener(delegate { this.Render(); });
  }


}
