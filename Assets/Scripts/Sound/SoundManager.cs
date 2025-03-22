using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    [Header("Audio Sources")]
    public AudioSource audioSourceForBGM;
    
    public List<AudioSource> audioSourcePool = new();
    public int MAX_audioSource = 10;
    [Header("button click clips")]
    public AudioClip ButtonClick_Clip;
    [Header("footstep sound")]
    public AudioClip Footstep_Clip;
    public float playerFootstepVolume = 1f;
    public float playerFootstepDelay = 0.5f;
    public bool canPlayPlayerFootstep = true;
    [Header("door sound")]
    public AudioClip DoorBell_Clip;
    
    [Header("Main menu button refs")]
    public Button MainMenu_StartNewGame_Button;
    public Button MainMenu_LoadGame_Button;
    public Button MainMenu_ExitGame_Button;
    [Header("Esc menu button refs")]
    public Button Escmenu_StartNewGame_Button;
    public Button Escmenu_SaveGame_Button;
    public Button Escmenu_LoadGame_Button;
    public Button Escmenu_Exit_Button;
    [Header("Save & load menu button refs")]
    public Button SaveMenu_01_Button;
    public Button SaveMenu_02_Button;
    public Button SaveMenu_03_Button;
    
    public Button LoadMenu_01_Button;
    public Button LoadMenu_02_Button;
    public Button LoadMenu_03_Button;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        SetUp_MainMenuButtonOnClick();
        PlayBGM();
    }

    AudioSource GetAvailableAudioSource()
    {
        foreach (var AS in audioSourcePool)
        {
            if (!AS.isPlaying)
            {
                return AS;
            }
        }

        if (audioSourcePool.Count < MAX_audioSource)
        {
            AudioSource newSource = gameObject.AddComponent<AudioSource>();
            audioSourcePool.Add(newSource);
            return newSource;
        }

        return null;
    }
    void PlayBGM()
    {
        audioSourceForBGM.loop = true;
        audioSourceForBGM.Play();
    }
    
    void PlayClip_ButtonClick()
    {
        AudioSource validAudioSource = GetAvailableAudioSource();
        if(validAudioSource!= null) validAudioSource.PlayOneShot(ButtonClick_Clip, 0.5f);
    }

    void SetUp_MainMenuButtonOnClick()
    {
        MainMenu_StartNewGame_Button.onClick.AddListener(PlayClip_ButtonClick);
        MainMenu_LoadGame_Button.onClick.AddListener(PlayClip_ButtonClick);
        MainMenu_ExitGame_Button.onClick.AddListener(PlayClip_ButtonClick);
        Escmenu_StartNewGame_Button.onClick.AddListener(PlayClip_ButtonClick);
        Escmenu_SaveGame_Button.onClick.AddListener(PlayClip_ButtonClick);
        Escmenu_LoadGame_Button.onClick.AddListener(PlayClip_ButtonClick);
        Escmenu_Exit_Button.onClick.AddListener(PlayClip_ButtonClick);
        SaveMenu_01_Button.onClick.AddListener(PlayClip_ButtonClick);
        SaveMenu_02_Button.onClick.AddListener(PlayClip_ButtonClick);
        SaveMenu_03_Button.onClick.AddListener(PlayClip_ButtonClick);
        LoadMenu_01_Button.onClick.AddListener(PlayClip_ButtonClick);
        LoadMenu_02_Button.onClick.AddListener(PlayClip_ButtonClick);
        LoadMenu_03_Button.onClick.AddListener(PlayClip_ButtonClick);
    }

    public IEnumerator PlayClip_PlayerWalk()
    {
        AudioSource validAudioSource = GetAvailableAudioSource();
        if (validAudioSource != null)
        {
            canPlayPlayerFootstep = false;
            validAudioSource.PlayOneShot(Footstep_Clip, playerFootstepVolume);
            yield return new WaitForSeconds(playerFootstepDelay);
            canPlayPlayerFootstep = true;
        }
    }
}
