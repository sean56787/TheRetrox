using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalItem : MonoBehaviour, IInteractable
{
    public Transform playerCameraTransform;
    public bool isHolding;
    private string _itemName;

    private void Start()
    {
        playerCameraTransform = GameObject.FindWithTag("PlayerCam").GetComponent<Transform>();
        if (playerCameraTransform == null)
        {
            Debug.LogError("Player camera not found");
        }
    }

    public void Interact(Transform interactorTransform)
    {
        TogglePick(interactorTransform);
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
        transform.position = playerCameraTransform.position + playerCameraTransform.TransformDirection(new Vector3(0.3f, -0.2f, 0.8f));
        transform.rotation = Quaternion.LookRotation(playerCameraTransform.forward, Vector3.up);
        transform.rotation *= Quaternion.Euler(0f, 180f, 0f);
        GetComponent<Rigidbody>().isKinematic = true;
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
        
        if(isHolding) DisableColliders();
        else EnableColliders();
    }
    
    public string GetInteractText()
    {
        return $"Press E to pick up {_itemName}";
    }

    public string GetUsage()
    {
        return "A interactable item!";
    }

    public void Use(Transform interactorTransform)
    {
        
    }

    public string GetItemName()
    {
        return _itemName;
    }

    void EnableColliders()
    {
        Collider selfCollider = GetComponent<Collider>();
        if(selfCollider) selfCollider.enabled = true;
        
        Collider[] childrenColliders = gameObject.GetComponentsInChildren<Collider>();
        if (childrenColliders != null)
        {
            foreach (var c in childrenColliders)
            {
                if(c) c.enabled = true;
            }
        }
    }
    
    void DisableColliders()
    {
        Collider selfCollider = GetComponent<Collider>();
        if(selfCollider) selfCollider.enabled = false;
        
        Collider[] childrenColliders = gameObject.GetComponentsInChildren<Collider>();
        if (childrenColliders != null)
        {
            foreach (var c in childrenColliders)
            {
                c.enabled = false;
            }
        }
    }
}
