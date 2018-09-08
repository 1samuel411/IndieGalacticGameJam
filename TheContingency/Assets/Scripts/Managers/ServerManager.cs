using SNetwork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerManager : MonoBehaviour
{

    public string ip;
    public int port;

    MasterNetworkPlayer masterNetworkPlayer;

    public void Connect(string username)
    {
        masterNetworkPlayer = new MasterNetworkPlayer();
        masterNetworkPlayer.username = username;

        MasterClientManager.instance.Connect(ip, port);

        MasterClientManager.instance.SendNetworkUser(masterNetworkPlayer);
    }

    void Update()
    {

    }
}
