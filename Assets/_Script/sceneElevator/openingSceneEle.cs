using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class openingSceneEle : MonoBehaviour
{
    public GameObject elevatorDoor;
    public GameObject player;
    public Color playerColor; //플레이어 어두워질 때의 색상 
    public Vector2 playerPos; //씬 시작했을 때 플레이어의 위치 
    openingSceneDoor doorScript;

    SpriteRenderer spr; //플레이어 색상 
    BoxCollider2D sensor; //이 엘리베이터의 콜라이더 

    public float speed; //엘리베이터 속도 
    public float startYpos; //엘리베이터가 움직이기 시작하는 Y좌표 
    public float finishYpos; //엘리베이터가 정지하는 Y좌표
    public bool isMove; //엘리베이터가 움직이고 있는지?

    public bool isActive; //엘리베이터를 작동시켜야 하는지의 여부
    public bool isArrived; //엘리베이터가 도착했는지의 여부
    bool isInAvtiveChecked; //엘리베이터의 비활성화 작업이 시행됐는지?
            
    Rigidbody2D rigid;
    AudioSource sound;

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        doorScript = elevatorDoor.GetComponent<openingSceneDoor>();
        spr = player.GetComponent<SpriteRenderer>(); //spr은 플레이어의 색상 제어를 위한 변수 
        sensor = GetComponent<BoxCollider2D>();
        
        sensorCheck();       
    }

    void sensorCheck()
    {
        Vector2 colliderCenter = new Vector2(transform.position.x + sensor.offset.x, transform.position.y + sensor.offset.y); //센서의 중심 좌표 

        float rightX_pos = colliderCenter.x + sensor.size.x / 2;
        float leftX_pos = colliderCenter.x - sensor.size.x / 2;
        float topY_pos = colliderCenter.y + sensor.size.y / 2;
        float bottomY_pos = colliderCenter.y - sensor.size.y / 2;

        float playerX = playerPos.x;
        float playerY = playerPos.y;

        if((leftX_pos < playerX) && (playerX < rightX_pos))
        {
            if((bottomY_pos < playerY) && (playerY < topY_pos))
            {
                isActive = true; //플레이어가 콜라이더 안에 들어가 있으면 isActive = true
                transform.position = new Vector2(transform.position.x, startYpos);

                spr.sortingLayerID = SortingLayer.NameToID("stageStartPlayer"); //플레이어는 엘리베이터 뒤 레이어에서 시작해야 함
                spr.color = playerColor; //시작하면 플레이어 색은 어두움

                InputManager.instance.isJumpBlocked = true;
                elevatorMove();
            }
        }

        else //엘리베이터 작동시킬 필요x
        {
            doorScript.inactive(); //엘리베이터 문 비활성화시켜야 함 
            transform.position = new Vector2(transform.position.x, finishYpos);

            spr.sortingLayerID = SortingLayer.NameToID("Player"); //플레이어 레이어 초기화(플레이어로)
            spr.color = new Color(1, 1, 1, 1); //플레이어 색상 기본값으로 
            doorScript.sceneElevatorCover.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 1);         
        }
    }

    private void OnTriggerStay2D(Collider2D collision) //이 함수 실행되고 다음 update 문에서 엘리베이터 작동 시작
    {
        /*
        if (isActive)
        {
            return; //이미 한 번 반응해서 isActive를 활성화시켰다면 그 다음부터는 중복해서 명령 내릴 필요 x 
        }

        if(collision.tag == "Player")
        {
            Debug.Log("coll");
            isActive = true; //플레이어가 엘리베이터 위에 있으므로 엘리베이터를 활성화시켜야 한다. 
        }   
        */
    }

    void Update()
    {
        /*
        if (isArrived || (!isActive))
        {
            if (isInAvtiveChecked)
            {
                return; //이미 했던 작업이면 다시할 필요 x 
            }

            doorScript.inactive(); //엘리베이터 문 비활성화시켜야 함 
            transform.position = new Vector2(transform.position.x, finishYpos);

            spr.sortingLayerID = SortingLayer.NameToID("Player"); //플레이어 레이어 초기화(플레이어로)
            spr.color = new Color(1, 1, 1, 1); //플레이어 색상 기본값으로 
            isInAvtiveChecked = true;

            return;
         
            //(1)플레이어가 엘리베이터를 타지 않아서 작동시킬 필요가 없거나
            //(2)맨 처음엔 작동했지만 이미 도착했다면 
            //더이상 고려할 것이 없다 + 엘리베이터는 이미 올라가 있어야 함 
        }

        else if (isActive) //플레이어가 엘리베이터 위에 타 있어서 작동시켜야 함 
        {
            spr.sortingLayerID = SortingLayer.NameToID("stageStartPlayer"); //플레이어는 엘리베이터 뒤 레이어에서 시작해야 함
            spr.color = playerColor; //시작하면 플레이어 색은 어두움

            elevatorMove();
        }
        */

       
        if((transform.position.y >= finishYpos)&&isMove) //움직이다가 정지하는 것이 아니라 이미 도착해서 움직이지 않는 것이라면 문을 다시 올릴 필요가 없다 
        {            
            isMove = false; //isMove 는 false로 설정 

            rigid.velocity = new Vector2(0,0); //finishYpos에 도착하면 엘리베이터 정지 

            doorScript.active(); //엘리베이터 문 움직이기 시작 
        }

        /*
        엘리베이터가 이미 도착해서 움직이지 않는 상태는 isActive,isMove 가 둘 다 false이다. isArrived는 true이다.          
         */
    }

    void elevatorMove()
    {
        isMove = true;
        rigid.velocity = new Vector2(0, speed);
    }

}
