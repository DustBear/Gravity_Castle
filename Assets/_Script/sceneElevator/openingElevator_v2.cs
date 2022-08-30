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
        if (GameManager.instance.shouldUseOpeningElevator) //엘리베이터를 사용할지 말지는 GM 이 결정 
        {
            transform.position = pos1;
            isElevatorArrived = false;
            GameManager.instance.nextPos = pos1 + new Vector2(0, 1.5f); //엘리베이터 위에 플레이어 생성 
            GameManager.instance.nextGravityDir = new Vector2(0, -1); //중력 방향 설정해 줌 

            rigid.velocity = new Vector2(0, -moveSpeed);
        }
        else
        {
            transform.position = pos2;
            isElevatorArrived = true;
        }


    }
    void Start()
    {
        InputManager.instance.isInputBlocked = false; //씬이 시작하면 inputBlock 해제 
    }
  
    void Update()
    {
        if (isElevatorArrived) return;
        stopCheck();
    }

    void stopCheck()
    {
        if(transform.position.y <= pos2.y)
        {
            rigid.velocity = Vector2.zero;
            isElevatorArrived = true;

            GameManager.instance.shouldUseOpeningElevator = false;
        }
    }
}
