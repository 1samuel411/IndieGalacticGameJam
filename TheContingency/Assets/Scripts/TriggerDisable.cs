using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDisable : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    public void DisableMe()
    {
        gameObject.SetActive(false);
    }
}
