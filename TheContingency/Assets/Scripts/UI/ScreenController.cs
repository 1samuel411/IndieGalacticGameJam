using SNetwork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenController : MonoBehaviour
{


    public GameObject aHolder;
    public GameObject bHolder;

    public GameObject[] increase;
    public GameObject[] decrease;

    public ControllerInput controllerInput;

    public Text attitudeText;
    public Text cabinPressureText;
    public Text speedText;

    void OnEnable()
    {
        controllerInput = new ControllerInput();
    }

    public void Send()
    {
        Room room = MasterClientManager.instance.GetRoom();

        if (room == null)
            return;

        for (int i = 0; i < room.usersInRoom.Count; i++)
        {
            if (room.usersInRoom[i].id == MasterClientManager.instance.getId())
            {
                if(room.usersInRoom[i].commander == false)
                {
                    MasterMessaging.instance.SendControllerInput(MasterClientManager.instance._client, controllerInput);
                }
            }
        }

        controllerInput = new ControllerInput();
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

        attitudeText.text = "Attitude (0)";
        cabinPressureText.text = "Cabin Pressure (0)";
        speedText.text = "Speed (0)";

        if (room.game.game.alert.resource.name == "Attitude")
            attitudeText.text = "Attitude (" + controllerInput.change + ")";
        if (room.game.game.alert.resource.name == "Cabin Pressure")
            cabinPressureText.text = "Cabin Pressure (" + controllerInput.change + ")";
        if (room.game.game.alert.resource.name == "Speed")
            speedText.text = "Speed (" + controllerInput.change + ")";

        for (int i = 0; i < room.usersInRoom.Count; i++)
        {
            if (room.usersInRoom[i].id == MasterClientManager.instance.getId())
            {
                // Us
                aHolder.gameObject.SetActive(false);
                bHolder.gameObject.SetActive(false);
                for (int x = 0; x < decrease.Length; x++)
                {
                    decrease[x].gameObject.SetActive(false);
                }
                for (int x = 0; x < increase.Length; x++)
                {
                    increase[x].gameObject.SetActive(false);
                }
                if (room.usersInRoom[i].a)
                {
                    aHolder.gameObject.SetActive(true);
                    for (int x = 0; x < increase.Length; x++)
                    {
                        increase[x].gameObject.SetActive(true);
                    }
                }
                else
                {
                    for(int x = 0; x <decrease.Length; x++)
                    {
                        decrease[x].gameObject.SetActive(true);
                    }
                    bHolder.gameObject.SetActive(true);
                }
            }
        }
    }

    public void IncreaseAttitude()
    {
        if(MasterClientManager.instance.GetRoom().game.game.alert.resource.name == "Attitude")
            controllerInput.change+=5;
    }

    public void DecreaseAttitude()
    {
        if(MasterClientManager.instance.GetRoom().game.game.alert.resource.name == "Attitude")
            controllerInput.change -= 5;
    }

    public void IncreasePressure()
    {
        if(MasterClientManager.instance.GetRoom().game.game.alert.resource.name == "Cabin Pressure")
            controllerInput.change++;
    }

    public void DecreasePressure()
    {
        if(MasterClientManager.instance.GetRoom().game.game.alert.resource.name == "Cabin Pressure")
            controllerInput.change--;
    }

    public void IncreaseSpeed()
    {
        if(MasterClientManager.instance.GetRoom().game.game.alert.resource.name == "Speed")
            controllerInput.change += 100;
    }

    public void DecreaseSpeed()
    {
        if(MasterClientManager.instance.GetRoom().game.game.alert.resource.name == "Speed")
            controllerInput.change -= 100;
    }
}
