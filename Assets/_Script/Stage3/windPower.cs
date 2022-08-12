using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class windPower : MonoBehaviour
{
    GameObject playerObj;
    Player playerScript;
    Rigidbody2D rigid;
    public float windForceSize; //플레이어에게 windZone 내에서 가해지는 가속도 
    public float maxWindForce; //플레이어가 windZone 내에서 가지는 최고 속도 

    public float InertialTime = 1; //관성력이 적용되는 시간 
    public float timer; //플레이어가 관성력을 받는 시간 타이머  
    bool shouldTimerWork;
    float exitPlayerVel; //플레이어가 windZone을 빠져나오는 순간 가지는 속도 

    private void Awake()
    {
        playerObj = GameObject.Find("Player");
        shouldTimerWork = false;

        rigid = playerObj.GetComponent<Rigidbody2D>();
        playerScript = playerObj.GetComponent<Player>();
    }

    private void OnTriggerEnter2D(Collider2D collision) //각 windZone 마다 고유한 windForce를 player 스크립트에 대입 
    {
        playerScript.windForce = windForceSize;
        playerScript.maxWindSpeed = maxWindForce;
    }

    //플레이어는 무조건 좌우 화살표에 의해서만 horizontal 축 방향의 속도가 결정됨 
    //horizontal wind 의 영향을 받아 비행하다가 영향권을 벗어나는 순간 곧바로 정지하게 됨. 
    //부자연스럽기 때문에 관성의 영향을 받는 운동을 구현할 필요가 있음

    //플레이어가 windZone 을 벗어나는 순간부터 windForceSize 에 비례하여 x축 방향으로 추가적인 속도를 받음 
    //추가적인 속도는 시간이 갈수록 조금씩 줄어듦
    //플레이어가 땅에 닿거나 벽에 부딪히는 순간 속도 추가는 정지 
    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("player.transform.up: " + playerObj.transform.up + "/ windZone.transform.up: " + transform.up);
        Debug.Log("내적의 값: " + Vector2.Dot(transform.up, playerObj.transform.up));


        if (Vector2.Dot(transform.up, playerObj.transform.up) != 1) //플레이어와 wind의 transform.up 내적이 1이 아님 ~> 내적은 0 ~> 바람은 플레이어의 x 축으로 부는 중 
        {
            Debug.Log(playerObj.transform.up);
            Debug.Log(transform.up);
            Debug.Log(Vector2.Dot(transform.up, playerObj.transform.up));

            Debug.Log("timer kill");
            return;
        }
        else //플레이어와 wind 의 방향이 수직일 때 ~> 플레이어는 x 축 방향으로 추가속도를 받아야 함 
        {
            playerScript.isPlayerExitFromWInd = true;

            shouldTimerWork = true;
            timer = InertialTime; //타이머 초기화 
            exitPlayerVel = transform.InverseTransformDirection(rigid.velocity).x; //플레이어가 windZone 을 빠져나오는 순간 x축 방향의 속도
            Debug.Log("exit Velocity: " + exitPlayerVel);
        }
    }

    private void Update()
    {        
        if (shouldTimerWork)
        {
            timer -= Time.deltaTime; //타이머 시간 계속 줄어듦 
            float addedVel = exitPlayerVel * (timer / InertialTime); //가속되는 addedVel 은 시간이 갈수록 공기저항을 받아 감쇄 

            Vector2 locVel = transform.InverseTransformDirection(rigid.velocity); //locvel 은 플레이어의 로컬좌표계 기준 이동방향    
            if(Mathf.Abs(locVel.x) < 0.1f) //속도가 이정도로 작아졌다는 것은 벽에 부딪혀 정지해다는 뜻 ~> addedVel 은 사라져야 한다 
            {
                playerScript.isPlayerExitFromWInd = false; //다시 player 움직임 주도권을 Player 스크립트가 가져감 
                timer = 0;
                shouldTimerWork = false;

                return;
            }

            locVel = new Vector2(InputManager.instance.horizontal * playerScript.walkSpeed + addedVel, locVel.y); //locVel 에 새로운 addedVel 더해 줌 

            rigid.velocity = transform.TransformDirection(locVel); //rigid 에 월드좌표 속도 할당

            if (timer <= 0 || playerObj.GetComponent<Player>().isGrounded) //타이머가 다 떨어지거나 플레이어가 지면에 착지하면 속도 추가 중지 
            {
                playerScript.isPlayerExitFromWInd = false; //다시 player 움직임 주도권을 Player 스크립트가 가져감 
                timer = 0;
                shouldTimerWork = false;
            }
        }      
    }
}
