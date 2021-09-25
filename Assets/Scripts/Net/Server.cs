using System;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

public class Server : MonoBehaviour
{
  public static Server Instance { set; get; }
  private void Awake()
  {
    Instance = this;
  }

  public NetworkDriver driver;
  private NativeList<NetworkConnection> connections;

  private bool isActive;
  public bool IsActive
  {
    get => this.isActive;
  }
  private const float keepAliveTickRate = 20.0f;
  private float lastKeepAlive;

  public Action connectionDropped;

  public void Init(ushort port)
  {
    this.driver = NetworkDriver.Create();
    NetworkEndPoint endpoint = NetworkEndPoint.AnyIpv4;
    endpoint.Port = port;

    if (this.driver.Bind(endpoint) != 0)
    {
      Debug.Log("Unable to bind on port " + endpoint.Port);
      return;
    }
    else
    {
      this.driver.Listen();
      Debug.Log("Currently listening on port " + endpoint.Port);
    }

    this.connections = new NativeList<NetworkConnection>(2, Allocator.Persistent);
    this.isActive = true;
  }

  public void Shutdown()
  {
    if (this.isActive)
    {
      this.driver.Dispose();
      this.connections.Dispose();
      this.isActive = false;
    }
  }
  private void OnDestroy()
  {
    this.Shutdown();
  }

  private void Update()
  {
    if (!this.isActive)
    {
      return;
    }
    this.KeepAlive();

    this.driver.ScheduleUpdate().Complete();
    this.CleanupConnections();
    this.AcceptNewConnections();
    this.UpdateMessagePump();
  }

  private void KeepAlive()
  {
    if (Time.time - this.lastKeepAlive > keepAliveTickRate)
    {
      this.lastKeepAlive = Time.deltaTime;
      this.Broadcast(new NetKeepAlive());
    }
  }

  private void CleanupConnections()
  {
    for (int i = 0; i < this.connections.Length; i++)
    {
      if (!this.connections[i].IsCreated)
      {
        this.connections.RemoveAtSwapBack(i);
        --i;
      }
    }
  }

  private void AcceptNewConnections()
  {
    NetworkConnection c;
    while ((c = this.driver.Accept()) != default(NetworkConnection))
    {
      this.connections.Add(c);
    }
  }

  private void UpdateMessagePump()
  {
    DataStreamReader stream;
    for (int i = 0; i < this.connections.Length; i++)
    {
      NetworkEvent.Type cmd;
      while ((cmd = this.driver.PopEventForConnection(this.connections[i], out stream)) != NetworkEvent.Type.Empty)
      {
        if (cmd == NetworkEvent.Type.Data)
        {
          NetUtility.OnData(stream, this.connections[i], this);
        }
        else if (cmd == NetworkEvent.Type.Disconnect)
        {
          Debug.Log("Client disconnected from the server");
          this.connections[i] = default(NetworkConnection);
          this.connectionDropped?.Invoke();
          this.Shutdown();
        }
      }
    }
  }

  public void SendToClient(NetworkConnection connection, NetMessage msg)
  {
    DataStreamWriter writer;
    this.driver.BeginSend(connection, out writer);
    msg.Serialize(ref writer);
    this.driver.EndSend(writer);
  }
  public void Broadcast(NetMessage msg)
  {
    for (int i = 0; i < this.connections.Length; i++)
    {
      if (this.connections[i].IsCreated)
      {
        Debug.Log($"Sending {msg.Code} to: {connections[i].InternalId}");
        SendToClient(connections[i], msg);
      }
    }
  }
}
