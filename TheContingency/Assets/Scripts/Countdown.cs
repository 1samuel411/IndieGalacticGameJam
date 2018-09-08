using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;

public class DisplayTimer : MonoBehaviour
{
    public TextMesh timer;

	void Start()
	{
        DateTime endTime = GetComponent<MasterClientManager.instance.GetRoom>();
        DateTime MaxTime = endTime - DateTime.Now.ToUniversalTime();
        seconds = MaxTime.Second;
        miliseconds = MaxTime.Millisecond;
    }

	void Update()
	{
        if (miliseconds <= 0)
        {
            if (seconds <= 0)
            {
                seconds = 59;
            }
            else if (seconds >= 0)
            {
                seconds--;
            }

            miliseconds = 100;
        }
        miliseconds -= Time.deltaTime * 100;
        timer.text = string.Format("{0}.{1}",seconds,miliseconds);
    }
}
