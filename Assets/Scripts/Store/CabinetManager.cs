using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CabinetManager : MonoBehaviour
{
    public static CabinetManager instance;
    private Dictionary<string, ProductCabinet> _cabinetsDic = new();

    private void Awake()
    {
        if(instance == null) instance = this;
    }

    public void RegisterCabinet(ProductCabinet productCabinet)
    {
        if (!_cabinetsDic.ContainsKey(productCabinet.cabinetCategory))
        {
            _cabinetsDic.Add(productCabinet.cabinetCategory, productCabinet);
        }
    }

    public ProductCabinet GetCabinet(string category)
    {
        return _cabinetsDic.ContainsKey(category) ? _cabinetsDic[category] : null;
    }
}
