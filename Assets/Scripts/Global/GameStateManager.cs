using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager instance;
    public bool inMainMenu = false;
    public bool isInGame = false;
    public bool inGamePaused = false;
    public bool isSaveLoadUIOpening = false;

    [Header("Default Game State")] 
    public Vector3 defaultPlayerPosition = new Vector3(11.5f, 1.5f, -13.0f);
    public float defaultPlayerMoney = 0f;
    public NPCSpawner[] npcSpawners;
    private void Awake()
    {
        if(instance == null) instance = this;
    }

    private void Start()
    {
        inMainMenu = true;
        isInGame = false;
        inGamePaused = false;
        isSaveLoadUIOpening = false;
        UpdateCursorState();
    }
    
    public void ResetGame()
    {
        Player.instance.transform.position = defaultPlayerPosition; // 重置玩家位置
        Player.instance.balance = defaultPlayerMoney; // 重置玩家金錢
        DayNightManager.instance.ResetTime(); // 重設時間
        ResetNPCStuffOnly();
    }

    public void ResetNPCStuffOnly()
    {
        foreach (var spawner in npcSpawners) // 刪除NPC和他們的商品
        {
            spawner.ResetAllNPCFromThisSpawner();
        }
    }
    
    public void SetMainMenuState(bool state)
    {
        inMainMenu = state;
    }
    
    public void SetGamePauseState(bool state)
    {
        inGamePaused = state;
    }
    
    public void SetIsInGameState(bool state)
    {
        isInGame = state;
    }
    
    public void SetIsSaveLoadUIOpeningState(bool state)
    {
        isSaveLoadUIOpening = state;
    }
    
    public void UpdateCursorState()
    {
        if (inMainMenu || inGamePaused || isSaveLoadUIOpening)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true; 
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void StopGame()
    {
        Time.timeScale = 0f;
    }
    
    public void ResumeGame()
    {
        Time.timeScale = 1f;
    }
}
