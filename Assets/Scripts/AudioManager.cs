using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource _showerLoop;
    public AudioSource _showerStart;

    public void PlayShowerLoopSFX()
    {
        _showerLoop.Play();
    }

    public void StopShowerLoopSFX()
    {
        _showerLoop.Stop();
    }
}
