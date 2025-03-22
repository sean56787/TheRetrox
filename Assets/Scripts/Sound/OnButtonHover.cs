using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class OnButtonHover : MonoBehaviour, IPointerEnterHandler
{
    public AudioSource audioSource;
    public AudioClip buttonHoverClip;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (audioSource != null && buttonHoverClip != null)
        {
            audioSource.PlayOneShot(buttonHoverClip);
        }
    }
}
