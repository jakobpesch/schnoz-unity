using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using Schnoz;

public class RuleView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
  public StandardGameClient GameClient;
  private RuleNames ruleName;
  public RuleNames RuleName {
    get => this.ruleName;
    set {
      if (this.ruleName != value) {
        this.ruleName = value;
        string fileName = this.ruleName.ToString();
        Sprite sprite = Resources.Load<Sprite>($"Sprites/{fileName}");
        this.Image.sprite = sprite;
      }
    }
  }
  public Dictionary<PlayerIds, TextMeshProUGUI> PlayerIdToTextField;
  public TextMeshProUGUI TextFieldPlayer1;
  public TextMeshProUGUI TextFieldPlayer2;
  public PlayerIds RuleWinner;
  public Image Image;
  private void Awake() {
    this.PlayerIdToTextField = new Dictionary<PlayerIds, TextMeshProUGUI>() {
      {PlayerIds.Player1, TextFieldPlayer1}, {PlayerIds.Player2, TextFieldPlayer2}
    };
    this.Image = this.GetComponent<Image>();
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
