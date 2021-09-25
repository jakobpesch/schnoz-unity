
using Schnoz;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
  public static NetworkManager Instance { set; get; }
  private void Awake()
  {
    Instance = this;
  }
  public Server server;
  public Client client;
  public NetworkIdentity NI { get; set; }
  public enum NetworkIdentity
  {
    DEDICATED_SERVER, HOST, CLIENT
  }
  public void Host()
  {
    this.server.Init(Constants.port);
    this.client.Init("127.0.0.1", Constants.port);
    this.NI = NetworkIdentity.HOST;
  }
  public void Connect(string ip)
  {
    this.client.Init(ip, Constants.port);
  }
}
