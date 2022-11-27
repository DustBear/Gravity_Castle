using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class seesawElevator_sensor : MonoBehaviour
{
    seesaw_elevator parentScr;

    private void Awake()
    {
        parentScr = transform.parent.gameObject.GetComponent<seesaw_elevator>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player" && collision.transform.up == transform.up)
        {
            parentScr.isPlayerOn = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            parentScr.isPlayerOn = false;
        }
    }
}
