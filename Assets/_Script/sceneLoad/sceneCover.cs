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

    void Start()
    {
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

    private void OnTriggerEnter2D(Collider2D collision) //플레이어가 콜라이더 밖 ~> 안으로 들어올 때
    {
        if(collision.tag == "Player")
        {
            StopCoroutine("fadeIn");
            StartCoroutine("fadeOut");
        }    
    }

    private void OnTriggerExit2D(Collider2D collision) //플레이어가 콜라이더 안 ~> 밖으로 나갈 때 
    {
        if(collision.tag == "Player")
        {
            isPlayerIn = false;

            StopCoroutine("fadeOut"); //밝아지는거 멈추고 
            StartCoroutine("fadeIn"); //다시 어두워짐 
        }       
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            isPlayerIn = true;
        }
    }

    IEnumerator fadeIn() //어두워짐
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
    IEnumerator fadeOut() //밝아짐
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
