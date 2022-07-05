using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class side2_movingStone_sensor : MonoBehaviour
{
    public GameObject moveStone;
    public side02_movingStone moveStoneScr;

    public int collType;
    /*
    - 1 -
    4 - 2
    - 3 - 방향으로 설정
     */
    void Start()
    {
        moveStoneScr = moveStone.GetComponent<side02_movingStone>();   
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Platform")
        {
            Debug.Log(collType + " collision");
            switch (collType)
            {
                case 1:
                    moveStoneScr.isTopCollOn = true;
                    break;
                case 2:
                    moveStoneScr.isRightCollOn = true;
                    break;
                case 3:
                    moveStoneScr.isBottomCollOn = true;
                    break;
                case 4:
                    moveStoneScr.isLeftCollOn = true;
                    break;
            }
        }       
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == "Platform")
        {
            switch (collType)
            {
                case 1:
                    moveStoneScr.isTopCollOn = false;
                    break;
                case 2:
                    moveStoneScr.isRightCollOn = false;
                    break;
                case 3:
                    moveStoneScr.isBottomCollOn = false;
                    break;
                case 4:
                    moveStoneScr.isLeftCollOn = false;
                    break;
            }
        }        
    }
}
