using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenAll : MonoBehaviour
{

    public Text alertText;

    public GameObject endedGame;

    public Text timerText;

    public Text fundingText;
    public Image fundingFG;

    public Text roundText;
    public Image roundFG;

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
            return;
        }

        if ((room.game.round >= 7 || room.game.funding < 0) && room.game.ended)
            endedGame.gameObject.SetActive(true);
        else
            endedGame.gameObject.SetActive(false);

        if (room.game.game.alert != null)
            alertText.text = "Set " + room.game.game.alert.resource.name + " to " + room.game.game.alert.targetResourceValue;

        fundingText.text = "Funding: $" + room.game.funding;
        fundingFG.fillAmount = (float)room.game.funding / 100000;

        roundText.text = "Round: " + (room.game.round + 1) + " / 7";

        roundFG.fillAmount = (float)(room.game.round + 1) / 7;

        TimeSpan diff = room.game.game.endTime - DateTime.UtcNow;
        double seconds = Mathf.Clamp(diff.Seconds, 0, 60);
        double milliseconds = Mathf.Clamp(diff.Milliseconds, 0, 999);
        timerText.text = seconds + ":" + milliseconds;

        if ((room.game.round >= 7 || room.game.funding < 0) && room.game.ended)
        {
            timerText.text = "";
        }
    }

    public void LeaveGame()
    {
        MasterClientManager.instance.LeaveRoom();
    }
}
