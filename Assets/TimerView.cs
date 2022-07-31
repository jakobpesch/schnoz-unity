using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Schnoz;
using TMPro;
public class TimerView : MonoBehaviour {
  public StandardGameClient GameClient;
  public TextMeshProUGUI textField;

  public void Render() {
    this.textField.text = ((int)this.GameClient.Timer).ToString();
  }
}
