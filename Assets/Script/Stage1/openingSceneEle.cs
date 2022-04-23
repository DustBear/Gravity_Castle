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
            return; //���������Ͱ� �����ߴٸ� ���̻� ����� ���� ���� 
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
            rigid.velocity = new Vector2(0,0); //finishYpos�� �����ϸ� ���������� ���� 
            isMove = false; //isMove �� false�� ���� 
            elevatorDoor.GetComponent<openingSceneDoor>().isDoorMove = true;
            isArrived = true;
        }
    }

    void elevatorMove()
    {
        rigid.velocity = new Vector2(0, speed);
    }

}
