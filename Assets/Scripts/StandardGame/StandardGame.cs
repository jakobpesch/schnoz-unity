using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Schnoz
{
  public class StandardGame : MonoBehaviour
  {
    [SerializeField] private Schnoz schnoz;
    private GameSettings gameSettings;
    private UnitFormation unitFormation;
    public Schnoz Schnoz
    {
      get => this.schnoz;
    }
    public void HandlePlayerInput(InputEventNames evt, object obj)
    {
      if (typeof(Tile) == obj.GetType())
      {
        Tile tile = (Tile)obj;
        if (evt == InputEventNames.OnMouseUp)
        {
          if (tile.Unit == null)
          {
            // this.schnoz.PlaceUnit(tile.Pos);
            this.schnoz.PlaceUnitFormation(tile.Pos, this.unitFormation);
          }
          else
          {
            this.schnoz.RemoveUnit(tile.Pos);
          }
        }
      }
    }
    private void Start()
    {
      this.gameSettings = new GameSettings(5, 5, 3, 0, 6, 30, new List<Player>() { new Player(0), new Player(1) });
      this.schnoz = new Schnoz(this.gameSettings);
      List<(int row, int col)> arrangement = new List<(int, int)>() { (0, 0) };
      this.unitFormation = new UnitFormation(arrangement);

      StandardGameViewManager viewManager = new GameObject("ViewManager").AddComponent<StandardGameViewManager>();
      viewManager.transform.SetParent(this.transform);
      viewManager.game = this;
      viewManager.StartListening();

      this.schnoz.CreateMap();
      this.schnoz.CreateDeck();
      this.schnoz.ShuffleDeck();
      this.schnoz.DrawCards();
      // List<RuleLogic> ruleLogics = new List<RuleLogic>();
      // ruleLogics.Add(RuleLogicMethods.DiagonalToTopRight);
    }
  }
}
