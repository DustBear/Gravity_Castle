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
    
    public void spikeIn_Play() //���� Ƣ����� �Ҹ� ��� 
    {
        sound.volume = sound.volume = GameManager.instance.optionSettingData.effectVolume_setting * GameManager.instance.optionSettingData.masterVolume_setting;
        sound.PlayOneShot(spike_in);
    }
    public void spikeOut_Play()
    {
        sound.volume = sound.volume = GameManager.instance.optionSettingData.effectVolume_setting * GameManager.instance.optionSettingData.masterVolume_setting;
        sound.PlayOneShot(spike_out);
    }
}