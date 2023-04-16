using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using TMPro;

public class Settings : MonoBehaviour
{
    #region FloatVaribles
    [HideInInspector] public float BGMMixerVolume;
    [HideInInspector] public float SFXMixerVolume;
    #endregion


    #region OtherVariables
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider BGMMixerVolumeSlider;
    [SerializeField] private Slider SFXMixerVolumeSlider;
    #endregion

    void Start()
    {
        BGMMixerVolume = PlayerPrefs.GetFloat("BGMMixerVolume",0);
        SFXMixerVolume = PlayerPrefs.GetFloat("SFXMixerVolume",0);

        audioMixer.SetFloat("BGM_Volume",BGMMixerVolume);
        audioMixer.SetFloat("SFX_Volume",SFXMixerVolume);

        BGMMixerVolumeSlider.value = BGMMixerVolume;
        SFXMixerVolumeSlider.value = SFXMixerVolume;
        
        gameObject.SetActive(false);
    }

    public void ChangeBGMVolume(float value)
    {
        audioMixer.SetFloat("BGM_Volume",value);
        PlayerPrefs.SetFloat("BGMMixerVolume",value);
    }

    public void ChangeSFXVolume(float value)
    {
        audioMixer.SetFloat("SFX_Volume",value);
        PlayerPrefs.SetFloat("SFXMixerVolume",value);
    }
}
