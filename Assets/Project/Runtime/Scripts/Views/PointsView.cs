using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Schnoz;
using TMPro;

public class PointsView : MonoBehaviour {
  public TextMeshProUGUI TextFieldPlayer1;
  public TextMeshProUGUI TextFieldPlayer2;
  public StandardGameClient GameClient;
  public void Render() {
    var playerIdToplayer = this.GameClient.GameClient.GameSettings.PlayerIdToPlayerDict;
    this.TextFieldPlayer1.text = playerIdToplayer[PlayerIds.Player1].Score.ToString();
    this.TextFieldPlayer2.text = playerIdToplayer[PlayerIds.Player2].Score.ToString();
  }
}
