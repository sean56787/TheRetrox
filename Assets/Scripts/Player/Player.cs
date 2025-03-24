using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{ 
    public static Player instance;
    public float balance;
    public bool homeUnlocked = false;
    private void Awake()
    {
        if(instance == null) instance = this;
        balance = 0;
        homeUnlocked = false;
    }

    private void Start()
    {
        PlayerUI.instance.UpdatePlayerUI();
    }
}
