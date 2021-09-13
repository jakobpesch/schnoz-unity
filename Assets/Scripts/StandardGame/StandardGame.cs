using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using TypeAliases;
using System.ComponentModel;
namespace Schnoz
{
  public class StandardGame : MonoBehaviour
  {
    private StandardGameViewManager viewManager;
    [SerializeField] private Schnoz schnoz;
    private GameSettings gameSettings;
    private Tile hoveringTile;
    public Tile HoveringTile
    {
      get { return this.hoveringTile; }
      set
      {
        this.hoveringTile = value;
        SetHoveringTiles(this.HoveringTile);
      }
    }
    public List<Tile> HoveringTiles;

    public Coordinate p = new Coordinate(0, 0);
    public Schnoz Schnoz
    {
      get => this.schnoz;
    }
    public void HandlePlayerInput(InputEventNames evt, object obj = null)
    {
      if (evt == InputEventNames.RotateRightButton)
      {
        this.schnoz.SelectedCard.unitFormation.RotateRight();
        SetHoveringTiles(this.hoveringTile);
      }
      if (evt == InputEventNames.RotateLeftButton)
      {
        this.schnoz.SelectedCard.unitFormation.RotateLeft();
        SetHoveringTiles(this.hoveringTile);
      }
      if (evt == InputEventNames.MirrorHorizontalButton)
      {
        this.schnoz.SelectedCard.unitFormation.MirrorHorizontal();
        SetHoveringTiles(this.hoveringTile);
      }
      if (evt == InputEventNames.MirrorVerticalButton)
      {
        this.schnoz.SelectedCard.unitFormation.MirrorVertical();
        SetHoveringTiles(this.hoveringTile);
      }

      if (typeof(Coordinate) == obj?.GetType())
      {
        Tile tile = this.Schnoz.Map.TileDict[(Coordinate)obj];
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

          this.HoveringTile = tile;
        }

        if (evt == InputEventNames.OnMouseExit)
        {
        }
      }
      if (typeof(Card) == obj?.GetType())
      {
        Card card = (Card)obj;
        if (evt == InputEventNames.OnMouseUp)
        {
          this.Schnoz.SelectCard(card);
        }
      }

    }
    private void SetHoveringTiles(Tile tile)
    {
      this.HoveringTiles = new List<Tile>();
      Arrangement arrangement = this.schnoz.SelectedCard?.unitFormation?.Arrangement;
      if (arrangement == null)
      {
        return;
      }
      foreach (Coordinate offset in arrangement)
      {
        Coordinate c = tile.Coordinate + offset;
        if (this.Schnoz.Map.TileDict.ContainsKey(c))
        {
          Tile t = this.Schnoz.Map.TileDict[c];
          this.HoveringTiles.Add(t);
        }
      }
      this.viewManager.OnPropertyChanged(this, new PropertyChangedEventArgs("Highlight"));
    }
    private void Start()
    {
      this.gameSettings = new GameSettings(49, 49, 3, 0, 6, 30, new List<Player>() { new Player(0), new Player(1) });
      this.schnoz = new Schnoz(this.gameSettings);

      this.viewManager = new GameObject("ViewManager").AddComponent<StandardGameViewManager>();
      this.viewManager.transform.SetParent(this.transform);
      this.viewManager.game = this;
      this.viewManager.StartListening();

      this.schnoz.CreateMap();
      this.schnoz.CreateDeck();
      this.schnoz.ShuffleDeck();
      this.schnoz.DrawCards();
      // List<RuleLogic> ruleLogics = new List<RuleLogic>();
      // ruleLogics.Add(RuleLogicMethods.DiagonalToTopRight);
    }
  }
}
