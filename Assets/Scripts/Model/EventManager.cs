
using System;
using Schnoz;
using UnityEngine;

namespace Schnoz
{
  public class ClickCardEventArgs
  {
    public Card Card;
    public ClickCardEventArgs(Card card)
    {
      this.Card = card;
    }
  }
  public class ClickTileEventArgs
  {
    public Tile Tile;
    public ClickTileEventArgs(Tile tile)
    {
      this.Tile = tile;
    }
  }
  public sealed class EventManager
  {
    private EventManager() { }
    private static readonly Lazy<EventManager> lazy = new Lazy<EventManager>(() => new EventManager());
    public static EventManager I { get { return lazy.Value; } }
    public event EventHandler<GameSettings> OnStartGame;
    public Action OnAllPlayersPresent;
    public event EventHandler<Player> OnStartTurn, OnEndTurn;
    public event EventHandler<Card> OnDrawCard, OnRotate, OnMirror, OnSelectCard;
    public event EventHandler<ClickCardEventArgs> OnClickCard;
    public event EventHandler<ClickTileEventArgs> OnClickTile;
    public event EventHandler<Player> OnSpaceButton;
    public event EventHandler<Player> OnAbort;
    public void StartGame(object sender, GameSettings settings)
    {
      // Debug.Log("EventManger:StartGame() Sender: " + sender);
      if (OnStartGame != null)
        OnStartGame(sender, settings);
    }
    public void StartTurn(object sender, Player player)
    {
      // Debug.Log("EventManger:StartTurn() Sender: " + sender + ". It's " + player.playerName + "'s turn.");
      if (OnStartTurn != null)
        OnStartTurn(sender, player);
    }
    public void EndTurn(object sender, Player player)
    {
      // Debug.Log("EventManger:EndTurn() Sender: " + sender);
      if (OnEndTurn != null)
        OnEndTurn(sender, player);
    }
    public void DrawCard(object sender, Card card)
    {
      // Debug.Log("EventManger:DrawCard() Sender: " + sender);
      if (OnDrawCard != null)
        OnDrawCard(sender, card);
    }
    public void ClickCard(object sender, ClickCardEventArgs e)
    {
      // Debug.Log("EventManger:ClickCard() Sender: " + sender);
      if (OnClickCard != null)
        OnClickCard(sender, e);
    }
    public void ClickTile(object sender, ClickTileEventArgs e)
    {
      // Debug.Log("EventManger:ClickTile() Sender: " + sender);
      if (OnClickTile != null)
        OnClickTile(sender, e);
    }
    public void SelectCard(object sender, Card card)
    {
      // Debug.Log("EventManger:ClickCard() Sender: " + sender);
      if (OnSelectCard != null)
        OnSelectCard(sender, card);
    }
    public void AllPlayersPresent()
    {
      // Debug.Log("EventManger:AllPlayersPresent()");
      if (OnAllPlayersPresent != null)
        OnAllPlayersPresent();
    }
    public void SpaceButton(object sender, Player player)
    {
      // Debug.Log("EventManager:SpaceButton() Sender: " + sender + ". Player " + player.playerName + " pressed the space bar.");
      if (OnSpaceButton != null)
        OnSpaceButton(sender, player);
    }
    public void Abort(object sender, Player player)
    {
      // Debug.Log("EventManager:Abort() Sender: " + sender + ". Player " + player.playerName + " pressed the secondary mouse button.");
      if (OnAbort != null)
        OnAbort(sender, player);
    }
  }

}

