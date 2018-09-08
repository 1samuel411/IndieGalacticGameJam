using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Alert : MonoBehaviour 
{
	
	public Text alertText;
	
    void Start ()
    {
        string[] alertArray1 = {"air pressure","attitude","velocity"};
		string[] alertArray2 = {"low","high"};
		int rand1 = UnityEngine.Random.Range(0,alertArray1.Length);
		int rand2 = UnityEngine.Random.Range(0,alertArray2.Length);
		
		string alertDisplay = "Your " + alertArray1[rand1] + " is "+alertArray2[rand2]+". Make a correction!";
		Debug.Log(alertDisplay);
		alertText.text = alertDisplay;
    }
}