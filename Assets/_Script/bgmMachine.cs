using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bgmMachine : MonoBehaviour
{
    AudioSource sound;
    [SerializeField] float soundDelay; //�Ҹ��� ������ Ŀ���� �� �ɸ��� �ð� 
    [SerializeField] float maxVolume; //�ִ� ���� ũ��(0~1���� �� ����)

    void Start()
    {
        sound = GetComponent<AudioSource>();
        sound.Stop();
        sound.volume = 0;
    }

  
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (sound.isPlaying)
        {
            return;
        }

        sound.Play();
        StartCoroutine("volumeUp");
    }

    IEnumerator volumeUp()
    {
        for(int index=1; index<=50; index++)
        {
            sound.volume = (maxVolume / 50) * index;
            yield return new WaitForSeconds(soundDelay / 50);
        }
    }

    public void soundOn()
    {
        sound.volume = maxVolume;
        sound.Play();
    }
}
