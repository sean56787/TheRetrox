using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class HintBoard : MonoBehaviour, IInteractable
{
    public Transform playerCameraTransform;
    public bool isHolding = false;
    public List<GameObject> hintImgList = new List<GameObject>();
    public GameObject hintCanvas;
    public GameObject clickCanvas;
    public int currentImgIndex;
    private string _itemName = "HintBoard";
    
    private void Start()
    {
        clickCanvas.SetActive(isHolding);
        playerCameraTransform = GameObject.FindWithTag("PlayerCam").transform;
        if(playerCameraTransform == null) 
            Debug.LogError("PlayerCam is null");
        currentImgIndex = 0;
        hintCanvas.GetComponent<Image>().sprite = hintImgList[currentImgIndex].GetComponent<Image>().sprite;
    }
    
    public void Interact(Transform interactorTransform)
    {
        Debug.Log("Interact with HintBoard");
        TogglePick(interactorTransform);
        clickCanvas.SetActive(isHolding);
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
    
    public string GetUsage()
    {
        return "Press F to Scan";
    }

    public void Use(Transform interactorTransform)
    {
        
    }

    public string GetItemName()
    {
        return _itemName;
    }
    
    public string GetInteractText()
    {
        return $"Press E to Pick up {GetItemName()}";
    }
    
    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && isHolding)
        {
            currentImgIndex += 1;
            if(currentImgIndex > hintImgList.Count - 1) currentImgIndex = 0;
            UpdateHintImg();
        }
    }
    
    private void LateUpdate()
    {
        if(isHolding)
        {
            GetComponent<BoxCollider>().enabled = false;
            Holding();
        }
        else
        {
            GetComponent<BoxCollider>().enabled = true;
            GetComponent<Rigidbody>().isKinematic = false;
        }
    }
    
    void Holding()
    {
        transform.position = playerCameraTransform.position + playerCameraTransform.TransformDirection(new Vector3(0.14f, 0.2f, 0.4f));
        Vector3 dirToPlayer = playerCameraTransform.forward;
        Quaternion rotation = Quaternion.LookRotation(dirToPlayer);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 20f);
        GetComponent<Rigidbody>().isKinematic = true;
    }

    void UpdateHintImg()
    {
        hintCanvas.GetComponent<Image>().sprite = hintImgList[currentImgIndex].GetComponent<Image>().sprite;
    }

}
