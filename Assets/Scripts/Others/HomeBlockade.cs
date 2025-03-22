using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeBlockade : MonoBehaviour, IInteractable
{
    public string _itemName = "Blockade";
    
    public void Interact(Transform interactorTransform)
    {
        if (interactorTransform.GetComponent<Player>().balance >= House.instance.price)
        {
            interactorTransform.GetComponent<Player>().balance -= House.instance.price;
            interactorTransform.GetComponent<Player>().homeUnlocked = true;
            gameObject.SetActive(false);
        }
    }

    public string GetInteractText(PlayerInteract playerInteract)
    {
        return "Buy a House, Price: 100$";
    }

    public string GetUsage()
    {
        throw new System.NotImplementedException();
    }

    public void Use(Transform interactorTransform)
    {
        throw new System.NotImplementedException();
    }

    public string GetItemName()
    {
        return _itemName;
    }
}
