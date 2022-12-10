using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movingSpike : MonoBehaviour
{
    public Vector2 pos1, pos2; //pos1�� pos2 ���� ���� ŭ 
    public float moveDelay; //spike�� pos1 �� pos2 ���̸� ������ �� �ɸ��� �ð� 
    public float stopDelay; //spike�� �� �������� �ӹ��� �ִ� �ð� 
    public int initPos;

    Rigidbody2D rigid;
    Animator anim;

    public AudioSource sound;
    public AudioSource sound_loop;

    public AudioClip moveStart;
    public AudioClip moveFinish;
    public AudioClip moving;
    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }
    void Start()
    {
        anim.SetFloat("animSpeed", 0);
        sound.clip = moving;

        if (initPos == 1) 
        {
            transform.position = pos1;
            StartCoroutine(spikeMove(1));
        }
        else
        {
            transform.position = pos2;
            StartCoroutine(spikeMove(-1));
        }
    }
    
    IEnumerator spikeMove(int aimpos) //aimPos�� ���� �� 
    {
        //1�̸� pos1 ~> pos2 
        //-2�̸� pos2 ~> pos1 �� �̵� 

        //�ʱ� �ӵ� 
        float moveSpeed = (pos1 - pos2).magnitude / moveDelay;

        sound_loop.volume = GameManager.instance.optionSettingData.masterVolume_setting * GameManager.instance.optionSettingData.effectVolume_setting;
        sound_loop.Play();

        sound.volume = GameManager.instance.optionSettingData.masterVolume_setting * GameManager.instance.optionSettingData.effectVolume_setting;
        sound.PlayOneShot(moveStart);

        if(aimpos == 1) //pos2�� ���� �� 
        {
            rigid.velocity = (pos2 - pos1).normalized * moveSpeed;
            anim.SetFloat("animSpeed", -1);
        }
        else //pos1�� ���� �� 
        {
            rigid.velocity = (pos1 - pos2).normalized * moveSpeed;
            anim.SetFloat("animSpeed", 1);
        }

        yield return new WaitForSeconds(moveDelay-0.2f);

        sound.volume = GameManager.instance.optionSettingData.masterVolume_setting * GameManager.instance.optionSettingData.effectVolume_setting;
        sound.PlayOneShot(moveFinish);

        yield return new WaitForSeconds(0.2f);

        rigid.velocity = Vector2.zero;

        if(aimpos == 1)
        {
            transform.position = pos2;
        }
        else
        {
            transform.position = pos1;
        }

        anim.SetFloat("animSpeed", 0);
        sound_loop.Stop();
       
        yield return new WaitForSeconds(stopDelay);
        StartCoroutine(spikeMove(aimpos * (-1)));
    }
}
