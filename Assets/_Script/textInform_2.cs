using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class textInform_2 : MonoBehaviour
{
    public GameObject textCanvas;

    private void Start()
    {
        textCanvas.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            textCanvas.SetActive(false);
        }
    }
}
