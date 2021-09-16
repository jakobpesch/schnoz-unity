using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using TypeAliases;
using System.ComponentModel;
using Unity.Networking.Transport;
namespace Schnoz
{
  public class StandardGame : MonoBehaviour
  {
    // Multiplayer logic

    private int playerCount = -1;
    private int currentTeam = -1;
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
              // this.schnoz.PlaceUnitFormation(tile.Coordinate, this.Schnoz.SelectedCard.unitFormation);
              UnitFormation untiFormation = this.schnoz.SelectedCard.unitFormation;
              NetMakeMove mm = new NetMakeMove();
              mm.row = tile.Row;
              mm.col = tile.Col;
              mm.unitFormationId = UnitFormation.unitFormationTypeToIdDict[untiFormation.Type];
              mm.rotation = untiFormation.rotation;
              mm.mirrorHorizontal = untiFormation.mirrorHorizontal ? 1 : 0;
              mm.mirrorVertical = untiFormation.mirrorVertical ? 1 : 0;
              mm.teamId = this.currentTeam;
              Debug.Log(JsonUtility.ToJson(mm));
              Client.Instance.SendToServer(mm);
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

    private void RegisterEvents()
    {
      NetUtility.S_WELCOME += this.OnWelcomeServer;
      NetUtility.C_WELCOME += this.OnWelcomeClient;
      NetUtility.C_START_GAME += this.OnStartGameClient;
      NetUtility.S_MAKE_MOVE += this.OnMakeMoveServer;
      NetUtility.C_MAKE_MOVE += this.OnMakeMoveClient;
    }
    private void UnregisterEvents()
    {
    }

    private void Awake()
    {
      this.RegisterEvents();
    }
    private void OnDestroy()
    {
      this.UnregisterEvents();
    }

    // Server
    private void OnWelcomeServer(NetMessage msg, NetworkConnection cnn)
    {
      NetWelcome nw = msg as NetWelcome;

      nw.AssinedTeam = ++this.playerCount;

      Server.Instance.SendToClient(cnn, nw);

      if (this.playerCount == 1)
      {
        Server.Instance.Broadcast(new NetStartGame());
      }
    }
    // Client
    private void OnWelcomeClient(NetMessage msg)
    {
      NetWelcome nw = msg as NetWelcome;

      this.currentTeam = nw.AssinedTeam;


      Debug.Log($"My assigned team is {nw.AssinedTeam}");
    }
    private void OnStartGameClient(NetMessage msg)
    {
      NetWelcome nw = msg as NetWelcome;

      this.currentTeam = nw.AssinedTeam;


      Debug.Log($"My assigned team is {nw.AssinedTeam}");
    }
    private void OnMakeMoveClient(NetMessage msg)
    {
      NetMakeMove mm = msg as NetMakeMove;

      Coordinate coordinate = new Coordinate(mm.row, mm.col);
      Debug.Log(coordinate);
      UnitFormation unitFormation = new UnitFormation(UnitFormation.unitFormationIdToTypeDict[mm.unitFormationId]);
      unitFormation.rotation = mm.rotation;
      unitFormation.mirrorHorizontal = mm.mirrorHorizontal == 1 ? true : false;
      unitFormation.mirrorVertical = mm.mirrorVertical == 1 ? true : false;
      this.schnoz.PlaceUnitFormation(coordinate, unitFormation);
    }
    private void OnMakeMoveServer(NetMessage msg, NetworkConnection cnn)
    {
      NetMakeMove mm = msg as NetMakeMove;
      Server.Instance.Broadcast(mm);
    }
    private void Start()
    {
      this.gameSettings = new GameSettings(9, 9, 3, 0, 6, 30, new List<Player>() { new Player(0), new Player(1) });
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
