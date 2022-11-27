using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class seesaw_elevator : MonoBehaviour
{
    [SerializeField] Vector2 pos1; //pos1 이 기본 디폴트 위치 
    [SerializeField] Vector2 pos2;
    [SerializeField] float moveSpeed;
    //pos1이 pos2 보다 상대좌표에 대해 더 수치가 커야 함 

    Rigidbody2D rigid;
    [SerializeField] GameObject weight; //반대쪽 무게추 
    [SerializeField] Vector2 weightDefaultPos; //무게추의 디폴트 위치 

    float moveDistance; //pos1 과 pos2 사이의 거리 
    public bool isPlayerOn; //현재 플레이어가 엘리베이터 위에 올라가 있는지 

    private void Awake()
    {
        //만약 pos1과 pos2의 위치를 잘못 입력했다면 자동으로 바꿔 줌 
        if(pos1.x < pos2.x || pos1.y < pos2.y)
        {
            Vector2 tempPos = pos1;
            pos1 = pos2;
            pos2 = tempPos;
        }

        rigid = GetComponent<Rigidbody2D>();
        moveDistance = (pos1 - pos2).magnitude;
    }

    void Start()
    {
        
    }


    void Update()
    {
        elevatorMove();
        weightMove();
    }

    void elevatorMove()
    {
        if (isPlayerOn) //플레이어가 엘리베이터 위에 올라가 있을 때 
        {            
            //이미 엘리베이터가 pos2 에 도달했으며 플레이어가 계속 위에 올라타 있다면 pos2에서 정지해야 함 
            if (pos1.x == pos2.x && transform.position.y <= pos2.y) //y축 방향으로 움직이는 경우 
            {
                rigid.velocity = Vector2.zero;
                transform.position = pos2;
                return;
            }
            if (pos1.y == pos2.y && transform.position.x <= pos2.x) //x축 방향으로 움직이는 경우 
            {
                rigid.velocity = Vector2.zero;
                transform.position = pos2;
                return;
            }

            //엘리베이터는 pos2 방향으로 등속운동하며 내려가야 함 
            rigid.velocity = (pos2 - pos1).normalized * moveSpeed;
        }
        else //플레이어가 엘리베이터에서 내려왔을 때 
        {            
            //이미 엘리베이터가 pos1 에 도달했으며 플레이어가 위에 올라가 있지 않다면 pos1에서 정지해야 함 
            if (pos1.x == pos2.x && transform.position.y >= pos1.y) //y축 방향으로 움직이는 경우 
            {
                rigid.velocity = Vector2.zero;
                transform.position = pos1;
                return;
            }
            if (pos1.y == pos2.y && transform.position.x >= pos1.x) //x축 방향으로 움직이는 경우 
            {
                rigid.velocity = Vector2.zero;
                transform.position = pos1;
                return;
            }

            //엘리베이터는 pos1 방향으로 등속운동하며 올라가야 함 
            rigid.velocity = (pos1 - pos2).normalized * moveSpeed;
        }
    }

    void weightMove()
    {
        weight.transform.position = weightDefaultPos - new Vector2(transform.position.x - pos1.x, transform.position.y - pos1.y);
    }
}
