using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boatDoorSensor : MonoBehaviour
{
    public GameObject boatMenuObj;
    public int doorType;  //0이면 배 안으로 들어오는 문, 1이면 배 밖으로 나가는 문 

    boatMenu boatMenuScript;
    bool isSensorOn;

    void Start()
    {
        isSensorOn = false;
        boatMenuScript = boatMenuObj.GetComponent<boatMenu>();
    }
   
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E) && isSensorOn)
        {
            if(doorType == 0)
            {
                boatMenuScript.moveInToRoom(); //방 안으로 들어옴
            }
            if(doorType == 1)
            {
                boatMenuScript.moveOut(); //방 밖으로 나감
            }            
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            isSensorOn = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            isSensorOn = false;
        }
    }
}
