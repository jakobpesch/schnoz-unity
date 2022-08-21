using System.Collections.Generic;
using Schnoz;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
  public SliderControlView ruleWaterSlider;
  public SliderControlView ruleHolesSlider;
  public SliderControlView ruleDiagonalsToTopRightSlider;
  private int mapSize { get => (int)mapSizeSlider.slider.value; }
  private int numberOfStages { get => (int)numberOfStagesSlider.slider.value; }
  private int secondsPerTurn { get => (int)secondsPerTurnSlider.slider.value; }
  private int partsGrass { get => (int)partsGrassSlider.slider.value; }
  private int partsStone { get => (int)partsStoneSlider.slider.value; }
  private int partsWater { get => (int)partsWaterSlider.slider.value; }
  private int partsBush { get => (int)partsBushSlider.slider.value; }
  private int ruleWater { get => (int)ruleWaterSlider.slider.value; }
  private int ruleHoles { get => (int)ruleHolesSlider.slider.value; }
  private int ruleDiagonalsToTopRight { get => (int)ruleDiagonalsToTopRightSlider.slider.value; }
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

    this.partsGrassSlider.UpdateText();
    this.partsStoneSlider.UpdateText();
    this.partsWaterSlider.UpdateText();
    this.partsBushSlider.UpdateText();

    this.ruleWaterSlider.UpdateText();
    this.ruleHolesSlider.UpdateText();
    this.ruleDiagonalsToTopRightSlider.UpdateText();

    this.StartGameButton.gameObject.SetActive(this.GameClient.ReadyToStartGame);
  }

  public void OnStartGameButton() {
    NetStartGame sg = new NetStartGame();
    sg.mapSize = this.mapSize;
    sg.numberOfStages = this.numberOfStages;
    sg.secondsPerTurn = this.secondsPerTurn;

    sg.partsGrass = this.partsGrass;
    sg.partsStone = this.partsStone;
    sg.partsWater = this.partsWater;
    sg.partsBush = this.partsBush;

    List<RuleNames> ruleNames = new List<RuleNames>();
    if (ruleWaterSlider.slider.value == 1) {
      ruleNames.Add(RuleNames.Water);

    }
    if (ruleHolesSlider.slider.value == 1) {
      ruleNames.Add(RuleNames.Holes);
    }

    if (ruleDiagonalsToTopRightSlider.slider.value == 1) {
      ruleNames.Add(RuleNames.DiagonalToTopRight);
    }

    if (ruleNames.Count == 0) {
      Debug.Log("At least one rule needs to be selected");
      return;
    }

    sg.ruleNames = ruleNames;

    RelayNetworking.Instance.SendToServer(sg);
    this.gameObject.SetActive(false);
  }
  private void Awake() {
    this.mapSizeSlider.slider.onValueChanged.AddListener(delegate { this.Render(); });
    this.numberOfStagesSlider.slider.onValueChanged.AddListener(delegate { this.Render(); });
    this.secondsPerTurnSlider.slider.onValueChanged.AddListener(delegate { this.Render(); });
    this.partsGrassSlider.slider.onValueChanged.AddListener(delegate { this.Render(); });
    this.partsStoneSlider.slider.onValueChanged.AddListener(delegate { this.Render(); });
    this.partsWaterSlider.slider.onValueChanged.AddListener(delegate { this.Render(); });
    this.partsBushSlider.slider.onValueChanged.AddListener(delegate { this.Render(); });
    this.ruleWaterSlider.slider.onValueChanged.AddListener(delegate { this.Render(); });
    this.ruleHolesSlider.slider.onValueChanged.AddListener(delegate { this.Render(); });
    this.ruleDiagonalsToTopRightSlider.slider.onValueChanged.AddListener(delegate { this.Render(); });
  }
}
