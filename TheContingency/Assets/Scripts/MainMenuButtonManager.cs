using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuButtonManager : MonoBehaviour
{

    public Animator anim;

    public GameObject MainMenu;
    public GameObject creditsMenu;
    public GameObject howToPlay;

    void Start()
    {
#if UNITY_STANDALONE
        
#endif
        Screen.SetResolution(320, 480, false);
        MainMenu.SetActive(true);
        creditsMenu.SetActive(false);
    }

    public void StartGame()
    {
        anim.SetBool("closed", true);
        StartCoroutine(PlayGame());
    }

    public void CreditsScreen()
    {
        anim.SetBool("closed", true);
        StartCoroutine(MainMenuGone());
    }

    public void HowToPlay()
    {
        anim.SetBool("closed", true);
        StartCoroutine(HowToPlayCoroutine());
    }

    public void BackToMenu()
    {
        anim.SetBool("closed", true);
        StartCoroutine(CreditsMenuGone());
    }

    IEnumerator MainMenuGone()
    {
        yield return new WaitForSeconds(1.4f);
        MainMenu.SetActive(false);
        creditsMenu.SetActive(true);
        anim.SetBool("closed", false);
        StopCoroutine(MainMenuGone());
    }


    IEnumerator HowToPlayCoroutine()
    {
        yield return new WaitForSeconds(1.4f);
        MainMenu.SetActive(false);
        creditsMenu.SetActive(false);
        howToPlay.SetActive(true);
        anim.SetBool("closed", false);
        StopCoroutine(MainMenuGone());
    }

    IEnumerator PlayGame()
    {
        yield return new WaitForSeconds(1.4f);
        SceneManager.LoadScene("Game");
        anim.SetBool("closed", false);
        StopCoroutine(MainMenuGone());
    }

    IEnumerator CreditsMenuGone()
    {
        yield return new WaitForSeconds(1.4f);
        MainMenu.SetActive(true);
        howToPlay.SetActive(false);
        creditsMenu.SetActive(false);
        anim.SetBool("closed", false);
        StopCoroutine(CreditsMenuGone());
    }
}
