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
            //���� audioSource�� �پ����� ������ �Ҹ� ��� x 
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
