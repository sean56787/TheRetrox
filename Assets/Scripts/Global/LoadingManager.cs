using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;
public class LoadingManager : MonoBehaviour
{
    public Slider progressBar;
    public TextMeshProUGUI progressText;

    private void Start()
    {
        string targetScene = PlayerPrefs.GetString("NewScene", "TheRetrox");
        Debug.Log("targetScene: " + targetScene);
    }

    IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        if (operation == null) yield break;
        operation.allowSceneActivation = false;
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            progressBar.value = progress; // 進度條
            progressText.text = (progress * 100f).ToString("F0") + "%"; // 百分比

            if (operation.progress >= 0.9f)
            {
                progressText.text = "Press any key to continue";
                if (Input.anyKeyDown)
                {
                    operation.allowSceneActivation = true;
                }
            }
            yield return null;
        }
    }
}
