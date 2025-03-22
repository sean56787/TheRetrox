using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;

public class Product
{
    public string name;
    public string category;
    public float price;
    public int quantity;

    public Product(string pName, string pCategory, float pPrice, int pQuantity)
    {
        name = pName;
        category = pCategory;
        price = pPrice;
        quantity = pQuantity;
    }
}
