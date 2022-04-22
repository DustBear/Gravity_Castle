using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stage01_elevator_button : MonoBehaviour
{
    public float startYPos;
    public float finishYPos;
    public float elevatorMoveSpeed;
    public float movedDistance;
    public float purposeDistance;
    public float currentYPos;
    public float initialYPos;
    public bool isMove;
    public bool isButtonActivated;
    Rigidbody2D rigid;

    private void Start()
    {
        rigid = gameObject.GetComponent<Rigidbody2D>();
        isMove = false;
        isButtonActivated = false;
        transform.position = new Vector2(transform.position.x, transform.position.y);
        initialYPos = transform.position.y;
        purposeDistance = startYPos - finishYPos;
    }
    void Update()
    {
        if (!isMove) //��ư�� �������� �ϴ� ��Ȳ�̸� move()�Լ� ���� 
        {
            return;
        }

        currentYPos = transform.position.y;
        movedDistance = initialYPos - currentYPos;
        move();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player") //player �� ��ư �ݶ��̴� �ǵ帮�� isMove�� ���� �ǰ� ��ư �����̱� ���� 
        {
            if (isButtonActivated) //�̹� ��ư�� Ȱ��ȭ�Ǿ����Ƿ� �÷��̾ �ٽ� �ݶ��̴��� �ǵ���� ��ư�� ������ �־�� �� 
            {
                return;
            }
            isButtonActivated = true; 
            isMove = true;
        }
    }

    void move()
    {
        if (movedDistance >= purposeDistance) //��ư�� ������ �Ÿ��� ������ �Ÿ����� �ָ�
        {
            rigid.velocity = new Vector2(0, 0); //�����ϰ� isMove=false ���� 
            isMove = false; 
            return;
        }
        rigid.velocity = new Vector2(0, -elevatorMoveSpeed * Time.deltaTime);
    }
}
