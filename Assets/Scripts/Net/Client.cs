using System;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

public class Client : MonoBehaviour {
  public static Client Instance { set; get; }
  private void Awake() {
    Instance = this;
  }

  public NetworkDriver driver;
  private NetworkConnection connection;

  private bool isActive;

  public Action connectionDropped;

  public void Init(string ip, ushort port) {
    this.driver = NetworkDriver.Create();
    NetworkEndPoint endpoint = NetworkEndPoint.Parse(ip, port);

    this.connection = this.driver.Connect(endpoint);

    this.isActive = true;
    RegisterToEvent();
  }

  public void Shutdown() {
    if (this.isActive) {
      UnregisterToEvent();
      this.driver.Dispose();
      this.isActive = false;
      this.connection = default(NetworkConnection);
    }
  }
  private void OnDestroy() {
    this.Shutdown();
  }

  private void Update() {
    if (!this.isActive) {
      return;
    }

    this.driver.ScheduleUpdate().Complete();
    this.CheckAlive();

    this.UpdateMessagePump();
  }

  private void CheckAlive() {
    if (!this.connection.IsCreated && this.isActive) {
      Debug.Log("Something went wrong, lost connection to server");
      this.connectionDropped?.Invoke();
      this.Shutdown();
    }
  }

  private void UpdateMessagePump() {
    DataStreamReader stream;
    NetworkEvent.Type cmd;
    while ((cmd = connection.PopEvent(this.driver, out stream)) != NetworkEvent.Type.Empty) {
      if (cmd == NetworkEvent.Type.Connect) {
        this.SendToServer(new NetWelcome());
        Debug.Log("We're connected!");
      } else if (cmd == NetworkEvent.Type.Data) {
        NetUtility.OnData(stream, default(NetworkConnection));
      } else if (cmd == NetworkEvent.Type.Disconnect) {
        Debug.Log("Client got disconnected from the server");
        this.connection = default(NetworkConnection);
        this.connectionDropped?.Invoke();
        this.Shutdown();
      }

    }
  }

  public void SendToServer(NetMessage msg) {
    DataStreamWriter writer;
    this.driver.BeginSend(this.connection, out writer);
    msg.Serialize(ref writer);
    this.driver.EndSend(writer);
  }
  public void SendToClient(NetworkConnection connection, NetMessage msg) {
    DataStreamWriter writer;
    this.driver.BeginSend(connection, out writer);
    msg.Serialize(ref writer);
    this.driver.EndSend(writer);
  }
  private void RegisterToEvent() {
    NetUtility.C_KEEP_ALIVE += OnKeepAlive;
  }
  private void UnregisterToEvent() {
    NetUtility.C_KEEP_ALIVE -= OnKeepAlive;
  }
  private void OnKeepAlive(NetMessage nm) {
    this.SendToServer(nm);
  }
}
