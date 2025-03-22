using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
public class DayNightManager : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public PlayerInteract playerInteract;
    public static DayNightManager instance;
    public Image fadePanel;
    public float fadeDuration = 1f;
    public GameObject sunLight;
    public float dayDuration = 300f;
    public float currentTime = 0f;
    public Material dayMaterial;
    public Material nightMaterial;
    public bool isDaytime = true;
    public GameObject[] turnableLights;
    public GameObject timeUIObject;
    public TextMeshProUGUI dayCycleTextMeshProUGUI;
    public Action OnPlayerSleep;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        
    }

    private void Start()
    {
        timeUIObject.SetActive(true);
        isDaytime = true;
        LightSwitch(false);
        SwitchToDay();
        OnPlayerSleep += SkipToDay;
       StartCoroutine(DayCycle());
    }

    public void ResetTime()
    {
        currentTime = 0f;
    }
    private IEnumerator DayCycle()
    {
        while (true) // TODO: while (true)不好看
        {
            if (isDaytime)
            {
                currentTime += Time.deltaTime;
                UpdateTimeUI();
            }
            
            if (currentTime >= dayDuration)
            {
                SwitchToNight();
            }
            
            yield return null;
        }
    }

    private void UpdateTimeUI()
    {
        int minutes = Mathf.FloorToInt((currentTime / dayDuration) * 12);
        int hours = 6 + minutes;
        dayCycleTextMeshProUGUI.text = $"{hours}:00";
    }

    private void SwitchToNight()
    {
        Debug.Log("SwitchToNight");
        sunLight.SetActive(false);
        isDaytime = false;
        LightSwitch(true);
        dayCycleTextMeshProUGUI.text = "Night";
        RenderSettings.skybox = nightMaterial;
        RenderSettings.ambientLight = new Color(0.1f, 0.1f, 0.1f);
        DynamicGI.UpdateEnvironment();
    }

    void SkipToDay()
    {
        playerInteract.enabled = false;
        playerMovement.enabled = false;
        StartCoroutine(ScreenFadeToBlack());
        
        IEnumerator ScreenFadeToBlack()
        {
            yield return StartCoroutine(Fade(0, 1, fadeDuration));
            yield return new WaitForSeconds(2f);
            StartCoroutine(ScreenFadeToNormal());
        }
    
        IEnumerator ScreenFadeToNormal()
        {
            yield return StartCoroutine(Fade(1, 0, fadeDuration));
            SwitchToDay();
        }

        IEnumerator Fade(float startAlpha, float endAlpha, float duration)
        {
            float elapsed = 0f;
            Color color = fadePanel.color;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                color.a = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
                fadePanel.color = color;
                yield return null;
            }

            color.a = endAlpha;
            fadePanel.color = color;
        }
    }


    private void SwitchToDay()
    {
        GameStateManager.instance.ResetNPCStuffOnly();
        playerInteract.enabled = true;
        playerMovement.enabled = true;
        sunLight.SetActive(true);
        isDaytime = true;
        LightSwitch(false);
        ResetTime();
        RenderSettings.skybox = dayMaterial;
        RenderSettings.ambientLight = new Color(0.6f, 0.6f, 0.6f);
        DynamicGI.UpdateEnvironment();
    }

    private void LightSwitch(bool x)
    {
        foreach (var l in turnableLights)
        {
            l.SetActive(x);
        }
    }

}
