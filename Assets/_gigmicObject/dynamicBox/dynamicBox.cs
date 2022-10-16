using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dynamicBox : MonoBehaviour
{   
    public GameObject player; //플레이어의 회전각을 기준으로 낙하방향을 정함 

    public bool isMoveHorizontal; //플랫폼이 가로로 움직이면 true, 세로로 움직이면 false
    int moveDirection;
    /*
    _ 1 _
    4 _ 2 //플레이어 머리가 향하는 방향 
    _ 3 _   
    */

    Rigidbody2D rigid;
    public bool isCollide = false;
    void Start()
    {        
        rigid = GetComponent<Rigidbody2D>();
        rigid.bodyType = RigidbodyType2D.Kinematic;
    }

    // Update is called once per frame
    void Update()
    {      
        directionCheck();
        bodyTypeCheck();
    }
    void directionCheck()
    {       
        if (player.transform.up == new Vector3(0, 1, 0)) //플레이어 머리가 위쪽을 향함 
        {
            moveDirection = 1;
        }
        else if (player.transform.up == new Vector3(1, 0, 0)) //플레이어 머리가 오른쪽을 향함 
        {
            moveDirection = 2;
        }
        else if (player.transform.up == new Vector3(0, -1, 0)) //플레이어 머리가 아래쪽을 향함 
        {
            moveDirection = 3;
        }
        else //플레이어 머리가 왼쪽을 향함 
        {
            moveDirection = 4;
        }
    }

    void bodyTypeCheck()
    {
        if (isMoveHorizontal) //가로로 움직이는 박스일 때 ~> moveDirection = 2,4 에서 중력작용 받아야 함 
        {
            if(moveDirection == 2 || moveDirection == 4)
            {
                if (isCollide)
                {
                    return;
                }
                rigid.bodyType = RigidbodyType2D.Dynamic;              
            }
            else
            {
                rigid.bodyType = RigidbodyType2D.Kinematic;
            }
        }
        else //세로로 움직이는 박스일 때 ~> moveDirection = 1,3 에서 중력작용 받아야 함 
        {
            if (moveDirection == 1 || moveDirection == 3)
            {
                if (isCollide)
                {
                    return;
                }
                rigid.bodyType = RigidbodyType2D.Dynamic;
            }
            else
            {
                rigid.bodyType = RigidbodyType2D.Kinematic;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        /*
        if (collision.gameObject.tag == "Platform")
        {
            isCollide = true;
            rigid.bodyType = RigidbodyType2D.Kinematic;
        }
        */
    }
}
