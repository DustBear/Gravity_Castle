using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class passiveBoxSound : MonoBehaviour
{
    public AudioSource sound;
    public AudioSource moveSound;

    public AudioClip chain_start;
    public AudioClip box_impact;

    bool isCollide = false;
    bool lastCollide = false;

    Rigidbody2D rigid;

    public int activeNum;
    void Start()
    {      
        rigid = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if(GameManager.instance.gameData.curAchievementNum != activeNum)
        {
            return;
        }

        if (!lastCollide && isCollide)
        {
            //box �� ���� �浹���� �� 

            sound.volume = GameManager.instance.optionSettingData.masterVolume_setting * GameManager.instance.optionSettingData.effectVolume_setting;
            sound.PlayOneShot(box_impact);
        }else if(lastCollide && !isCollide)
        {
            //box �� �������� �������� �� 
            sound.volume = GameManager.instance.optionSettingData.effectVolume_setting * GameManager.instance.optionSettingData.masterVolume_setting;
            sound.PlayOneShot(chain_start);
        }

        if(rigid.velocity.magnitude >= 0.05f)
        {
            if (!moveSound.isPlaying)
            {
                moveSound.volume = GameManager.instance.optionSettingData.effectVolume_setting * GameManager.instance.optionSettingData.masterVolume_setting;
                moveSound.Play();
            }
        }
        else
        {
            if (moveSound.isPlaying)
            {
                moveSound.Stop();
            }
        }
        lastCollide = isCollide;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Platform")
        {
            isCollide = true;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Platform")
        {
            isCollide = false;
        }
    }
}
