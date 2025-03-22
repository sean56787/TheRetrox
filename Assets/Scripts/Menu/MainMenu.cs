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
    public GameObject playerCamera;
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
        OpenMainMenu();
    }

    public void OpenMainMenu()
    {
        GameStateManager.instance.SetMainMenuState(true);
        GameStateManager.instance.UpdateCursorState();
        mainMenu.SetActive(true);
    }
    
    public void CloseMainMenu()
    {
        GameStateManager.instance.SetMainMenuState(false);
        GameStateManager.instance.UpdateCursorState();
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
    public void StartNewGame()
    {
        Debug.Log("StartNewGame");
        // turn off main menu
        CloseMainMenu();
        // Reset All vars
        GameStateManager.instance.ResetGame();
        GameStateManager.instance.SetIsInGameState(true);
        EscMenu.instance.Resume();
    }
    
    public void ExitGame()
    {
        Debug.Log("ExitGame");
    }
}
