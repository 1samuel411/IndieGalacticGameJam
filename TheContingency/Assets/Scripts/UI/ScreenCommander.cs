using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenCommander : MonoBehaviour
{

    public Text attitude;
    public Text speed;
    public Text cabinPressure;

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

        if (room == null)
            return;

        string attitudeStr = "";
        for (int i = 0; i < room.game.game.attitude.symbol.Count; i++)
        {
            attitudeStr += room.game.game.attitude.symbol[i].unicodeValue;
        }
        attitude.text = attitudeStr;

        string speedStr = "";
        for (int i = 0; i < room.game.game.speed.symbol.Count; i++)
        {
            speedStr += room.game.game.speed.symbol[i].unicodeValue;
        }
        speed.text = speedStr;

        string cabinStr = "";
        for (int i = 0; i < room.game.game.cabinPressure.symbol.Count; i++)
        {
            cabinStr += room.game.game.cabinPressure.symbol[i].unicodeValue;
        }
        cabinPressure.text = cabinStr;
    }
}
