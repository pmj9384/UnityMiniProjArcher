using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public enum AudioType
    {
        musicVol,
        sfxVol
    }
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider sfxSlider;

    private bool IsMuted = false;
    public void Start()
    {
        float musicVol;
        audioMixer.GetFloat(AudioType.musicVol.ToString(), out musicVol);
        musicSlider.value = musicVol != 0 ? Mathf.Pow(10, musicVol / 20) : 1;
        float sfxVol;
        audioMixer.GetFloat(AudioType.sfxVol.ToString(), out sfxVol);
        sfxSlider.value = sfxVol != 0 ? Mathf.Pow(10, sfxVol / 20) : 1;
    }

    public void SetMusicVolume(float volume)
    {
        SetVolume(AudioType.musicVol, volume);
    }

    public void SetSfxVolume(float volume)
    {
        SetVolume(AudioType.sfxVol, volume);
    }

    public void OnMuteToggle(bool mute)
    {
        if(mute)
        {
            SetMusicVolume(0);
            SetSfxVolume(0);
            IsMuted = true;
        }
        else
        {
            IsMuted = false;
            SetMusicVolume(musicSlider.value);
            SetSfxVolume(sfxSlider.value);
        }
    }
    
    public void SetVolume(AudioType type, float volume)
    {
        if(!IsMuted)
            audioMixer.SetFloat(type.ToString(), volume !=0 ? Mathf.Log10(volume) * 20 : -60);
    }
    
    
}