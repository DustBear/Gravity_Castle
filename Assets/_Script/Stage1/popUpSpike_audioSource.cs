using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class popUpSpike_audioSource : MonoBehaviour
{
    AudioSource sound;

    public AudioClip spike_in;
    public AudioClip spike_out;

    private void Awake()
    {
        sound = GetComponent<AudioSource>();
    }
    
    public void spikeIn_Play() //가시 튀어나오는 소리 재생 
    {
        sound.PlayOneShot(spike_in);
    }
    public void spikeOut_Play()
    {
        sound.PlayOneShot(spike_out);
    }
}