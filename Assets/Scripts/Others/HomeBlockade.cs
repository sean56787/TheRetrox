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
            StartCoroutine(SoundManager.instance.PlayClip_BuyHouse());
            interactorTransform.GetComponent<Player>().balance -= House.instance.price;
            PlayerUI.instance.UpdatePlayerUI();
            interactorTransform.GetComponent<Player>().homeUnlocked = true;
            gameObject.SetActive(false);
        }
    }

    public string GetInteractText()
    {
        return $"Press E To Buy a House, Price: <color=green>{House.instance.price}$</color>";
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
