using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    public float openSpeed = 200f;
    public Transform doorPivot;
    public float openAngle = 120f;
    public float currentAngle;
    
    private Vector3 _closedPosition;
    private Vector3 _openedPosition;
    private bool _isOpen = false;
    private bool _isOpeningDoor = false;
    private string _itemName = "Door";
    public void Interact(Transform interactorTransform)
    {
        ToggleOpen();
    }

    void ToggleOpen()
    {
        if (!_isOpeningDoor)
        {
            _isOpen = !_isOpen;
            StartCoroutine(SwitchDoorState());
        }
    }

    IEnumerator SwitchDoorState()
    {
        _isOpeningDoor = true;
        if (_isOpen)// 要打開
        {
            SoundManager.instance.PlayClip_DoorClose();
            if (currentAngle < openAngle)
            {
                while (currentAngle < openAngle)
                {   
                    float angleToAdd = openSpeed * Time.deltaTime;
                    if (currentAngle + angleToAdd > openAngle)
                    {
                        angleToAdd = openAngle - currentAngle;
                    }
                    transform.RotateAround(doorPivot.position, Vector3.up, angleToAdd);
                    currentAngle += angleToAdd;
                    yield return null;
                }
            }
            else
            {
                while (currentAngle > openAngle) // openAngle 是負值
                {
                    
                    float angleToAdd = openSpeed * Time.deltaTime;
                    if (currentAngle - angleToAdd < openAngle)
                    {
                        angleToAdd = currentAngle - openAngle;
                    }
                    transform.RotateAround(doorPivot.position, Vector3.up, -angleToAdd);
                    currentAngle -= angleToAdd;
                    yield return null;
                }
            }
            
        }
        else if (!_isOpen)
        {
            SoundManager.instance.PlayClip_DoorOpen();
            if (currentAngle > 0)
            {
                while (currentAngle > 0)
                {
                    float angleToAdd = openSpeed * Time.deltaTime;
                    if (currentAngle - angleToAdd < 0)
                    {
                        angleToAdd = currentAngle;
                    }
                    transform.RotateAround(doorPivot.position, Vector3.up, -angleToAdd);
                    currentAngle -= angleToAdd;
                    yield return null;
                }
            }
            else
            {
                while (currentAngle < 0)
                {
                    float angleToAdd = openSpeed * Time.deltaTime;
                    if (currentAngle + angleToAdd > 0)
                    {
                        angleToAdd = Mathf.Abs(currentAngle);
                    }
                    transform.RotateAround(doorPivot.position, Vector3.up, angleToAdd);
                    currentAngle += angleToAdd;
                    yield return null;
                }
            }
            
        }
        _isOpeningDoor = false;
    }
    
    public string GetInteractText()
    {
        if (_isOpen)
        {
            return $"Press E to CLOSE door";
        }
        else
        {
            return $"Press E to OPEN door";
        }
    }

    public string GetUsage()
    {
        return "Please make sure to close all the doors before leave the store";
    }

    public void Use(Transform interactorTransform)
    {
        Debug.Log("cant use");
    }

    public string GetItemName()
    {
        return _itemName;
    }
}
