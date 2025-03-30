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
    [Header("scan sound")]
    public AudioClip Scan_Clip;
    [Header("cash sound")]
    public AudioClip Cash_Clip;
    [Header("angry sound")]
    public AudioClip Angry_Clip;
    [Header("byebye sound")]
    public AudioClip Byebye_Clip;
    [Header("itemPop sound")]
    public AudioClip ItemPop_Clip;
    [Header("snore sound")]
    public AudioClip Snore_Clip;
    [Header("hurry up & wasting my time sound")]
    public AudioClip HurryUp_Clip;
    [Header("Player Throw")]
    public AudioClip PlayerThrow_Clip;
    [Header("Door")]
    public AudioClip DoorOpen_Clip;
    public AudioClip DoorClose_Clip;
    [Header("switch")]
    public AudioClip Switch_Clip;
    
    
    
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

    public AudioSource GetAvailableAudioSource()
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

    void PlayClip_ButtonClick_Invoker()
    {
        StartCoroutine(PlayClip_ButtonClick());
    }
    
    IEnumerator PlayClip_ButtonClick()
    {
        AudioSource validAudioSource = GetAvailableAudioSource();
        float duration = ButtonClick_Clip.length;
        if(validAudioSource!= null) validAudioSource.PlayOneShot(ButtonClick_Clip, 0.3f);
        yield return new WaitForSeconds(duration);
        validAudioSource.Stop();
    }
    
    public IEnumerator PlayClip_Scan()
    {
        AudioSource validAudioSource = GetAvailableAudioSource();
        float duration = Scan_Clip.length;
        if(validAudioSource!= null) validAudioSource.PlayOneShot(Scan_Clip);
        yield return new WaitForSeconds(duration);
        validAudioSource.Stop();
    }
    
    public IEnumerator PlayClip_Cash()
    {
        AudioSource validAudioSource = GetAvailableAudioSource();
        float duration = Cash_Clip.length;
        if(validAudioSource!= null) validAudioSource.PlayOneShot(Cash_Clip);
        yield return new WaitForSeconds(duration);
        validAudioSource.Stop();
    }
    
    public IEnumerator PlayClip_Angry()
    {
        AudioSource validAudioSource = GetAvailableAudioSource();
        float duration = Angry_Clip.length;
        if(validAudioSource!= null) validAudioSource.PlayOneShot(Angry_Clip, 0.5f);
        yield return new WaitForSeconds(duration);
        validAudioSource.Stop();
    }

    public IEnumerator PlayClip_ItemPop()
    {
        AudioSource validAudioSource = GetAvailableAudioSource();
        float duration = ItemPop_Clip.length;
        if(validAudioSource!= null) validAudioSource.PlayOneShot(ItemPop_Clip, 0.5f);
        yield return new WaitForSeconds(duration);
        validAudioSource.Stop();
    }
    
    public IEnumerator PlayClip_Snore(AudioSource validAudioSource)
    {
        // AudioSource validAudioSource = GetAvailableAudioSource();
        float duration = Snore_Clip.length;
        if(validAudioSource!= null) validAudioSource.PlayOneShot(Snore_Clip, 0.3f);
        yield return new WaitForSeconds(duration);
        validAudioSource.Stop();
    }
    
    public IEnumerator PlayClip_HurryUp()
    {
        AudioSource validAudioSource = GetAvailableAudioSource();
        float duration = HurryUp_Clip.length;
        if(validAudioSource!= null) validAudioSource.PlayOneShot(HurryUp_Clip, 0.5f);
        yield return new WaitForSeconds(duration);
        validAudioSource.Stop();
    }
    
    public IEnumerator PlayClip_PlayerThrow()
    {
        AudioSource validAudioSource = GetAvailableAudioSource();
        float duration = PlayerThrow_Clip.length;
        if(validAudioSource!= null) validAudioSource.PlayOneShot(PlayerThrow_Clip, 0.5f);
        yield return new WaitForSeconds(duration);
        validAudioSource.Stop();
    }
    
    public IEnumerator PlayClip_DoorOpen()
    {
        AudioSource validAudioSource = GetAvailableAudioSource();
        float duration = DoorOpen_Clip.length;
        if(validAudioSource!= null) validAudioSource.PlayOneShot(DoorOpen_Clip, 0.3f);
        yield return new WaitForSeconds(duration);
        validAudioSource.Stop();
    }
    
    public IEnumerator PlayClip_DoorClose()
    {
        AudioSource validAudioSource = GetAvailableAudioSource();
        float duration = DoorClose_Clip.length;
        if(validAudioSource!= null) validAudioSource.PlayOneShot(DoorClose_Clip, 0.3f);
        yield return new WaitForSeconds(duration);
        validAudioSource.Stop();
    }
    
    public IEnumerator PlayClip_PressSwitch()
    {
        AudioSource validAudioSource = GetAvailableAudioSource();
        float duration = Switch_Clip.length;
        if(validAudioSource!= null) validAudioSource.PlayOneShot(Switch_Clip, 0.3f);
        yield return new WaitForSeconds(duration);
        validAudioSource.Stop();
    }
    
    void SetUp_MainMenuButtonOnClick()
    {
        MainMenu_StartNewGame_Button.onClick.AddListener(PlayClip_ButtonClick_Invoker);
        MainMenu_LoadGame_Button.onClick.AddListener(PlayClip_ButtonClick_Invoker);
        MainMenu_ExitGame_Button.onClick.AddListener(PlayClip_ButtonClick_Invoker);
        Escmenu_StartNewGame_Button.onClick.AddListener(PlayClip_ButtonClick_Invoker);
        Escmenu_SaveGame_Button.onClick.AddListener(PlayClip_ButtonClick_Invoker);
        Escmenu_LoadGame_Button.onClick.AddListener(PlayClip_ButtonClick_Invoker);
        Escmenu_Exit_Button.onClick.AddListener(PlayClip_ButtonClick_Invoker);
        SaveMenu_01_Button.onClick.AddListener(PlayClip_ButtonClick_Invoker);
        SaveMenu_02_Button.onClick.AddListener(PlayClip_ButtonClick_Invoker);
        SaveMenu_03_Button.onClick.AddListener(PlayClip_ButtonClick_Invoker);
        LoadMenu_01_Button.onClick.AddListener(PlayClip_ButtonClick_Invoker);
        LoadMenu_02_Button.onClick.AddListener(PlayClip_ButtonClick_Invoker);
        LoadMenu_03_Button.onClick.AddListener(PlayClip_ButtonClick_Invoker);
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
