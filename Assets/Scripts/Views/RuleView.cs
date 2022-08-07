using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using Schnoz;

public class RuleView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
  public StandardGameClient GameClient;
  public RuleNames ruleName;
  public RuleNames RuleName {
    get => this.ruleName;
    set {
      if (this.ruleName != value) {
        this.ruleName = value;
      }
    }
  }
  public Dictionary<PlayerIds, TextMeshProUGUI> PlayerIdToTextField;
  public TextMeshProUGUI TextFieldPlayer1;
  public TextMeshProUGUI TextFieldPlayer2;
  public PlayerIds RuleWinner;
  public Image Background;
  public Image Icon;
  private void Awake() {
    this.PlayerIdToTextField = new Dictionary<PlayerIds, TextMeshProUGUI>() {
      {PlayerIds.Player1, this.TextFieldPlayer1}, {PlayerIds.Player2, this.TextFieldPlayer2}
    };
  }
  public void ShowRelevantTiles() {
    this.GameClient.HandlePlayerInput(this, InputEventNames.ShowRelevantTiles, this.RuleName);
  }

  public void OnPointerEnter(PointerEventData eventData) {
    Debug.Log("OnPointerEnter");
    this.ShowRelevantTiles();
  }

  public void OnPointerExit(PointerEventData eventData) {
    Debug.Log("OnPointerExit");
    this.ShowRelevantTiles();
  }
}
