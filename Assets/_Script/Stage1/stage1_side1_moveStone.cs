using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stage1_side1_moveStone : MonoBehaviour
{
    [SerializeField] float moveTime; //stone이 최고점에 도달하는 데 걸리는 시간
    float moveSpeed;
    [SerializeField] float limitSpeed; //떨어질 때 낙하속도 상한 
    [SerializeField] float floatTime; //stone이 최고점에 도달하고 정지해 있는 시간

    [SerializeField] Vector3 startPos;
    [SerializeField] Vector3 finishPos;
    [SerializeField] GameObject fallingSensor;

    public bool shouldStoneStart;
    public bool shouldStoneMove;
    public bool shouldStoneFall;
    Rigidbody2D rigid;

    public GameObject moveStoneGear;
    [SerializeField] float fallDelay;

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        shouldStoneStart = false;
        shouldStoneMove = false;

        moveTime = moveTime - 0.82f;
        moveSpeed = (finishPos - startPos).magnitude / moveTime; //moveSpeed는 moveTime에 따라 달라진다 
    }

    void Update()
    {
        fallDelay += Time.deltaTime;
        if (shouldStoneStart)
        {
            StartCoroutine(stoneStart());
        }
        if (shouldStoneMove)
        {
            stoneMove();
        }
        if (shouldStoneFall)
        {
            stoneFall();
        }
    }

    IEnumerator stoneStart()
    {
        shouldStoneStart = false;
     
        transform.position += new Vector3(0, 0.05f, 0);
        yield return new WaitForSeconds(0.08f);
        transform.position += new Vector3(0, -0.05f, 0);
        yield return new WaitForSeconds(0.08f);
        transform.position += new Vector3(0, 0.05f, 0);
        yield return new WaitForSeconds(0.08f);
        transform.position += new Vector3(0, -0.05f, 0);
        yield return new WaitForSeconds(0.08f);

        yield return new WaitForSeconds(0.5f); //레버를 당기고 0.82초가 지나야 돌이 움직이기 시작
        shouldStoneMove = true;
    }
    void stoneMove()
    {
        rigid.velocity = transform.up * moveSpeed;
        moveStoneGear.transform.Rotate(0, 0, 360/moveTime * Time.deltaTime); //stone이 움직이는 동안 기어는 시계방향으로 한바퀴 회전함

        Vector3 toStartPos = startPos - transform.position;
        Vector3 toFinishPos = finishPos - transform.position;

        if((Vector3.Dot(toStartPos,toFinishPos) > 0) && rigid.velocity.magnitude > 0) 
            //stone으로부터 finishPos 까지의 벡터의 방향이 stone-startPos 벡터의 방향과 같고 stone의 속도가 0이상이면 목표지점에 도착한것으로 봄
        {
            shouldStoneMove = false;
            rigid.velocity = Vector3.zero;
            transform.position = finishPos;
            moveStoneGear.transform.rotation = Quaternion.Euler(0, 0, 0);

            StartCoroutine("stoneFloat");
        }
    }
    
    IEnumerator stoneFloat() //stone이 공중에 머무른 상태에서 잠시 머무르다가 떨어짐
    {
        for (int index = 1; index <= 4; index++) //4번 반시계방향으로 90도씩 회전한 뒤 낙하
        {
            yield return new WaitForSeconds(floatTime / 4 - 0.1f);
            moveStoneGear.transform.Rotate(0, 0, -90);

            yield return new WaitForSeconds(0.05f);
            moveStoneGear.transform.Rotate(0, 0, -10);
            yield return new WaitForSeconds(0.05f);
            moveStoneGear.transform.Rotate(0, 0, 10);           
        }

        shouldStoneFall = true;
        fallDelay = 0; //타이머 초기화 
    }
 
    void stoneFall()
    {
        if (rigid.velocity.y <= -limitSpeed) //속도 상한에 도달하면 속도 고정 
        {
            rigid.velocity = -transform.up * limitSpeed;
        }
        else
        {
            float fallSpeed = 4.9f * fallDelay; //가속도 9.8인 등가속도운동(1/2*t^2에 비례)
            rigid.velocity = -transform.up * fallSpeed;
        }
        
        
        Vector3 toStartPos = startPos - transform.position;
        Vector3 toFinishPos = finishPos - transform.position;

        if ((Vector3.Dot(toStartPos, toFinishPos) > 0) && rigid.velocity.magnitude > 0)
        //stone으로부터 finishPos 까지의 벡터의 방향이 stone-startPos 벡터의 방향과 같고 stone의 속도가 0이상이면 목표지점에 도착한것으로 봄
        {
            shouldStoneFall = false;
            rigid.velocity = Vector3.zero;
            transform.position = startPos;
            moveStoneGear.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
}
