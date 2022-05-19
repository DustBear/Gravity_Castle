using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class openingSceneEle : MonoBehaviour
{
    public GameObject player;
    public float speed;
    public float finishYpos;
    public bool isMove;
    bool isArrived=false;
    public GameObject elevatorDoor;
    public Collider2D[] coll = new Collider2D[3];
    public float Active_Threshold;
    public float startYpos;
    Rigidbody2D rigid;

    void Start()
    {
        transform.position = new Vector2(transform.position.x, startYpos);

        if(!(player.transform.position.y <= Active_Threshold)) //씬을 처음 시작하는 것이 아니면 엘리베이터 움직이지 않음 
        {
            isMove = false;
            transform.position = new Vector2(transform.position.x, finishYpos);
            return;
        }

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
        if (isMove)
        {
            elevatorMove();
        }

       
        if((transform.position.y >= finishYpos)&&isMove) //움직이다가 정지하는 것이 아니라 이미 도착해서 움직이지 않는 것이라면 문을 다시 올릴 필요가 없다 
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
