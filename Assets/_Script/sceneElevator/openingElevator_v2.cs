using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class openingElevator_v2 : MonoBehaviour
{
    public Vector2 pos1; //내려오기 전 위치
    public Vector2 pos2; //내려온 후 위치 
    public float moveSpeed;

    public GameObject playerObj;
    Rigidbody2D rigid;
    
    bool isElevatorArrived; //엘리베이터가 pos2에 도착했는지의 여부 

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();

        if(GameManager.instance.gameData.curAchievementNum == 0) //achNum = 0 이면 새로운 스테이지가 시작한 것 ~> 엘리베이터 내려오는 것 부터 시작 
        {
            transform.position = pos1;
            isElevatorArrived = false;

            elevatorMove();
        }
        else
        {
            isElevatorArrived = true;
            transform.position = pos2;
        }
    }
    void Start()
    {
        
    }

    
    void Update()
    {
        if (isElevatorArrived) return;

        stopCheck();
    }

    void elevatorMove()
    {
        rigid.velocity = new Vector2(0, -moveSpeed);
    }

    void stopCheck()
    {
        if(transform.position.y <= pos2.y)
        {
            rigid.velocity = Vector2.zero;
            isElevatorArrived = true;
        }
    }
}
