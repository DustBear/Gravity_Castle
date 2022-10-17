using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class objectFallSound : MonoBehaviour
{
    bool isStoneFall;
    bool isCollide = false;
    bool lastCollide = false;
    public bool shouldNotWork;

    AudioSource sound;
    void Start()
    {
        sound = GetComponent<AudioSource>();   
    }

    void Update()
    {
        if(!lastCollide && isCollide)
        {
            sound.Play();
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
