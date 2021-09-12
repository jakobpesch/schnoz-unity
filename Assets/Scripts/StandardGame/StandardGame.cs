using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using TypeAliases;
namespace Schnoz
{
  public class StandardGame : MonoBehaviour
  {

    [SerializeField] private Schnoz schnoz;
    private GameSettings gameSettings;
    public List<Tile> HoveringTiles = new List<Tile>();

    public Coordinate p = new Coordinate(0, 0);
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
            if (this.Schnoz.SelectedCard != null)
            {
              this.schnoz.PlaceUnitFormation(tile.Coordinate, this.Schnoz.SelectedCard.unitFormation);
            }
          }
          else
          {
            this.schnoz.RemoveUnit(tile.Coordinate);
          }
        }

        if (evt == InputEventNames.OnMouseEnter)
        {
          List<Tile> tiles = this.schnoz.Map.Tiles.Where(
            tileOnMap => this.schnoz.SelectedCard.unitFormation.Arrangement.Any(
              arrangement => tileOnMap.Coordinate == tile.Coordinate + arrangement
              )
            ).ToList();
          this.HoveringTiles = tiles;
        }

        if (evt == InputEventNames.OnMouseExit)
        {
          // this.HoveringTiles.Remove(tile);
        }
      }
      if (typeof(Card) == obj.GetType())
      {
        Card card = (Card)obj;
        if (evt == InputEventNames.OnMouseUp)
        {
          this.Schnoz.SelectCard(card);
        }
      }
    }
    private void Start()
    {
      this.gameSettings = new GameSettings(5, 5, 3, 0, 6, 30, new List<Player>() { new Player(0), new Player(1) });
      this.schnoz = new Schnoz(this.gameSettings);

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
