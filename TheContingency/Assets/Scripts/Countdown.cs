using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;

public class Countdown : MonoBehaviour
{
    public Text timer;
    double seconds = 0;
    double miliseconds = 0;

	void Start()
	{
        DateTime endTime = MasterClientManager.instance.GetRoom().game.game.endTime;
        TimeSpan MaxTime = endTime - DateTime.UtcNow;
        seconds = MaxTime.TotalSeconds;
        miliseconds = MaxTime.TotalMilliseconds;
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