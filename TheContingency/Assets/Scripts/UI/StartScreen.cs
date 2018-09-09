using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartScreen : MonoBehaviour
{

    private ServerManager serverManager;

    public string username;
    public string gameId;
    public string ip;
    public string port;

    public bool enteringRoom;

    public GameObject connectedGameObject;

    public void SetName(string username)
    {
        this.username = username;
    }

    public void SetGameId(string id)
    {
        gameId = id;
    }

    public void SetIP(string ip)
    {
        this.ip = ip;
    }

    public void SetPort(string port)
    {
        this.port = port;
    }

    public void ConnectToServer()
    {
        GameObject newObj = new GameObject();
        serverManager = newObj.AddComponent<ServerManager>();

        serverManager.ip = ip;
        serverManager.port = int.Parse(port);

        serverManager.Connect(username);
    }

    public void EnterGame()
    {
        enteringRoom = true;

        MasterClientManager.instance.JoinRoom(int.Parse(gameId));

        UIManager.instance.loadingScreen.gameObject.SetActive(true);
        UIManager.instance.loadingScreen.SetText("Entering Game");
    }

    public void CreateRoom()
    {
        enteringRoom = true;

        MasterClientManager.instance.CreateRoom();

        UIManager.instance.loadingScreen.gameObject.SetActive(true);
        UIManager.instance.loadingScreen.SetText("Creating Game");
    }

    void Update()
    {
        if(serverManager != null && MasterClientManager.instance.isConnected())
        {
            connectedGameObject.SetActive(true);
        }
        else
        {
            connectedGameObject.SetActive(false);
        }

        if(enteringRoom)
        {
            if(MasterClientManager.instance.GetRoom() != null)
            {
                gameObject.SetActive(false);
                UIManager.instance.loadingScreen.gameObject.SetActive(false);
                UIManager.instance.waitingScreen.gameObject.SetActive(true);
            }
        }
    }
}
