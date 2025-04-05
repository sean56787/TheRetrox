using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData
{
    public float playerMoney;
    public float[] playerPosition;
    public float gameCurrentTime;
    public string saveTime;
    public bool isHomeUnlocked;
    public GameData(Player player) // 存檔系統用
    {
        playerMoney = player.balance;
        playerPosition = new float[3]
            { player.transform.position.x, player.transform.position.y, player.transform.position.z };
        gameCurrentTime = DayNightManager.instance.currentTime;
        saveTime = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
        isHomeUnlocked = player.homeUnlocked;
    }
    
}
