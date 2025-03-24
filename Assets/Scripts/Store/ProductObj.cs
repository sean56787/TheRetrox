using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ProductObj : MonoBehaviour, IInteractable
{
    public Transform playerCameraTransform;
    public string productObjName;
    public string productObjCategory;
    public float productObjPrice;
    public int productObjQuantity;
    public Product product;
    public bool isChecked;
    private Outline _outline;
    public bool isHolding;
    private string _itemName;
    private void Awake()
    {
        product = ProductManager.InitProduct(productObjName, productObjCategory, productObjPrice, productObjQuantity);
        productObjName = product.name;
        productObjCategory = product.category;
        productObjPrice = product.price;
        productObjQuantity = product.quantity;
        isChecked = false;
        _itemName = productObjName;
        
    }

    private void Start()
    {
        _outline = GetComponent<Outline>();
        if(_outline) _outline.enabled = false;
        playerCameraTransform = GameObject.FindWithTag("PlayerCam").transform;
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
        transform.position = playerCameraTransform.position + playerCameraTransform.TransformDirection(new Vector3(0.3f, -0.2f, 0.8f));
        transform.rotation = Quaternion.LookRotation(playerCameraTransform.forward, Vector3.up);
        transform.rotation *= Quaternion.Euler(0f, 180f, 0f);
        GetComponent<Rigidbody>().isKinematic = true;
    }
    
    public void Interact(Transform interactorTransform)
    {
        Debug.Log($"interacted with {transform.name}");
        TogglePick(interactorTransform);
    }

    public string GetInteractText()
    {
        if (PlayerInteract.instance.holdingItem != null && PlayerInteract.instance.holdingItem.transform.GetComponent<IInteractable>().GetItemName() == "Scanner")
        {
            return $"{PlayerInteract.instance.holdingItem.transform.GetComponent<IInteractable>().GetUsage()} {productObjName}";
        }
        else
        {
            return $"Press E to pick up {productObjName}";
        }
    }
    public string GetUsage()
    {
        return "Remember to checkout this item before walk out the store ~";
    }

    public string GetItemName()
    {
        return _itemName;
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
    public void Use(Transform interactorTransform)
    {
        
    }

    public void Highlight()
    {
        _outline.enabled = true;
        _outline.OutlineColor = Color.green;
        _outline.OutlineWidth = 5f;
    }

    private void UnHighlight()
    {
        _outline.enabled = false;
    }
}
