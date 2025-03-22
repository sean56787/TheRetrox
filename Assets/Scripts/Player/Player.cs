using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{ 
    public GameObject BalanceGUI;
    public static Player instance;
    public float balance;
    public bool homeUnlocked = false;
    private void Awake()
    {
        if(instance == null) instance = this;
        BalanceGUI.SetActive(true);
        balance = 0;
        homeUnlocked = false;
    }

    private void Update()
    {
        BalanceGUI.GetComponent<TextMeshProUGUI>().text = $"Balance: {balance}";
        // if (Input.GetKeyDown(KeyCode.F5)) SaveGame();
        // if (Input.GetKeyDown(KeyCode.F9)) LoadGame();
    }
     
}
