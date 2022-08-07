using UnityEngine;
using Schnoz;
using TMPro;
public class TimerView : MonoBehaviour {
  public StandardGameClient GameClient;
  public TextMeshProUGUI textField;

  public void Render() {
    this.textField.text = ((int)this.GameClient.Timer + 1).ToString();
    if (this.GameClient.Timer < 5) {
      this.textField.color = new Color(1, 0.5f, 0.5f);
      this.textField.fontSize = 144f;
    } else {
      this.textField.color = Color.white;
      this.textField.fontSize = 72f;
    }
  }
}
