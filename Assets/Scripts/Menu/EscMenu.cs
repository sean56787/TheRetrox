using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class EscMenu : MonoBehaviour
{
    public static EscMenu instance;
    public GameObject pauseUI;
    private void Awake()
    {
        if(instance == null) instance = this;
    }

    // private void Start()
    // {
    //     StartCoroutine(WaitForGameStateManager());
    // }
    //
    // IEnumerator WaitForGameStateManager()
    // {
    //     yield return new WaitUntil(()=> GameStateManager.instance != null);
    //     CloseEscMenu();
    // }
    
    private void Update()
    {
        if (!GameStateManager.instance.inMainMenu) CheckEscPress();
    }

    void CheckEscPress()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameStateManager.instance.inGamePaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }
    public void Resume()
    {
        CloseEscMenu();
        GameStateManager.instance.ResumeGame();
    }

    public void Pause()
    {
        OpenEscMenu();
        GameStateManager.instance.StopGame();
    }

    public void OpenEscMenu()
    {
        GameStateManager.instance.SetGamePauseState(true);
        GameStateManager.instance.UpdateCursorState();
        pauseUI.SetActive(true);
    }
    public void CloseEscMenu()
    {
        GameStateManager.instance.SetGamePauseState(false);
        GameStateManager.instance.UpdateCursorState();
        pauseUI.SetActive(false);
    }
    public void BackToMainMenu()
    {
        Resume();
        GameStateManager.instance.SetIsInGameState(false);
        MainMenu.instance.ActivateMainMenuselectButtonsUI();
        MainMenu.instance.OpenMainMenu();
    }
}
