using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PlayerInteractUI : MonoBehaviour
{
    public static PlayerInteractUI instance;
    public GameObject interactableUIObj;
    public TextMeshProUGUI interactableUITest;
    
    public GameObject interactableUIHintObj;
    public TextMeshProUGUI interactableUIHintText;
    public Image crossfire;
    public bool isShowingHint = false;
    private void Awake()
    {
        if(instance == null) instance = this;
    }

    private void Update()
    {
        
        if (PlayerInteract.instance.GetInteractableObj())
        {
            crossfire.color = Color.green;
            ShowInteract(PlayerInteract.instance.GetInteractableObj().GetComponent<IInteractable>());
        }
        else
        {
            crossfire.color = Color.white;
            HideInteract();
        }
        
        if (PlayerInteract.instance.holdingItem != null)
        {
            crossfire.color = new Color(255, 107, 0);
            string hint = GetUnequipHint();
            ShowHint(hint);
        }
        else
        {
            crossfire.color = Color.white;
            if(!isShowingHint) HideHint();
        }
    }

    void ShowInteract(IInteractable interactable)
    {
        interactableUIObj.SetActive(true);
        interactableUITest.text = interactable.GetInteractText();
    }
    
    void HideInteract()
    {
        interactableUIObj.SetActive(false);
        interactableUITest.text = "";
    }
    
    public void ShowHint(string hint)
    {
        interactableUIHintObj.SetActive(true);
        interactableUIHintText.text = hint;
    }
    
    public void HideHint()
    {
        interactableUIHintObj.SetActive(false);
        interactableUIHintText.text = "";
    }
    
    string GetUnequipHint()
    {
        return $"Press Q to throw {PlayerInteract.instance.holdingItem.GetComponent<IInteractable>().GetItemName()} out";
    }
    
}
