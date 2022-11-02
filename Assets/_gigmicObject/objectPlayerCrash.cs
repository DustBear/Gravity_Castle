using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class objectPlayerCrash : MonoBehaviour
{
    //이 스크립트가 붙어있는 오브젝트는 일정 속도 이상으로 낙하하다가 플레이어와 부딪히면 플레이어가 죽음 

    Rigidbody2D rigid;
    public float thresholdSpeed;
    //이 속도 이상으로 플레이어와 충돌하면 사망 

    bool shouldVelCheck = true;

    private void Awake()
    {
        if(GetComponent<Rigidbody2D>() != null)
        {
            //rigidBody가 존재한다면 가져옴 
            rigid = GetComponent<Rigidbody2D>(); 
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(rigid == null)
        {
            return;
        }

        //매 업데이트 현재 속도벡터 구해서 저장 
        if (shouldVelCheck)
        {
            curVel = rigid.velocity;
        }
    }
    GameObject playerObj;
    Player playerScr;
    Vector2 curVel;

    IEnumerator playerCrash()
    {
        Vector2 movingDir = new Vector2(curVel.x, curVel.y); //충돌시점에서 오브젝트 이동방향 
        Vector2 playerDir = playerObj.transform.position - transform.position; //오브젝트에서 플레이어까지의 거리 

        Vector2 toDir = playerDir - movingDir;
        //float degree = Mathf.Atan2(toDir.y, toDir.x) * Mathf.Rad2Deg;
        //
        float degree = Mathf.Acos(Vector2.Dot(movingDir, playerDir) / (movingDir.magnitude * playerDir.magnitude)) * Mathf.Rad2Deg;
        //현재 오브젝트의 이동방향 벡터와 플레이어와의 상대적 위치벡터 사이의 각 ~> 충돌각 의미 

        Debug.Log("movingDIr: " + movingDir +  ", playerDir: " + playerDir + ", degree: " + degree);

        if(Mathf.Abs(degree) <= 45f && curVel.magnitude >= thresholdSpeed) //기준치 이상 속도로 충돌하며 충돌각의 절댓값이 45도 이하일 때 
        {
            playerScr = playerObj.GetComponent<Player>();
            yield return playerScr.StartCoroutine(playerScr.Die()); //플레이어 충돌로 사망
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        shouldVelCheck = false; 
        //플레이어와 충돌하면 속도측정 중지
        //따라서 충돌 직전의 속도가 기준이 되게 됨 

        if(collision.gameObject.tag == "Player")
        {
            Debug.Log("player Detect, speed: " + curVel.magnitude);

            playerObj = collision.gameObject;
            playerScr = playerObj.GetComponent<Player>();

            if (!playerScr.isDieCorWork)
            {
                StartCoroutine(playerCrash());
            }
        }

        shouldVelCheck = true;
    }
}
