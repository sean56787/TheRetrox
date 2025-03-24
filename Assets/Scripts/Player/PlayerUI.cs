using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    public static PlayerUI instance;
    public GameObject playerUIObj;
    public GameObject playerBalanceGUI;

    private void Awake()
    {
        if(instance == null) instance = this;
    }

    public void EnablePlayerUI()
    {
        playerUIObj.SetActive(true);
    }
    
    public void DisablePlayerUI()
    {
        playerUIObj.SetActive(false);
    }

    public void UpdatePlayerUI()
    {
        playerBalanceGUI.GetComponent<TextMeshProUGUI>().text = $"$ {Player.instance.balance}";
    }
}
