using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AlertText : MonoBehaviour 
{
	public Text alertText;

    void Start ()
    {
        Resource resource = MasterClientManager.instance.GetRoom().game.game.alert.resource;
        int targetResourceValue = MasterClientManager.instance.GetRoom().game.game.alert.targetResourceValue;
        string alertDisplay = "Set the " + resource + "to " + targetResourceValue;
        alertText.text = alertDisplay;
    }

    void Update()
    {
    }
}