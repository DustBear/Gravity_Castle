using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class openingSceneEle : MonoBehaviour
{
    public float speed;
    public float finishYpos;
    public bool isMove;
    bool isArrived=false;
    public GameObject elevatorDoor;
    public Collider2D[] coll = new Collider2D[3];
    Rigidbody2D rigid;
    void Start()
    {
        isMove = true;
        rigid = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isArrived)
        {
            return; //엘리베이터가 도착했다면 더이상 고려할 것이 없다 
        }
        /*
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isMove = true;
        }
        */
        if (isMove)
        {
            elevatorMove();
        }

       
        if(transform.position.y >= finishYpos)
        {
            rigid.velocity = new Vector2(0,0); //finishYpos에 도착하면 엘리베이터 정지 
            isMove = false; //isMove 는 false로 설정 
            elevatorDoor.GetComponent<openingSceneDoor>().isDoorMove = true;
            isArrived = true;
        }
    }

    void elevatorMove()
    {
        rigid.velocity = new Vector2(0, speed);
    }

}
