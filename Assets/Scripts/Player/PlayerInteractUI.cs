using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PlayerInteractUI : MonoBehaviour
{
    public PlayerInteract playerInteract;
    public GameObject interactableTextObj;
    public TextMeshProUGUI interactTextMeshProUGUI;
    public Image crosshair;
    private void Update()
    {
        if (playerInteract.GetInteractableObj())
        {
            crosshair.color = Color.green;
            Show(playerInteract.GetInteractableObj().GetComponent<IInteractable>());
        }
        else
        {
            crosshair.color = Color.white;
            Hide();
        }
    }

    private void Show(IInteractable interactable)
    {
        interactableTextObj.SetActive(true);
        interactTextMeshProUGUI.text = interactable.GetInteractText(playerInteract);
    }

    private void Hide()
    {
        interactableTextObj.SetActive(false);
    }
}
