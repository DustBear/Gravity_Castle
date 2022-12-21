using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sensorResHammer_sensor : MonoBehaviour
{
    [SerializeField] sensorRes_hammer hammer;

    bool isPlayerOn;
    bool isPlatformOn;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player" && collision.transform.up == transform.up)
        {
            isPlayerOn = true;
        }
        if(collision.tag == "Platform")
        {
            isPlatformOn = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            isPlayerOn = false;
        }
        if(collision.tag == "Platform")
        {
            isPlatformOn = false;
        }
    }

    private void Update()
    {
        if(isPlatformOn || isPlayerOn)
        {
            hammer.shouldHit = true;
        }
        else
        {
            hammer.shouldHit = false;
        }
    }
}
