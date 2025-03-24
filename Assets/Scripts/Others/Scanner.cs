using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Scanner : MonoBehaviour, IInteractable
{
    public Transform playerCameraTransform;
    public bool isHolding = false;
    private string _itemName = "Scanner";

    private void Start()
    {
        playerCameraTransform = GameObject.FindWithTag("PlayerCam").transform;
        if(playerCameraTransform == null) Debug.LogError("PlayerCam is null");
    }

    public void Interact(Transform interactorTransform)
    {
        Debug.Log($"interacted with {transform.name}");
        TogglePick(interactorTransform);
    }

    private void TogglePick(Transform interactorTransform)
    {
        isHolding = !isHolding;
        
        if (interactorTransform.GetComponent<PlayerInteract>().holdingItem != null)
        {
            if(isHolding) isHolding = !isHolding;
        }
        
        if (isHolding)
        {
            interactorTransform.GetComponent<PlayerInteract>().holdingItem = this.gameObject;
        }
        
        if(interactorTransform.GetComponent<PlayerInteract>().holdingItem != null && interactorTransform.GetComponent<PlayerInteract>().holdingItem == this.gameObject)
        {
            if(!isHolding) interactorTransform.GetComponent<PlayerInteract>().holdingItem = null;
        }
    }

    public String GetUsage()
    {
        return "Press F to Scan";
    }

    public string GetItemName()
    {
        return _itemName;
    }
    public string GetInteractText()
    {
        return $"Press E to Pick up {transform.name}";
    }
    
    public void Use(Transform interactorTransform)
    {
        GameObject itemToScan = interactorTransform.GetComponent<PlayerInteract>().GetInteractableObj();
        if (isHolding)
        {
            if(itemToScan == null) return;
            if (itemToScan.transform.GetComponent<IInteractable>() != null && // 可互動
                itemToScan.transform.GetComponent<ProductObj>() != null && //是商品Obj
                !itemToScan.GetComponent<ProductObj>().isChecked) // 還沒被掃描過
            {
                SoundManager.instance.PlayClip_Scan();
                itemToScan.GetComponent<ProductObj>().Highlight();
                itemToScan.GetComponent<ProductObj>().isChecked = true;
            }
        }
    }
    private void Update()
    {
        Debug.DrawRay(transform.position, transform.up * 2, Color.green);
    }

    private void LateUpdate()
    {
        if(isHolding)
        {
            Holding();
        }
        else
        {
            GetComponent<Rigidbody>().isKinematic = false;
        }
    }
    
    void Holding()
    {
        transform.position = playerCameraTransform.position + playerCameraTransform.TransformDirection(new Vector3(0.55f, -0.55f, 0.8f));
        transform.rotation = Quaternion.LookRotation(playerCameraTransform.forward, Vector3.up);
        transform.rotation *= Quaternion.Euler(0f, 180f, 0f);
        GetComponent<Rigidbody>().isKinematic = true;
    }
}
