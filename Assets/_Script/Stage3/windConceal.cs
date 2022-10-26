using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class windConceal : MonoBehaviour
{
    GameObject playerObj;
    Player playerScr;

    [SerializeField] int curWindType;
    // up:    0
    // right: 1
    // down:  2
    // left:  3

    private void Awake()
    {
        playerObj = GameObject.Find("Player");
        playerScr = playerObj.GetComponent<Player>();
    }

    void Start()
    {
        
    }

    
    void Update()
    {
        switch (curWindType)
        {
            case 0:
                transform.rotation = Quaternion.Euler(0, 0, 180f);
                break;
            case 1:
                transform.rotation = Quaternion.Euler(0, 0, 90f);
                break;
            case 2:
                transform.rotation = Quaternion.Euler(0, 0, 0);
                break;
            case 3:
                transform.rotation = Quaternion.Euler(0, 0, 270f);
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player" && !playerScr.isPlayerGrab) //플레이어가 박스를 잡고있을 땐 체크 x 
        {
            //playerScr.isWindConceal = true;
        }

        if(collision.tag == "UpWind")
        {
            curWindType = 0;
        }
        if (collision.tag == "RightWind")
        {
            curWindType = 1;
        }
        if (collision.tag == "DownWind")
        {
            curWindType = 2;
        }
        if (collision.tag == "LeftWind")
        {
            curWindType = 3;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            //playerScr.isWindConceal = false;
        }


    }
}
