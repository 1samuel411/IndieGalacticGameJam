using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using PirateGame.Managers;
using Sirenix.Utilities;
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
        _client.clientSettings = Resources.Load<MasterClientSettingsScriptableObject>(MasterClientSettingsScriptableObject.location);
        if (_client.clientSettings == null)
        {
            instance = null;
            Destroy(this.gameObject);
            throw new Exception("ClientSettings Missing... Go to Tools/SNetworking/Client Settings to create and configure ClientSettings");
        }
        _clientResponseHandler = new MasterClientResponseHandler(_client);

        _clientResponseHandler.Initialize();

        ResponseManager.instance.AddServerResponse(MessageResponse, 7);

        DontDestroyOnLoad(this.gameObject);
    }

    public MasterClientSettingsScriptableObject GetClientSettings()
    {
        return _client.clientSettings;
    }

    public SNetwork.MasterNetworkPlayer GetNetworkPlayer()
    {
        return _client.networkPlayers.FirstOrDefault(x => x.id == getId());
    }

    public Room GetRoom()
    {
        return _client.room;
    }

    public Invite[] GetInvites()
    {
        return _client.invites;
    }

    public Socket getSocket()
    {
        return _client.clientSocket;
    }

    public Match getMatch()
    {
        return _client.match;
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

    public void SendInvite(string playfabIdTarget)
    {
        if (playfabIdTarget.IsNullOrWhitespace())
            return;

        MasterMessaging.instance.SendInvite(playfabIdTarget, 2, getId(), 0, _client.clientSocket);
    }

    public void SendInviteAccept(int inviteId)
    {
        MasterMessaging.instance.SendAccept(inviteId, 2, getId(), 0, _client.clientSocket);
    }

    public void SendInviteDecline(int inviteId)
    {
        MasterMessaging.instance.SendDecline(inviteId, 2, getId(), 0, _client.clientSocket);
    }

    public void SendLeave()
    {
        MasterMessaging.instance.SendLeave(2, getId(), 0, _client.clientSocket);
    }

    public void SendMatchMake()
    {
        MasterMessaging.instance.SendMatchMake(2, getId(), 0, _client.clientSocket);
    }

    public void CancelMatchMake()
    {
        MasterMessaging.instance.SendMatchMakeLeave(2, getId(), 0, _client.clientSocket);
    }

    public void SendNewPlayMode(int playMode)
    {
        MasterMessaging.instance.SendNewPlayMode(playMode, 2, getId(), 0, _client.clientSocket);
    }

    public delegate void OnConnectedDelegate(ResponseMessage message);
    public OnConnectedDelegate onConnectDelegate;
    public delegate void OnCloseDelegate();
    public OnCloseDelegate onCloseDelegate;
    public delegate void OnDisconnectDelege();
    public OnDisconnectDelege onDisconnectDelegate;

    public void Connect(string ip = "", int port = 0)
    {
        if (ip.IsNullOrWhitespace())
        {
            if (PlayerManager.instance.region == "NA")
                ip = _client.clientSettings.ipAddressNA;
            else if (PlayerManager.instance.region == "SA")
                ip = _client.clientSettings.ipAddressSA;
            else if (PlayerManager.instance.region == "AS")
                ip = _client.clientSettings.ipAddressAS;
            else if (PlayerManager.instance.region == "EU")
                ip = _client.clientSettings.ipAddressEU;
            else if (PlayerManager.instance.region == "AU")
                ip = _client.clientSettings.ipAddressAU;
            else
                ip = _client.clientSettings.ipAddressNA;
        }
        if (port <= 0)
        {
            if (PlayerManager.instance.region == "NA")
                port = _client.clientSettings.portNA;
            else if (PlayerManager.instance.region == "SA")
                port = _client.clientSettings.portSA;
            else if (PlayerManager.instance.region == "AS")
                port = _client.clientSettings.portAS;
            else if (PlayerManager.instance.region == "EU")
                port = _client.clientSettings.portEU;
            else if (PlayerManager.instance.region == "AU")
                port = _client.clientSettings.portAU;
            else
                port = _client.clientSettings.portNA;

        }
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
}
