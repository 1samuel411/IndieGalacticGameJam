using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaitingScreen : MonoBehaviour
{

    public Sprite waitingIcon;
    public Sprite readyIcon;
    public Sprite notReadyIcon;

    [System.Serializable]
    public struct User
    {
        public Text userName;
        public Image statusImage;
    }
    public User 

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

        for(int i = 0; i < room.usersInRoom.Count; i++)
        {
            
        }
    }
}
