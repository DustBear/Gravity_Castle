using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameOver : MonoBehaviour
{
    public GameObject gameoverCanvas;
    void Start()
    {
        gameoverCanvas.SetActive(false);
    }

   
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        gameoverCanvas.SetActive(true);
    }
}
