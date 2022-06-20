using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class textInform : MonoBehaviour
{
    public GameObject textCanvas;

    private void Start()
    {
        textCanvas.SetActive(false);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            textCanvas.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            textCanvas.SetActive(false);   
        }
    }
   
}
