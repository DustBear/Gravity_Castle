using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boatDoorSensor : MonoBehaviour
{
    public GameObject boatMenuObj;
    public int doorType;  //0�̸� �� ������ ������ ��, 1�̸� �� ������ ������ �� 

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
                boatMenuScript.moveInToRoom(); //�� ������ ����
            }
            if(doorType == 1)
            {
                boatMenuScript.moveOut(); //�� ������ ����
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
