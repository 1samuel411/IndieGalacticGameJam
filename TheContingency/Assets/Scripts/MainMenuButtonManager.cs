using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuButtonManager : MonoBehaviour {

	public Animator anim;

	public GameObject MainMenu;
	public GameObject creditsMenu;

	void Start(){
		MainMenu.SetActive (true);
		creditsMenu.SetActive (false);
	}

	public void StartGame()
    {
		anim.SetBool ("closed", true);
    }

	public void CreditsScreen(){
		anim.SetBool ("closed", true);
		StartCoroutine (MainMenuGone());
	}

	public void QuitGame(){
		Application.Quit ();
	}

	public void BackToMenu(){
		anim.SetBool ("closed", true);
		StartCoroutine (CreditsMenuGone());
	}

	IEnumerator MainMenuGone(){
		yield return new WaitForSeconds (2.0f);
		MainMenu.SetActive (false);
		creditsMenu.SetActive (true);
		anim.SetBool ("closed", false);
		StopCoroutine (MainMenuGone());
	}

	IEnumerator CreditsMenuGone(){
		yield return new WaitForSeconds (2.0f);
		MainMenu.SetActive (true);
		creditsMenu.SetActive (false);
		anim.SetBool ("closed", false);
		StopCoroutine (CreditsMenuGone());
	}
}
