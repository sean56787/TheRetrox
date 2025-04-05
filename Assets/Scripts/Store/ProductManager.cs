using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductManager : MonoBehaviour
{
    public static ProductManager instance;
    public List<GameObject> productPrefabs = new List<GameObject>();
    private Dictionary<string, List<Product>> _menu;
    private void Awake() //這裡設置商品價格
    {
        if(instance == null) instance = this;
        Product cola = InitProduct("Cola", "Drink", 1f, -1); 
        Product milk = InitProduct("Milk", "Drink", 4f, -1);
        Product juice = InitProduct("Juice", "Drink", 3f, -1);
        Product apple = InitProduct("Apple", "Fruit", 0.5f, -1);
        Product banana = InitProduct("Banana", "Fruit", 0.5f, -1);
        Product orange = InitProduct("Orange", "Fruit", 0.5f, -1);
        Product cookie = InitProduct("Cookie", "Snack", 3.5f, -1);
        Product lays = InitProduct("Lays", "Snack", 2.5f, -1);
        Product chocolate = InitProduct("Chocolate", "Snack", 1.5f, -1);
        _menu = new Dictionary<string, List<Product>>()
        {
            { "Drinks", new List<Product> { cola, milk, juice } },
            { "Fruits", new List<Product> { apple, banana, orange } },
            { "Snacks", new List<Product> { cookie, lays, chocolate } },
            { "All" , new List<Product> {cola, milk, juice, apple, banana, orange, cookie, lays, chocolate}}
        };
    }

    public Dictionary<string, List<Product>> GetMenu()
    {
        if(_menu == null) Debug.LogError("Menu is null");
        return _menu;
    }
    public static Product InitProduct(string pName, string pCategory, float pPrice, int pQuantity) // Product繼承了mono->無法直接new 變成先建立遊戲物件 再手動加入Product類
    {
        Product product = new Product(pName, pCategory, pPrice, pQuantity);
        return product;
    }
    public GameObject InstantiateProductObj(Product productToInstantiate, Transform initPos) 
    {
        foreach (var prefab in productPrefabs)
        {
            if (productToInstantiate.name == prefab.GetComponent<ProductObj>().name)
            {
                GameObject obj = Instantiate(prefab, initPos.position, Quaternion.identity);
                return obj;
            }
        }
        return null;
    }
}
