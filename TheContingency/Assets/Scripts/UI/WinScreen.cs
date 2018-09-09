using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinScreen : MonoBehaviour
{

    public CanvasGroup layoutGroup;
    public GameObject won;
    public GameObject lost;
    public Text diffText;
    public bool fadeAway = false; 

    void Start()
    {

    }

    public void ShowMe(int diff, bool wonb)
    {
        fadeAway = false;
        layoutGroup.alpha = 1;
        StartCoroutine(Wait());
        diffText.text = diff.ToString();
        won.SetActive(false);
        lost.SetActive(false);
        if (wonb)
            won.SetActive(true);
        else
            lost.SetActive(true);
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(0.5f);
        fadeAway = true;
    }

    void Update()
    {
        if(fadeAway)
        {
            layoutGroup.alpha -= 2f * Time.deltaTime;
        }
    }
}
