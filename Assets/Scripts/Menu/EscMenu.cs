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
    
    private void Update()
    {
        if (GameStateManager.instance.isInGame) CheckEscPress();
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
    public void Resume() // 繼續遊戲
    {
        CloseEscMenu();
        GameStateManager.instance.ResumeGame();
    }

    public void Pause() // 暫停遊戲
    {
        OpenEscMenu();
        GameStateManager.instance.StopGame();
    }

    public void OpenEscMenu()
    {
        PlayerUI.instance.DisablePlayerUI();
        GameStateManager.instance.SetGamePauseState(true);
        GameStateManager.instance.UpdateCursorState();
        pauseUI.SetActive(true);
    }
    public void CloseEscMenu()
    {
        PlayerUI.instance.EnablePlayerUI();
        GameStateManager.instance.SetGamePauseState(false);
        GameStateManager.instance.UpdateCursorState();
        pauseUI.SetActive(false);
    }
    public void BackToMainMenu() // 回到主選單
    {
        Resume();
        MainMenu.instance.ActivateMainMenuselectButtonsUI();
        MainMenu.instance.OpenMainMenu();
    }
}
