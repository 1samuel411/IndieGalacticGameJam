using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using SNetwork;
using SNetwork.Client;
using UnityEngine;

public class MasterClientManager : MonoBehaviour
{

    private MasterClient _client;
    private MasterClientResponseHandler _clientResponseHandler;

    private static MasterClientManager _instance;
    public static MasterClientManager instance
    {
        get
        {
            if (_instance == null)
            {
                // create an instance
                GameObject newObject = new GameObject();
                _instance = newObject.AddComponent<MasterClientManager>();
                newObject.name = "Client Instance";
            }

            return _instance;
        }
        set { _instance = value; }
    }
    public delegate void OnMatchFound();
    public OnMatchFound onMatchFound;

    void Awake()
    {
        ResponseManager.instance.Clear();
        _client = gameObject.AddComponent<MasterClient>();

        _clientResponseHandler = new MasterClientResponseHandler(_client);

        _clientResponseHandler.Initialize();

        ResponseManager.instance.AddServerResponse(MessageResponse, 7);

        DontDestroyOnLoad(this.gameObject);
    }

    public SNetwork.MasterNetworkPlayer GetNetworkPlayer()
    {
        return _client.networkPlayers.FirstOrDefault(x => x.id == getId());
    }

    public Room GetRoom()
    {
        return _client.room;
    }

    public Socket getSocket()
    {
        return _client.clientSocket;
    }

    public bool isConnected()
    {
        return _client.IsConnectedClient();
    }

    public bool isConnecting()
    {
        return _client.connecting;
    }

    public bool isDisconnecting()
    {
        return _client.disconnecting;
    }

    public int getId()
    {
        return _client.ourId;
    }

    public object GetServerData(string key)
    {
        if (_client.serverData == null)
            return null;

        return _client.serverData.FirstOrDefault(x => x.Key.Equals(key)).Value;
    }

    public object GetClientData(string key, int client = -1)
    {
        if (client == -1)
            client = _client.ourId;
        return _client.networkPlayers.FirstOrDefault(x => x.id.Equals(client)).data.FirstOrDefault(x => x.Key.Equals(key)).Value;
    }

    public delegate void OnConnectedDelegate(ResponseMessage message);
    public OnConnectedDelegate onConnectDelegate;
    public delegate void OnCloseDelegate();
    public OnCloseDelegate onCloseDelegate;
    public delegate void OnDisconnectDelege();
    public OnDisconnectDelege onDisconnectDelegate;

    public void Connect(string ip = "", int port = 0)
    {
        _client.Connect(ip, port, OnConnected, OnClose);
    }

    public void OnConnected(ResponseMessage response)
    {
        if (onConnectDelegate != null)
            onConnectDelegate(response);
        if (response.type == ResponseMessage.ResponseType.Success)
        {
            Debug.Log("Success!");
            // Do something
        }
        else if (response.type == ResponseMessage.ResponseType.Failure)
        {
            Debug.Log("Failed!");
            // Do something
        }
        else if (response.type == ResponseMessage.ResponseType.Full)
        {
            Debug.Log("Server full!");
            // Do something
        }
    }

    public void OnClose()
    {
        if (onCloseDelegate != null)
            onCloseDelegate();

        Debug.Log("Connection Closed!");
        // Do something

        Destroy(this.gameObject);
    }

    public void Disconnect()
    {
        if (onDisconnectDelegate != null)
            onDisconnectDelegate();

        _client.Disconnect(OnDisconnected);
    }

    public void OnDisconnected()
    {
        Debug.Log("Disconnected!");

        // Destroy client
        Destroy(this.gameObject);
    }

    public void SendString(string message)
    {
        _client.SendString(message);
    }

    public void SetServerData(KeyValuePairs data)
    {
        MasterMessaging.instance.SendServerDataSetting(ByteParser.ConvertKeyValuePairToData(data), _client.clientSocket);
    }

    public void SetUserData(KeyValuePairs data)
    {
        MasterMessaging.instance.SendUserDataSetting(ByteParser.ConvertKeyValuePairToData(data), _client.ourId, _client.clientSocket);
    }

    void MessageResponse(byte[] response, Socket socket, int from)
    {
        string message = ByteParser.ConvertToASCII(response);

        if (message.Equals("Full"))
        {
            ResponseMessage messageResposne = new ResponseMessage();
            messageResposne.type = ResponseMessage.ResponseType.Full;
            OnConnected(messageResposne);
            Disconnect();
        }
    }

    public void SendNetworkUser(MasterNetworkPlayer player)
    {
        MasterMessaging.instance.SendMasterNetworkPlayer(player, 2, _client.ourId, 0, _client.clientSocket);
    }

    public void JoinRoom(int id)
    {
        MasterMessaging.instance.SendJoinRoom(id, _client);
    }

    public void CreateRoom()
    {
        MasterMessaging.instance.SendCreateRoom(_client);
    }
}
