using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using TypeAliases;
using System.ComponentModel;
namespace Schnoz
{
  public class StandardGameClient : MonoBehaviour
  {
    // Multiplayer logic
    [SerializeField] private int currentTeam = -1;
    public int CurrentTeam
    {
      get => this.currentTeam;
    }
    [SerializeField] private StandardGameViewManager viewManager;
    public Schnoz Game { get; private set; }
    [SerializeField] private GameSettings gameSettings;
    [SerializeField] private Tile hoveringTile;
    public Tile HoveringTile
    {
      get => this.hoveringTile;
      set
      {
        this.hoveringTile = value;
        this.SetHoveringTiles(this.HoveringTile);
      }
    }
    public List<Tile> HoveringTiles;
    public Guid selectedCardId;
    public Guid SelectedCardId
    {
      get => this.selectedCardId;
      set
      {
        this.selectedCardId = value;
        this.viewManager.OnPropertyChanged(this, new PropertyChangedEventArgs("SelectedCard"));
      }
    }
    public Dictionary<Guid, Card> OpenCardsDict
    {
      get => this.Game.OpenCards.ToDictionary(card => card.Id);
    }
    public Dictionary<Coordinate, Tile> TileDict
    {
      get => this.Game.Map.Tiles.ToDictionary(tile => tile.Coordinate);
    }
    public void HandlePlayerInput(object sender, InputEventNames evt, object obj = null)
    {
      #region Change card orientation
      if (this.SelectedCardId != null)
      {
        if (evt == InputEventNames.RotateRightButton)
        {
          this.OpenCardsDict[this.SelectedCardId].unitFormation.RotateRight();
          this.SetHoveringTiles(this.hoveringTile);
        }
        if (evt == InputEventNames.RotateLeftButton)
        {
          this.OpenCardsDict[this.SelectedCardId].unitFormation.RotateLeft();
          this.SetHoveringTiles(this.hoveringTile);
        }
        if (evt == InputEventNames.MirrorHorizontalButton)
        {
          this.OpenCardsDict[this.SelectedCardId].unitFormation.MirrorHorizontal();
          this.SetHoveringTiles(this.hoveringTile);
        }
        if (evt == InputEventNames.MirrorVerticalButton)
        {
          this.OpenCardsDict[this.SelectedCardId].unitFormation.MirrorVertical();
          this.SetHoveringTiles(this.hoveringTile);
        }
      }
      #endregion

      #region Tile events
      if (typeof(TileView) == sender?.GetType())
      {
        Tile tile = this.TileDict[(Coordinate)obj];

        // Place tile
        if (evt == InputEventNames.OnMouseUp)
        {
          if (this.SelectedCardId != null
            && this.OpenCardsDict.ContainsKey(this.SelectedCardId)
            && this.OpenCardsDict[this.SelectedCardId] != null)
          {
            UnitFormation untiFormation = this.OpenCardsDict[this.SelectedCardId].unitFormation;
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

        // Set hovering tile
        if (evt == InputEventNames.OnMouseEnter)
        {
          this.SetHoveringTiles(tile);
          return;
        }

        if (evt == InputEventNames.OnMouseExit)
        {
          this.SetHoveringTiles(null);
        }
      }
      #endregion

      #region Card events
      if (typeof(CardView) == sender?.GetType())
      {
        Guid cardId = (Guid)obj;
        if (evt == InputEventNames.OnMouseUp)
        {
          this.SelectedCardId = cardId;
        }
      }
      #endregion
    }

    #region UI
    private void SetHoveringTiles(Tile tile)
    {
      if (tile == null)
      {
        this.HoveringTiles = new List<Tile>();
        this.viewManager.OnPropertyChanged(this, new PropertyChangedEventArgs("Highlight"));
        return;
      }
      this.hoveringTile = tile;
      if (this.SelectedCardId == Guid.Empty)
      {
        return;
      }
      this.HoveringTiles = new List<Tile>();
      Arrangement arrangement = this.OpenCardsDict[this.SelectedCardId].unitFormation.Arrangement;
      if (arrangement == null)
      {
        return;
      }
      foreach (Coordinate offset in arrangement)
      {
        Coordinate c = tile.Coordinate + offset;
        if (this.Game.Map.CoordinateToTileDict.ContainsKey(c))
        {
          Tile t = this.Game.Map.CoordinateToTileDict[c];
          this.HoveringTiles.Add(t);
        }
      }
      this.viewManager.OnPropertyChanged(this, new PropertyChangedEventArgs("Highlight"));
    }
    #endregion

    #region Networking
    // Setup and cleanup
    private void Awake()
    {
      this.RegisterEvents();
    }
    private void OnDestroy()
    {
      this.UnregisterEvents();
    }
    private void RegisterEvents()
    {
      NetUtility.C_WELCOME += this.OnWelcome;
      NetUtility.C_START_GAME += this.OnStartGame;
      NetUtility.C_UPDATE_CARDS += this.OnUpdateCards;
      NetUtility.C_UPDATE_MAP += this.OnUpdateMap;
    }
    private void UnregisterEvents()
    {
      NetUtility.C_WELCOME -= this.OnWelcome;
      NetUtility.C_START_GAME -= this.OnStartGame;
      NetUtility.C_UPDATE_CARDS -= this.OnUpdateCards;
      NetUtility.C_UPDATE_MAP -= this.OnUpdateMap;
    }

    /// <summary>
    /// The server accepted the connection and assigned a team to the client
    /// </summary>
    /// <param name="msg"></param>
    private void OnWelcome(NetMessage msg)
    {
      NetWelcome nw = msg as NetWelcome;
      this.currentTeam = nw.AssinedTeam;
    }

    /// <summary>
    /// The server started the game and sends the map
    /// </summary>
    /// <param name="msg"></param>
    private void OnStartGame(NetMessage msg)
    {
      NetStartGame sg = msg as NetStartGame;
      Debug.Log("OnStartGame");
      Debug.Log($"Map: {sg.netMapString.ToString()}");
      Debug.Log($"OpenCards: {sg.netOpenCardsString.ToString()}");

      this.gameSettings = new GameSettings(9, 9, 3, 0, 6, 30, new List<Player>() { new Player(0), new Player(1) });
      this.Game = new Schnoz(this.gameSettings);

      NetMap netMap = JsonUtility.FromJson<NetMap>(sg.netMapString.ToString());
      this.Game.Map = new Map(netMap);

      NetOpenCards netOpenCards = JsonUtility.FromJson<NetOpenCards>(sg.netOpenCardsString.ToString());
      this.Game.OpenCards = new List<Card>();
      foreach (NetCard netCard in netOpenCards.o)
      {
        this.Game.OpenCards.Add(new Card((CardType)netCard.t));
      }

      this.CreateViewManager();
      this.viewManager.OnPropertyChanged(this, new PropertyChangedEventArgs("Map"));
      this.viewManager.OnPropertyChanged(this, new PropertyChangedEventArgs("OpenCards"));
    }
    private void OnUpdateCards(NetMessage msg)
    {
      NetUpdateCards uc = msg as NetUpdateCards;
      NetOpenCards netOpenCards = JsonUtility.FromJson<NetOpenCards>(uc.cardsString.ToString());
      foreach (NetCard netCard in netOpenCards.o)
      {
        this.Game.OpenCards.Add(new Card(netCard));
      }
      this.viewManager.OnPropertyChanged(this, new PropertyChangedEventArgs("OpenCards"));
    }

    /// <summary>
    /// The server sends the updated the map
    /// </summary>
    /// <param name="msg"></param>
    private void OnUpdateMap(NetMessage msg)
    {
      NetUpdateMap up = msg as NetUpdateMap;
      Debug.Log($"OnUpdateMap: {up.netMapString.ToString()}");

      NetMap netMap = JsonUtility.FromJson<NetMap>(up.netMapString.ToString());
      this.Game.Map = new Map(netMap);
      this.viewManager.OnPropertyChanged(this, new PropertyChangedEventArgs("Map"));
    }

    #endregion
    private void CreateViewManager()
    {
      this.viewManager = this.gameObject.AddComponent<StandardGameViewManager>();
      this.viewManager.enabled = true;
      this.viewManager.game = this;
      this.viewManager.StartListening();
    }
    private void StartGame()
    {
      // List<RuleLogic> ruleLogics = new List<RuleLogic>();
      // ruleLogics.Add(RuleLogicMethods.DiagonalToTopRight);
    }
  }
}
