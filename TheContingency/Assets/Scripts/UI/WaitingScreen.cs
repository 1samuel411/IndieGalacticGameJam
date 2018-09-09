using SNetwork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaitingScreen : MonoBehaviour
{

    public Sprite waitingIcon;
    public Sprite readyIcon;
    public Sprite notReadyIcon;
    public Text gameDataText;

    public User[] users;

    [System.Serializable]
    public struct User
    {
        public Text userName;
        public Image statusImage;
    }

    void Start()
    {

    }

    void Update()
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        Room room = MasterClientManager.instance.GetRoom();
        if(room == null)
        {
            gameObject.SetActive(false);
            UIManager.instance.startScreen.gameObject.SetActive(true);
            return;
        }

        if(room.startedGame)
        {
            gameObject.SetActive(false);
            UIManager.instance.screenAll.gameObject.SetActive(true);
            return;
        }

        int playerNeeded = 3 - room.usersInRoom.Count;
        string myName = "";
        for (int i = 0; i < users.Length; i++)
        {
            MasterNetworkPlayer player = null;
            if(i < room.usersInRoom.Count)
                player = room.usersInRoom[i];

            if (player != null)
            {
                users[i].userName.text = player.username.ToString();

                if (player.ready)
                    users[i].statusImage.sprite = readyIcon;
                else
                    users[i].statusImage.sprite = notReadyIcon;

                if (player.id == MasterClientManager.instance.getId())
                    myName = player.username;
            }
            else
            {
                users[i].statusImage.sprite = waitingIcon;
                users[i].userName.text = "Waiting...";
            }
        }

        gameDataText.text = "Room ID: " + room.roomId + "\n\n" + "We need " + playerNeeded + " player(s)";

    }

    public void LeaveGame()
    {
        MasterClientManager.instance.LeaveRoom();
    }

    public void ToggleReady()
    {
        MasterClientManager.instance.ToggleReady();
    }
}
