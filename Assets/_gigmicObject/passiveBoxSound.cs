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

    void Start()
    {      
        rigid = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!lastCollide && isCollide)
        {
            //box 가 땅에 충돌했을 때 
            sound.Stop();
            sound.clip = box_impact;
            sound.Play();
        }else if(lastCollide && !isCollide)
        {
            //box 가 떨어지기 시작했을 때 
            sound.Stop();
            sound.clip = chain_start;
            sound.Play();
        }

        if(rigid.velocity.magnitude >= 0.05f)
        {
            if (!moveSound.isPlaying)
            {
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
