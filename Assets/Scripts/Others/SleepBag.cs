using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SleepBag : MonoBehaviour, IInteractable
{
    public string _itemName = "SleepBag";
    public float canvasGroupFadeDuration = 3f;
    
    public void Interact(Transform interactorTransform) // 商店睡袋
    {
        if (DayNightManager.instance.isDaytime)
        {
            if (!PlayerInteractUI.instance.isShowingHint) StartCoroutine(CanNotSleep());
        }
        else
        {
            DayNightManager.instance.OnPlayerSleep?.Invoke(); // 進入下一天
        }
    }

    public string GetInteractText()
    {
        return $"Press E to Sleep";
    }

    public string GetUsage()
    {
        return "none";
    }

    public void Use(Transform interactorTransform)
    {
        Debug.Log("can not use");
    }

    public string GetItemName()
    {
        return _itemName;
    }

    IEnumerator CanNotSleep()
    {
        PlayerInteractUI.instance.isShowingHint = true;
        PlayerInteractUI.instance.ShowHint("Cant Sleep right now, Still got work to do...");
        PlayerInteractUI.instance.interactableUIHintText.GetComponent<CanvasGroup>().alpha = 1;
        
        float startAlpha = PlayerInteractUI.instance.interactableUIHintText.GetComponent<CanvasGroup>().alpha;
        float elaspedTime = 0;
        while (elaspedTime < canvasGroupFadeDuration)
        {
            PlayerInteractUI.instance.interactableUIHintText.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(startAlpha, 0, elaspedTime / canvasGroupFadeDuration);
            elaspedTime += Time.deltaTime;
            yield return null;
        }
        PlayerInteractUI.instance.interactableUIHintText.GetComponent<CanvasGroup>().alpha = 0;
        PlayerInteractUI.instance.HideHint();
        PlayerInteractUI.instance.interactableUIHintText.GetComponent<CanvasGroup>().alpha = 1;
        PlayerInteractUI.instance.isShowingHint = false;
    }
}
