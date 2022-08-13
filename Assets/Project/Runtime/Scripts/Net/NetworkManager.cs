
using UnityEngine;

public class NetworkManager : MonoBehaviour {
  public static NetworkManager Instance { set; get; }
  private void Awake() {
    Instance = this;
  }
  public NetworkIdentity NI { get; set; }
  public enum NetworkIdentity {
    DEDICATED_SERVER, HOST, CLIENT, LOCAL
  }
  public void Host() {
    Debug.Log(RelayNetworking.Instance);
    RelayNetworking.Instance.Host();
    this.NI = NetworkIdentity.HOST;
  }
  public void LocalGame() {
    // this.RelayNetworking = RelayNetworking.Instance;
    // this.server.Init(Constants.port);
    // this.client.Init("127.0.0.1", Constants.port);
    // this.NI = NetworkIdentity.LOCAL;
  }
  public void Connect(string joinCode) {
    RelayNetworking.Instance.Join(joinCode);
  }
}
