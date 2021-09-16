
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
  public Server server;
  public Client client;
  public void OnOnlineHostButton()
  {
    this.server.Init(8007);
    this.client.Init("127.0.0.1", 8007);
  }
  public void OnOnlineConnectButton()
  {
    this.client.Init("127.0.0.1", 8007);
  }
}
