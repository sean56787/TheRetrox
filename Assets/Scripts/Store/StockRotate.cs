using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StockRotate : MonoBehaviour
{
    private void Update()
    {
        transform.Rotate(new Vector3(0,50,0) * Time.deltaTime);
    }
}
