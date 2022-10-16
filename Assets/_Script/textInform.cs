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
        sound = GetComponent<AudioSource>();
    }
    private void Start()
    {
        Canvas.SetActive(false);        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            Canvas.SetActive(true);
            sound.Stop();
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
