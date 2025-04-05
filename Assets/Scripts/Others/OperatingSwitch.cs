using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
public class OperatingSwitch: MonoBehaviour, IInteractable
{
    public static OperatingSwitch instance;
    public GameObject signOutside;
    public GameObject signInside;
    public GameObject lightOutside;
    public GameObject lightInside;
    private string _itemName;
    private Material _matOutside;
    private Material _matInside;
    private void Awake()
    {
        if(instance == null) instance = this;
    }

    private void Start()
    {
        _matOutside = signOutside.GetComponent<Renderer>().material;
        _matInside = signInside.GetComponent<Renderer>().material;
        _matOutside.DisableKeyword("_EMISSION");
        _matInside.DisableKeyword("_EMISSION");
        
        lightOutside.SetActive(true);
        lightOutside.GetComponent<Light>().color = Color.red;
        
        lightInside.SetActive(true);
        lightInside.GetComponent<Light>().color = Color.red;
        
    }

    public void Interact(Transform interactorTransform)
    {
        Debug.Log("switch");
        StartCoroutine(SoundManager.instance.PlayClip_PressSwitch());
        GameStateManager.instance.isOperating = !GameStateManager.instance.isOperating;
        SignLight();
    }

    public string GetInteractText()
    {
        if(!GameStateManager.instance.isOperating) return $"Press E to <color=green>OPEN</color> store";
        else return $"Press E to <color=red>CLOSE</color> store";
    }

    public string GetUsage()
    {
        return "A Switch to Open Store";
    }

    public void Use(Transform interactorTransform)
    {
        
    }

    public string GetItemName()
    {
        return _itemName;
    }

    public void SignLight() // 招牌燈光
    {
        if (GameStateManager.instance.isOperating)
        {
            _matOutside = signOutside.GetComponent<Renderer>().material;
            _matInside = signInside.GetComponent<Renderer>().material;
            _matOutside.EnableKeyword("_EMISSION");
            _matInside.EnableKeyword("_EMISSION");
            _matOutside.SetColor("_EmissionColor", Color.green);
            _matInside.SetColor("_EmissionColor", Color.green);
            
            lightOutside.GetComponent<Light>().color = Color.green;
            lightInside.GetComponent<Light>().color = Color.green;
        }
        else
        {
            _matOutside = signOutside.GetComponent<Renderer>().material;
            _matInside = signInside.GetComponent<Renderer>().material;
            _matOutside.DisableKeyword("_EMISSION");
            _matInside.DisableKeyword("_EMISSION");
            lightOutside.GetComponent<Light>().color = Color.red;
            lightInside.GetComponent<Light>().color = Color.red;
        }
    }
}

    