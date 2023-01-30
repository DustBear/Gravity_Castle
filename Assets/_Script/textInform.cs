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
        if(GetComponent<AudioSource>() != null)
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
        if(collision.tag == "Player")
            //���� audioSource�� �پ����� ������ �Ҹ� ��� x 
        {
            Canvas.SetActive(true);           
            sound.Stop();
            
            sound.volume = GameManager.instance.optionSettingData.masterVolume_setting * GameManager.instance.optionSettingData.effectVolume_setting;
            if (!GameManager.instance.gameData.UseOpeningElevetor_bool)
            {
                sound.Play();
            }
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
