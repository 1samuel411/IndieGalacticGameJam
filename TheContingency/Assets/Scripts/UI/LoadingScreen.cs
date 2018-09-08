using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{

    public Text messageText;

    void Start()
    {
        messageText.text = "";
    }

    public void SetText(string text)
    {
        messageText.text = text;
    }
}
