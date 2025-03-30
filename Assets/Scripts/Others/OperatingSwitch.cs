using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
public class OperatingSwitch: MonoBehaviour, IInteractable
{
    public static OperatingSwitch instance;
    public GameObject[] signOutside;
    public GameObject[] signInside;
    private string _itemName;

    private void Awake()
    {
        if(instance == null) instance = this;
    }

    private void Start()
    {
        foreach (GameObject sign in signOutside)
        {
            sign.SetActive(true);
            sign.GetComponent<Light>().color = Color.red;
        }
        foreach (GameObject sign in signInside)
        {
            sign.SetActive(true);
            sign.GetComponent<Light>().color = Color.red;
        }
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

    public void SignLight()
    {
        if (GameStateManager.instance.isOperating)
        {
            foreach (GameObject sign in signOutside)
            {
                sign.GetComponent<Light>().color = Color.green;
            }
            foreach (GameObject sign in signInside)
            {
                sign.GetComponent<Light>().color = Color.green;
            }
        }
        else
        {
            foreach (GameObject sign in signOutside)
            {
                sign.GetComponent<Light>().color = Color.red;
            }
            foreach (GameObject sign in signInside)
            {
                sign.GetComponent<Light>().color = Color.red;
            }
        }
    }
}

    