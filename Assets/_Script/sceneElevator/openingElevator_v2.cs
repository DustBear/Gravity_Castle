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
        if (GameManager.instance.shouldUseOpeningElevator) //���������͸� ������� ������ GM �� ���� 
        {
            transform.position = pos1;
            isElevatorArrived = false;
            GameManager.instance.nextPos = pos1 + new Vector2(0, 1.5f); //���������� ���� �÷��̾� ���� 
            GameManager.instance.nextGravityDir = new Vector2(0, -1); //�߷� ���� ������ �� 

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
        InputManager.instance.isInputBlocked = false; //���� �����ϸ� inputBlock ���� 
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
