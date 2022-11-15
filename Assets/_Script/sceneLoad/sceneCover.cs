using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class sceneCover : MonoBehaviour
{
    public float fadeTime;
    SpriteRenderer spr;

    [SerializeField] bool isPlayerIn;
    [SerializeField] bool isCoroutinePlay;

    AudioSource sound;
    public AudioClip fadeInSound;
    public AudioClip fadeOutSound;

    void Start()
    {
        sound = GetComponent<AudioSource>();
        spr = GetComponent<SpriteRenderer>();
        if (isPlayerIn)
        {
            spr.color = new Color(1, 1, 1, 0);
        }
        else
        {
            spr.color = new Color(1, 1, 1, 1);
        }
    }

    
    void Update()
    {
        if (isPlayerIn && !isCoroutinePlay)
        {
            spr.color = new Color(1, 1, 1, 0);
        }

        else if (!isPlayerIn && !isCoroutinePlay)
        {
            spr.color = new Color(1, 1, 1, 1);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) //�÷��̾ �ݶ��̴� �� ~> ������ ���� ��
    {
        if(collision.tag == "Player")
        {
            StopCoroutine("fadeIn");
            StartCoroutine("fadeOut");

            sound.PlayOneShot(fadeInSound);
        }    
    }

    private void OnTriggerExit2D(Collider2D collision) //�÷��̾ �ݶ��̴� �� ~> ������ ���� �� 
    {
        if(collision.tag == "Player")
        {
            isPlayerIn = false;

            StopCoroutine("fadeOut"); //������°� ���߰� 
            StartCoroutine("fadeIn"); //�ٽ� ��ο��� 

            sound.PlayOneShot(fadeOutSound);
        }       
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            isPlayerIn = true;
        }
    }

    IEnumerator fadeIn() //��ο���
    {
        isCoroutinePlay = true;
        for(int index=1; index<=100; index++)
        {
            spr.color = new Color(1, 1, 1, spr.color.a + 0.01f);
            yield return new WaitForSeconds(fadeTime / 100);

            if(spr.color.a >= 1)
            {
                spr.color = new Color(1, 1, 1, 1);
                isCoroutinePlay = false;
                break;
            }
        }
        isCoroutinePlay = false;
    }
    IEnumerator fadeOut() //�����
    {
        isCoroutinePlay = true;
        for (int index = 99; index >= 0; index--)
        {
            spr.color = new Color(1, 1, 1, spr.color.a - 0.01f);
            yield return new WaitForSeconds(fadeTime / 100);

            if (spr.color.a <= 0)
            {
                spr.color = new Color(1, 1, 1, 0);
                isCoroutinePlay = false;
                break;
            }
        }
        isCoroutinePlay = false;
    }
}
