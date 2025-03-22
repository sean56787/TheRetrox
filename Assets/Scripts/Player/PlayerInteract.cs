using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInteract : MonoBehaviour
{
    public Camera playerCam;
    public float interactRange = 3f;
    public LayerMask interactableLayer;
    private IInteractable _currentInteractable;
    public GameObject isHolding;
    private void Update()
    {
        _currentInteractable = GetInteractableObj()?.GetComponent<IInteractable>();
        if (Input.GetKeyDown(KeyCode.E) && _currentInteractable != null)
        {
            _currentInteractable.Interact(this.gameObject.transform);
        }
        
        if (Input.GetKeyDown(KeyCode.F) && isHolding) //使用手中物品
        {
            isHolding.GetComponent<IInteractable>().Use(this.gameObject.transform);
        }
        else if (Input.GetKeyDown(KeyCode.Q) && isHolding) // 丟棄 
        {
            isHolding.GetComponent<IInteractable>().Interact(this.gameObject.transform);
        }
    }

    public GameObject GetInteractableObj()
    {
        Ray ray = new Ray(playerCam.transform.position, playerCam.transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, interactRange, interactableLayer))
        {
            if (hit.transform.GetComponent<IInteractable>() != null)
            {
                return hit.transform.gameObject;
            }
        }
        return null;
    }
}
