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

    }

	void Update()
	{
        DateTime endTime = MasterClientManager.instance.GetRoom().game.game.endTime;
        TimeSpan MaxTime = endTime - DateTime.UtcNow;
        seconds = MaxTime.TotalSeconds;
        seconds = Mathf.Clamp((float)seconds,0,9999);
        miliseconds = MaxTime.TotalMilliseconds;
        miliseconds = Mathf.Clamp((float)miliseconds, 0, 9999);

        timer.text = string.Format("{0}.{1}",seconds,miliseconds);
    }
}