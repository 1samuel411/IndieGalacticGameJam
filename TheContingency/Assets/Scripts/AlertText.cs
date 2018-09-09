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
    }

    void Update()
    {
    }
}