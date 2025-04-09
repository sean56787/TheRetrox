using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using Random = UnityEngine.Random;


public class NPCPersonality : MonoBehaviour
{
    public enum Personality
    {
        Normal,     //正常
        Shopaholic, //購物狂
        Thrifty,    //節儉的
        InHurry,    //很急、暴怒者
        Sloth,      //懶人
        Drinker,    //飲料狂魔
        Fruiter,    //健康達人
        Snacker,    //零食達人
    }
    public Personality personality;
    
    private void Awake() //初始化變數、載入資源、註冊事件 在物件被載入時執行（即使物件未啟用）。
    {
        personality = GetRandomPersonality<Personality>();
    }
    
    private static T GetRandomPersonality<T>() where T : Enum // 隨機個性
    {
        Array values = Enum.GetValues(typeof(T));
        int randomIndex = Random.Range(0, values.Length);
        return (T)values.GetValue(randomIndex);
    }
}
