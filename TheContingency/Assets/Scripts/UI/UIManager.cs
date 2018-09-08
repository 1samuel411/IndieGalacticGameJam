using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    public static UIManager instance;

    public ScreenAll screenAll;
    public ScreenController screenController;
    public ScreenCommander screenCommander;
    public LoadingScreen loadingScreen;
    public StartScreen startScreen;
    public WaitingScreen waitingScreen;

    void Awake()
    {
        instance = this;
    }

    void Update()
    {

    }
}
