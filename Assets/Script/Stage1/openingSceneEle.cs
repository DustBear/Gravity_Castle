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
    Rigidbody2D rigid;
    void Start()
    {
        if(!(player.transform.position.y <= -8)) //���� ó�� �����ϴ� ���� �ƴϸ� ���������� �������� ���� 
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
            return; //���������Ͱ� �����ߴٸ� ���̻� ����� ���� ���� 
        }      
        if (isMove)
        {
            elevatorMove();
        }

       
        if((transform.position.y >= finishYpos)&&isMove) //�����̴ٰ� �����ϴ� ���� �ƴ϶� �̹� �����ؼ� �������� �ʴ� ���̶�� ���� �ٽ� �ø� �ʿ䰡 ���� 
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
