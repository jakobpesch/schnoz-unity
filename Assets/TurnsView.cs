using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Schnoz;

public class TurnsView : MonoBehaviour {

  public TurnView TurnViewPrefab;
  public EndOfStageView EndOfStageViewPrefab;
  private List<TurnView> TurnViews = new List<TurnView>();
  public List<EndOfStageView> EndOfStageViews = new List<EndOfStageView>();
  public StandardGameClient GameClient;

  public void Render() {
    int nTurnPerStage = this.GameClient.GameClient.GameSettings.NumberOfTurnsPerStage;
    if (this.transform.childCount == 0) {
      List<PlayerIds> turnOrder = this.GameClient.GameClient.GameSettings.TurnOrder;
      for (int i = 0; i < turnOrder.Count; i++) {
        PlayerIds playerId = turnOrder[i];
        TurnView turnView = Instantiate<TurnView>(TurnViewPrefab);
        turnView.PlayerId = playerId;
        turnView.Image.color = Constants.PlayerColors[playerId];
        turnView.transform.SetParent(this.transform);
        RectTransform rect = turnView.GetComponent<RectTransform>();
        rect.localScale = Vector3.one;
        this.TurnViews.Add(turnView);
        bool endOfStage = i != 0 && (i + 1) % nTurnPerStage == 0;
        if (endOfStage) {
          EndOfStageView endOfStageView = Instantiate<EndOfStageView>(EndOfStageViewPrefab);
          endOfStageView.Image.color = Color.green;
          endOfStageView.transform.SetParent(this.transform);
          RectTransform rectEndOfStage = endOfStageView.GetComponent<RectTransform>();
          rectEndOfStage.localScale = Vector3.one;
          this.EndOfStageViews.Add(endOfStageView);
        }
      }
    }
    int turn = this.GameClient.GameClient.Turn;
    if (turn > 0) {
      Destroy(this.TurnViews[0].gameObject);
      this.TurnViews.RemoveAt(0);
      bool endOfStage = turn % nTurnPerStage == 0;
      if (endOfStage) {
        Destroy(this.EndOfStageViews[0].gameObject);
        this.EndOfStageViews.RemoveAt(0);
      }
    }
  }
}
