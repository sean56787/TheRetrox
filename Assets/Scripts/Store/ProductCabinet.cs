using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[System.Serializable]

public class ProductCabinet : MonoBehaviour
{
    public string cabinetCategory;
    public List<ProductObj> stock = new();
    public List<Transform> customerQueueTransforms = new();
    public bool[] queueCheck = {false};
    public event Action<Queue<Product>, Product> OnCustomerProductRequest;
    
    private void Start()
    {
        StartCoroutine(WaitForCabinetManager());
        OnCustomerProductRequest += CustomerGetProduct;
    }
    
    private IEnumerator WaitForCabinetManager()
    {
        while (CabinetManager.instance == null)
        {
            CabinetManager.instance = FindObjectOfType<CabinetManager>();
            yield return null;
        }
        CabinetManager.instance.RegisterCabinet(this);
    }
    
    public void CustomerProductRequestInvoker(Queue<Product> arg1, Product arg2)
    {
        if(OnCustomerProductRequest != null)
        {
            OnCustomerProductRequest?.Invoke(arg1, arg2);
        }
    }
    
    public void CustomerGetProduct(Queue<Product> currentProductQueue, Product targetProduct) // 顧客拿商品
    {
        foreach (var productInStock in stock)
        {
            if (productInStock.productObjName == targetProduct.name && productInStock.productObjQuantity >= targetProduct.quantity)
            {
                productInStock.productObjQuantity -= targetProduct.quantity;
                currentProductQueue.Enqueue(new Product(productInStock.productObjName, productInStock.productObjCategory,productInStock.productObjPrice,targetProduct.quantity));
            }
        }
    }
    
    public Transform GetValidPosition()
    {
        for (int i = 0; i < customerQueueTransforms.Count; i++)
        {
            if (!queueCheck[i])
            {
                queueCheck[i] = true;
                return customerQueueTransforms[i];
            }
        }
        return null;
    }
    
    public void ReleaseValidPosition(Transform trans)
    {
        for (int i = 0; i < customerQueueTransforms.Count; i++)
        {
            if (customerQueueTransforms[i] == trans)
            {
                queueCheck[i] = false;
            }
        }
    }
}
