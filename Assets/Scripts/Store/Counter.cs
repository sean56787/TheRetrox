using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Counter : MonoBehaviour
{
    public Transform checkAreaPosition;

    private void Awake()
    {
        if (checkAreaPosition == null)
        {
            Debug.LogError("checkAreaPosition is null");
        }
    }

    private void Start()
    {
        
    }
}
