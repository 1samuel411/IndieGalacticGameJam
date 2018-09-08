using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;

public class DisplayTimer : MonoBehaviour
{

	public TextMesh timer;
	float second = 0;
	float milliseconds = 0;
	//float MaxTime = endTime-DateTime.ToUniversalTime();

	void Start()
	{
		//second = DateTime.ParseExact(MaxTime, "H:m:s", null);
	}

	void Update()
	{
	}
}
