using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class MainMenu : MonoBehaviour
{
    
    public static MainMenu instance;
    public GameObject mainMenu;
    public GameObject selectButtonsUI;
    private void Awake()
    {
        if(instance == null) { instance = this; }
    }

    private void Start()
    {
        StartCoroutine(WaitForGameStateManager());
    }
    
    IEnumerator WaitForGameStateManager()
    {
        yield return new WaitUntil(()=> GameStateManager.instance != null);
        PlayerUI.instance.DisablePlayerUI();
        ActivateMainMenuselectButtonsUI();
        OpenMainMenu();
    }

    public void OpenMainMenu()
    {
        GameStateManager.instance.SetIsInGameState(false);
        GameStateManager.instance.SetMainMenuState(true);
        GameStateManager.instance.UpdateCursorState();
        PlayerUI.instance.DisablePlayerUI();
        mainMenu.SetActive(true);
    }
    
    public void CloseMainMenu()
    {
        GameStateManager.instance.SetIsInGameState(true);
        GameStateManager.instance.SetMainMenuState(false);
        GameStateManager.instance.UpdateCursorState();
        PlayerUI.instance.EnablePlayerUI();
        mainMenu.SetActive(false);
    }
    
    public void ActivateMainMenuselectButtonsUI()
    {
        selectButtonsUI.SetActive(true);
    }
    
    public void DeActivateMainMenuselectButtonsUI()
    {
        selectButtonsUI.SetActive(false);
    }
    
    public void StartNewGame() // 開始新遊戲
    {
        // turn off main menu
        CloseMainMenu();
        // Reset All vars
        GameStateManager.instance.ResetGame();
        GameStateManager.instance.SetIsInGameState(true);
        EscMenu.instance.Resume();
    }
    
    public void ExitGame()
    {
        Application.Quit(); // 關閉遊戲
    }
}
