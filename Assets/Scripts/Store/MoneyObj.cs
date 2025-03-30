using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class MoneyObj : MonoBehaviour, IInteractable
{
    public float money;
    public bool playerReceived;
    public bool absorbing;
    private string _itemName = "MoneyObj";
    public Transform playerTransform;

    public void Interact(Transform interactorTransform)
    {
        if (interactorTransform.GetComponent<Player>() != null) //是玩家
        {
            StartCoroutine(SoundManager.instance.PlayClip_Cash());
            playerTransform = interactorTransform;
            absorbing = true;
            playerReceived = true;
            gameObject.GetComponent<Rigidbody>().isKinematic = true;
            gameObject.GetComponent<MeshCollider>().enabled = false;
        }
    }
    
    private void Update()
    {
        Debug.DrawRay(transform.position, transform.up * 2, Color.green);
        if (absorbing)
        {
            transform.position = Vector3.MoveTowards(transform.position, playerTransform.position, Time.deltaTime * 3f);
            if (Vector3.Distance(transform.position, playerTransform.position) < 0.1f)
            {
                Player.instance.balance += money;
                PlayerUI.instance.UpdatePlayerUI();
                absorbing = false;
                Destroy(gameObject);
            }
        }
    }

    public string GetInteractText()
    {
        return "Press E to take money";
    }

    public string GetUsage()
    {
        throw new NotImplementedException();
    }

    public void Use(Transform interactorTransform)
    {
        throw new NotImplementedException();
    }

    public string GetItemName()
    {
        return _itemName;
    }
    
    public void Highlight()
    {
        transform.GetComponent<Outline>().enabled = true;
        transform.GetComponent<Outline>().OutlineColor = Color.yellow;
        transform.GetComponent<Outline>().OutlineWidth = 5f;
    }
    
}
