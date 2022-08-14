using System.Collections;
using System.Collections.Generic;
using Schnoz;
using UnityEngine;
using UnityEngine.UI;

public class TurnView : MonoBehaviour {
  public PlayerIds PlayerId { get; set; }
  public Image Image { get; private set; }

  private void Awake() {
    this.Image = this.GetComponent<Image>();
  }
}
