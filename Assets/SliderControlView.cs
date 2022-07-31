using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class SliderControlView : MonoBehaviour {
  public TextMeshProUGUI valueTextField;
  public Slider slider;
  public void UpdateText() {
    this.valueTextField.text = this.slider.value.ToString();
  }
}
