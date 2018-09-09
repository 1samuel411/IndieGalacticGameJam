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

        attitudeText.text = "Attitude (" + controllerInput.attitude + ")";
        cabinPressureText.text = "Cabin Pressure (" + controllerInput.pressure + ")";
        speedText.text = "Speed (" + controllerInput.speed + ")";

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
        controllerInput.attitude+=5;
    }

    public void DecreaseAttitude()
    {
        controllerInput.attitude-=5;

    }

    public void IncreasePressure()
    {
        controllerInput.pressure++;

    }

    public void DecreasePressure()
    {
        controllerInput.pressure--;
    }

    public void IncreaseSpeed()
    {
        controllerInput.speed+=100;

    }

    public void DecreaseSpeed()
    {
        controllerInput.speed-=100;

    }
}
