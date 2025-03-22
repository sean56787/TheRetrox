using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public class NPCShoppingList : MonoBehaviour
{
    public List<Product> targetShoppingList = new();
    public int maxPurchaseAmount;
    private NPCPersonality _npcPersonality;
    public float totalPrice;
    private float _customerPaid;
    private void Start()
    {
        StartCoroutine(WaitForNPCPersonality());
        if(ProductManager.instance == null || _npcPersonality == null)
        {
            throw new Exception("missing ProductManager or NPCPersonality");
        }

        totalPrice = 0;
        GenerateTargetShoppingList();
    }

    private IEnumerator WaitForNPCPersonality()
    {
        while (_npcPersonality == null)
        {
            _npcPersonality = FindObjectOfType<NPCPersonality>();
            yield return null;
        }
        _npcPersonality = GetComponent<NPCPersonality>();
    }
    private void GenerateTargetShoppingList()
    {
        GenerateRandomItem();
        AddUpShoppingList();
    }
    private void GenerateRandomItem()
    {
        if(ProductManager.instance == null) Debug.LogError("ProductManager.instance is null");
        maxPurchaseAmount = GetPurchaseAmount();
        switch (_npcPersonality.personality)
        {
            case NPCPersonality.Personality.Normal:
            case NPCPersonality.Personality.Shopaholic:
            case NPCPersonality.Personality.Thrifty:
            case NPCPersonality.Personality.InHurry: 
            case NPCPersonality.Personality.Sloth:
                for (int i = 0; i < maxPurchaseAmount; i++)
                {
                    List<Product> all = ProductManager.instance.GetMenu()["All"];
                    Product randomProduct = all[Random.Range(0, all.Count)];
                    Product newProduct = ProductManager.InitProduct(randomProduct.name, randomProduct.category, randomProduct.price, 1);
                    targetShoppingList.Add(newProduct);
                }
                break;
            case NPCPersonality.Personality.Drinker:
                for (int i = 0; i < maxPurchaseAmount; i++)
                {
                    List<Product> allDrinks = ProductManager.instance.GetMenu()["Drinks"];
                    Product randomProduct = allDrinks[Random.Range(0, allDrinks.Count)];
                    Product newProduct = ProductManager.InitProduct(randomProduct.name, randomProduct.category, randomProduct.price, 1);
                    targetShoppingList.Add(newProduct);
                }
                break;
            case NPCPersonality.Personality.Fruiter:
                for (int i = 0; i < maxPurchaseAmount; i++)
                {
                    List<Product> allFruits = ProductManager.instance.GetMenu()["Fruits"];
                    Product randomProduct = allFruits[Random.Range(0, allFruits.Count)];
                    Product newProduct = ProductManager.InitProduct(randomProduct.name, randomProduct.category, randomProduct.price, 1);
                    targetShoppingList.Add(newProduct);
                }
                break;
            case NPCPersonality.Personality.Snacker:
                for (int i = 0; i < maxPurchaseAmount; i++)
                {
                    List<Product> allSnacks = ProductManager.instance.GetMenu()["Snacks"];
                    Product randomProduct = allSnacks[Random.Range(0, allSnacks.Count)];
                    Product newProduct = ProductManager.InitProduct(randomProduct.name, randomProduct.category, randomProduct.price, 1);
                    targetShoppingList.Add(newProduct);
                }
                break;
        }
    }
    private void AddUpShoppingList()
    {
        Dictionary<string, Product> calculationDic = new Dictionary<string, Product>();

        foreach (var product in targetShoppingList)
        {
            if (calculationDic.ContainsKey(product.name))
            {
                calculationDic[product.name].quantity += product.quantity;
            }
            else
            {
                Product newProduct = ProductManager.InitProduct(product.name, product.category, product.price, product.quantity);
                calculationDic[product.name] = newProduct;
            }

            totalPrice += product.price;
        }
        targetShoppingList = calculationDic.Values.ToList();
    }
    private int GetPurchaseAmount()
    {
        switch (_npcPersonality.personality)
        {
            case NPCPersonality.Personality.Normal: 
            case NPCPersonality.Personality.Drinker:
            case NPCPersonality.Personality.Fruiter:
            case NPCPersonality.Personality.Snacker:
                return Random.Range(3, 5); // 3~4
            case NPCPersonality.Personality.Shopaholic:
                return Random.Range(5, 7); // 5~6
            case NPCPersonality.Personality.Thrifty:
                return Random.Range(1, 3);// 1~2
            case NPCPersonality.Personality.InHurry: 
                return 1;
            case NPCPersonality.Personality.Sloth:
                return 4;
            default: return 1;
        }
    }

    public float GetCustomerPaid()
    {
        _customerPaid = totalPrice;
        return _customerPaid;
    }
}
