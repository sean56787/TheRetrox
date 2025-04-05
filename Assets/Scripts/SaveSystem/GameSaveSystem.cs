using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameSaveSystem : MonoBehaviour
{
    public GameObject saveloadUI;
    static GameSaveSystem _instance;
    private string _savePath;
    public GameObject saveSlots;
    public GameObject loadSlots;
    public TextMeshProUGUI[] saveTimes;
    public TextMeshProUGUI[] moneyTexts;
    public GameObject homeBlockade;
    private void Awake()
    {
        if (_instance == null) _instance = this;
    }
    
    private void Start()
    {
        saveloadUI.SetActive(false);
        CheckSaveDir();                 // 先檢查預設存檔位置: C:\Users\名字\AppData\LocalLow\sean56787\One Receipt Away
        UpdateSaveSlots();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && GameStateManager.instance.isSaveLoadUIOpening)
        {
            CloseSaveLoadMenu();
            if (GameStateManager.instance.inMainMenu)
            {
                MainMenu.instance.ActivateMainMenuselectButtonsUI();
                MainMenu.instance.OpenMainMenu();
            }
            else if (GameStateManager.instance.inGamePaused)
            {
                EscMenu.instance.OpenEscMenu();
                EscMenu.instance.Pause();
            }
        }
    }
    
    public void SaveGame(int slot) // 儲存遊戲
    {
        string filePath = $"{_savePath}/save_0{slot}.json";
        Debug.Log($"going to write {filePath}");
        GameData data = new GameData(Player.instance);
        string jsData = JsonUtility.ToJson(data, true);
        File.WriteAllText(filePath, jsData);
        UpdateSaveSlots();
    }
    
    public void LoadGame(int slot) // 讀取遊戲
    {
        if(!IsGameStateManagerAvailable()) Debug.LogError("NO GSM");
        string filePath = $"{_savePath}/save_0{slot}.json";
        if (File.Exists(filePath))
        {
            if (GameStateManager.instance.inMainMenu) // if從主頁面加載
            {
                MainMenu.instance.CloseMainMenu();
            }
            //GameStateManager.instance.ResetGame();
            GameStateManager.instance.SetIsInGameState(true);
            LoadDataToGame();
            CloseSaveLoadMenu();
            EscMenu.instance.Resume();
        }
        void LoadDataToGame()
        {
            string jsData = File.ReadAllText(filePath);
            GameData data = JsonUtility.FromJson<GameData>(jsData);
            Player.instance.homeUnlocked = data.isHomeUnlocked;
            Player.instance.balance = data.playerMoney;
            PlayerUI.instance.UpdatePlayerUI();
            Player.instance.transform.position = new Vector3(data.playerPosition[0], data.playerPosition[1], data.playerPosition[2]);
            homeBlockade.SetActive(!data.isHomeUnlocked);
            DayNightManager.instance.currentTime = data.gameCurrentTime;
        }
    }
    
    void UpdateSaveSlots() // 更新檔案
    {
        for (int i = 0; i < 3; i++)
        {
            string filePath = $"{_savePath}/save_0{i}.json";
            if (File.Exists(filePath))
            {
                GameData data = JsonUtility.FromJson<GameData>(File.ReadAllText(filePath));
                saveTimes[i].text = data.saveTime;
                moneyTexts[i].text = "$" + data.playerMoney;
            }
            else
            {
                saveTimes[i].text = "Empty Slot";
                moneyTexts[i].text = "$???";
            }
        }
    }
    
    public void OpenSaveLoadMenu(int mode)
    {
        if(!IsGameStateManagerAvailable()) Debug.LogError("NO GSM");
        UpdateSaveSlots();
        PlayerUI.instance.DisablePlayerUI();
        if (GameStateManager.instance.inGamePaused)
        {
            EscMenu.instance.CloseEscMenu();
        }
        else if (GameStateManager.instance.inMainMenu)
        {
            MainMenu.instance.DeActivateMainMenuselectButtonsUI();
        }
        else
        {
            Debug.LogError("error");
        }
        GameStateManager.instance.SetIsSaveLoadUIOpeningState(true);
        GameStateManager.instance.UpdateCursorState();
        saveloadUI.SetActive(true);
        switch (mode)
        {
            case 0:
                saveSlots.SetActive(true);
                loadSlots.SetActive(false);
                break;
            case 1:
                saveSlots.SetActive(false);
                loadSlots.SetActive(true);
                break;
            default:
                Debug.LogError("wrong mode");
                break;
        }
    }
    
    public void CloseSaveLoadMenu()
    {
        if(!IsGameStateManagerAvailable()) Debug.LogError("NO GSM");
        PlayerUI.instance.EnablePlayerUI();
        GameStateManager.instance.SetIsSaveLoadUIOpeningState(false);
        GameStateManager.instance.UpdateCursorState();
        saveloadUI.SetActive(false);
    }

    bool IsGameStateManagerAvailable()
    {
        return !(GameStateManager.instance == null);
    }

    void CheckSaveDir()
    {
        _savePath = Path.Combine(Application.persistentDataPath, "GameSave");
        if (!Directory.Exists(_savePath))
        {
            Directory.CreateDirectory(_savePath);
        }
    }
}
