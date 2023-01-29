using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Text;

public class textInform : MonoBehaviour
{
    public GameObject Canvas;
    AudioSource sound;

    private void Awake()
    {
        if(GetComponent<AudioSource>().gameObject != null)
        {
            sound = GetComponent<AudioSource>();
        }
    }
    private void Start()
    {
        Canvas.SetActive(false);        
    }
   
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player" && sound != null)
            //만약 audioSource가 붙어있지 않으면 소리 재생 x 
        {
            Canvas.SetActive(true);
            sound.Stop();

            sound.volume = GameManager.instance.optionSettingData.masterVolume_setting * GameManager.instance.optionSettingData.effectVolume_setting;
            sound.Play();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Canvas.SetActive(false);
        }
    }
   
}
