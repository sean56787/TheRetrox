using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class House : MonoBehaviour
{
    public float price = 150;
    public static House instance;

    private void Awake()
    {
        if (instance == null) { instance = this; }
    }
}
