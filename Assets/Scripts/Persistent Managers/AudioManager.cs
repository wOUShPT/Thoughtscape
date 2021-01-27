using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource cityLoop;
    public AudioSource showerLoop;
    public AudioSource showerStart;
    public bool isMuted;

    private void Start()
    {
        Mute(isMuted);
    }

    public void PlayMainMenuAmbience()
    {
        cityLoop.Play();
    }

    public void StopMainMenuAmbience()
    {
        cityLoop.Stop();
    }

    public void PlayShowerLoopSFX()
    {
        showerLoop.Play();
    }

    public void StopShowerLoopSFX()
    {
        showerLoop.Stop();
    }

    public void Mute(bool state)
    {
        isMuted = state;
        SetMute();
    }

    public void SetMute()
    {
        cityLoop.mute = isMuted;
        showerLoop.mute = isMuted;
    }
    

}
