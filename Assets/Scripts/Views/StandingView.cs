using UnityEngine;
using System;
using System.Collections.Generic;
using Schnoz;
public class StandingView : MonoBehaviour {
  public PointsView PointsView;
  public RulesView RulesView;
  public StandardGameClient GameClient;
  public void Render() {
    this.PointsView.GameClient = this.GameClient;
    this.PointsView.Render();
    this.RulesView.GameClient = this.GameClient;
    this.RulesView.Render();
  }

}
