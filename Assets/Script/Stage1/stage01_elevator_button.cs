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
        if (!isMove) //버튼이 움직여야 하는 상황이면 move()함수 실행 
        {
            return;
        }

        currentYPos = transform.position.y;
        movedDistance = initialYPos - currentYPos;
        move();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player") //player 가 버튼 콜라이더 건드리면 isMove가 참이 되고 버튼 움직이기 시작 
        {
            if (isButtonActivated) //이미 버튼이 활성화되었으므로 플레이어가 다시 콜라이더를 건드려도 버튼은 가만히 있어야 함 
            {
                return;
            }
            isButtonActivated = true; 
            isMove = true;
        }
    }

    void move()
    {
        if (movedDistance >= purposeDistance) //버튼이 움직인 거리가 설정된 거리보다 멀면
        {
            rigid.velocity = new Vector2(0, 0); //정지하고 isMove=false 지정 
            isMove = false; 
            return;
        }
        rigid.velocity = new Vector2(0, -elevatorMoveSpeed * Time.deltaTime);
    }
}
