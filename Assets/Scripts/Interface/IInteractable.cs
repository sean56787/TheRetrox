using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable // 互動介面
{
    void Interact(Transform interactorTransform);
    string GetInteractText();
    string GetUsage();
    void Use(Transform interactorTransform);
    string GetItemName();
}
