using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class openingElevator_v2 : MonoBehaviour
{
    public Vector2 pos1; //�������� �� ��ġ
    public Vector2 pos2; //������ �� ��ġ 
    public float moveSpeed;

    public GameObject playerObj;
    Rigidbody2D rigid;
    
    bool isElevatorArrived; //���������Ͱ� pos2�� �����ߴ����� ���� 

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();

        if(GameManager.instance.gameData.curAchievementNum == 0) //achNum = 0 �̸� ���ο� ���������� ������ �� ~> ���������� �������� �� ���� ���� 
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
