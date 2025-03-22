using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SleepBag : MonoBehaviour, IInteractable
{
    public string _itemName = "SleepBag";
    public void Interact(Transform interactorTransform)
    {
        Debug.Log($"interacted with {transform.name}");
        DayNightManager.instance.OnPlayerSleep?.Invoke();
    }

    public string GetInteractText(PlayerInteract playerInteract)
    {
        return $"Press E to Sleep";
    }

    public string GetUsage()
    {
        return "none";
    }

    public void Use(Transform interactorTransform)
    {
        Debug.Log("can not use");
    }

    public string GetItemName()
    {
        return _itemName;
    }
}
