using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Schnoz
{
  public class StandardGame : MonoBehaviour
  {
    [SerializeField] private Schnoz schnoz;
    private GameSettings gameSettings;

    public Schnoz Schnoz
    {
      get => this.schnoz;
    }
    private void Start()
    {
      this.gameSettings = new GameSettings(5, 5, 3, 0, 6, 30, new List<Player>() { new Player(0), new Player(1) });
      this.schnoz = new Schnoz(this.gameSettings);

      StandardGameViewManager viewManager = new GameObject("ViewManager").AddComponent<StandardGameViewManager>();
      viewManager.transform.SetParent(this.transform);
      viewManager.game = this;
      viewManager.StartListening();
      // List<RuleLogic> ruleLogics = new List<RuleLogic>();
      // ruleLogics.Add(RuleLogicMethods.DiagonalToTopRight);
      StartCoroutine(this.Simulate());
    }
    private IEnumerator Simulate()
    {
      yield return new WaitForSeconds(1);
      this.schnoz.CreateMap();
      yield return new WaitForSeconds(1);
      this.schnoz.PlaceUnit(this.schnoz.Map.CenterTile.Pos);
      yield return new WaitForSeconds(1);
      this.schnoz.PlaceUnit((1, 1));
      yield return new WaitForSeconds(1);
      this.schnoz.PlaceUnit((0, 2));
      yield return new WaitForSeconds(1);
      this.schnoz.CreateDeck();
      yield return new WaitForSeconds(1);
      this.schnoz.ShuffleDeck();
      yield return new WaitForSeconds(1);
      this.schnoz.DrawCards();
      yield return new WaitForSeconds(1);
      this.schnoz.SetActivePlayer(0);
    }
  }
}
