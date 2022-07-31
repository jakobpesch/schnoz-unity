using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndOfStageView : MonoBehaviour {
  public Image Image { get; set; }
  private void Awake() {
    this.Image = this.GetComponent<Image>();
  }

}
