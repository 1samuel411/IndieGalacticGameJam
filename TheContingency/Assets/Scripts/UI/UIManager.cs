using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    public static UIManager instance;

    public ScreenAll screenAll;
    public ScreenController screenController;
    public ScreenCommander screenCommander;
    public LoadingScreen loadingScreen;
    public StartScreen startScreen;
    public WaitingScreen waitingScreen;
    public WinScreen winScreen;

    public GameObject winAnimation;
    public GameObject loseAnimation;

    void Awake()
    {
        instance = this;
    }

    private int lastRound;
    private int lastMoney;
    void Update()
    {
        Room room = MasterClientManager.instance.GetRoom();
        if (room == null)
        {
            UIManager.instance.screenAll.gameObject.SetActive(false);
            UIManager.instance.screenCommander.gameObject.SetActive(false);
            UIManager.instance.screenController.gameObject.SetActive(false);
            UIManager.instance.startScreen.gameObject.SetActive(true);

            return;
        }

        if (room.startedGame)
        {
            if(room.game.round != lastRound && room.game.round != 0)
            {
                winScreen.ShowMe(room.game.funding - lastMoney, room.game.beatLast);
                lastRound = room.game.round;
                
            }

            if (room.game.funding != lastMoney)
            {
                lastMoney = room.game.funding;
            }
            UIManager.instance.screenAll.gameObject.SetActive(true);
            bool isCommander = false;
            for (int i = 0; i < room.usersInRoom.Count; i++)
            {
                if(room.usersInRoom[i].id == MasterClientManager.instance.getId())
                {
                    if (room.usersInRoom[i].commander)
                        isCommander = true;
                }
            }

            if(isCommander)
            {
                screenCommander.gameObject.SetActive(true);
            }
            else
            {
                screenController.gameObject.SetActive(true);
            }
            return;
        }
        else
        {
            waitingScreen.gameObject.SetActive(true);
            screenAll.gameObject.SetActive(false);
            screenController.gameObject.SetActive(false);
            screenCommander.gameObject.SetActive(false);
        }
    }

    public void FailedConnect(string issue)
    {
        Debug.Log("Failed to connect: " + issue);
        if(issue == "Full")
        {
            StartCoroutine(ShowMessage("Room is full"));
        }

        if (issue == "Not Exist")
        {
            StartCoroutine(ShowMessage("Room does not exist"));
        }
    }

    IEnumerator ShowMessage(string value)
    {
        Debug.Log("Showing Message: " + value);
        loadingScreen.gameObject.SetActive(true);
        loadingScreen.messageText.text = value;
        yield return new WaitForSeconds(1);
        loadingScreen.gameObject.SetActive(false);
        startScreen.enteringRoom = false;
    }

    public void Over()
    {
        screenController.Send();
    }

    public void Win()
    {
        waitingScreen.gameObject.SetActive(true);
        winAnimation.SetActive(true);
    }

    public void Lost()
    {
        waitingScreen.gameObject.SetActive(true);
        loseAnimation.SetActive(true);
    }

    public void PlayerLost()
    {
        waitingScreen.gameObject.SetActive(true);
    }

}
