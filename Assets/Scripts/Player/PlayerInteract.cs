using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PlayerInteract : MonoBehaviour
{
    public static PlayerInteract instance;
    public Camera playerCam;
    public float interactRange = 3f;
    public LayerMask interactableLayer;
    private IInteractable _currentInteractable;
    public GameObject holdingItem;
    public float throwRange = 5f;
    public float throwPower = 5f;
    private void Awake()
    {
        if(instance == null) instance = this;
    }

    private void Update()
    {
        _currentInteractable = GetInteractableObj()?.GetComponent<IInteractable>();
        if (Input.GetKeyDown(KeyCode.E) && _currentInteractable != null)
        {
            _currentInteractable.Interact(this.gameObject.transform);
        }
        
        if (Input.GetKeyDown(KeyCode.F) && holdingItem) //使用手中物品
        {
            holdingItem.GetComponent<IInteractable>().Use(this.gameObject.transform);
        }
        else if (Input.GetKeyDown(KeyCode.Q) && holdingItem) // 丟棄 
        {
            ThrowHolingItem();
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
    
    void ThrowHolingItem()
    {
        GameObject lastHoldingItem = holdingItem; //先存起來
        holdingItem.GetComponent<IInteractable>().Interact(this.gameObject.transform); // 再解除
        Rigidbody itemRb = lastHoldingItem.GetComponent<Rigidbody>();
        itemRb.isKinematic = false;
        Vector3 dirToThrow = ((playerCam.transform.position + playerCam.transform.forward * throwRange) -
                              lastHoldingItem.transform.position).normalized;
        StartCoroutine(SoundManager.instance.PlayClip_PlayerThrow());
        itemRb.AddForce(dirToThrow * throwPower, ForceMode.Impulse);
    } 
}
