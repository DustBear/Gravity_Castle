using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stage01_side2_moveWall : MonoBehaviour
{
    [SerializeField] Vector3 pos1;
    [SerializeField] Vector3 pos2;
    [SerializeField] float moveTime; //움직이는 데 걸리는 시간
    public int curPos; //현재 stone의 위치가 어디인지 
    public int initialPos; //시작할 때의 위치 
    public bool isMoving; //움직이고 있는 동안은 레버 조작x 

    public Sprite[] spriteGroup;
    SpriteRenderer spr;
    void Start()
    {
        spr = GetComponent<SpriteRenderer>();
        spr.sprite = spriteGroup[0];

        switch (initialPos)
        {
            case 1:
                transform.position = pos1;
                curPos = 1;
                break;
            case 2:
                transform.position = pos2;
                curPos = 2;
                break;
        }
    }
    
    void Update()
    {
        
    }

    public void stoneMove()
    {
        if (!isMoving)
        {
            if (curPos == 1) StartCoroutine(stoneMoveCor(2));
            else if (curPos == 2) StartCoroutine(stoneMoveCor(1));
        }
    }

    IEnumerator moveAni_right()
    {
        //moveTime 동안 4바퀴 회전(시계방향)
        for(int index=1; index<=20; index++)
        {
            spr.sprite = spriteGroup[index % 5];
            yield return new WaitForSeconds(moveTime / 19);
        }
    }
    IEnumerator moveAni_left()
    {
        //moveTime 동안 4바퀴 회전(반시계방향)
        for (int index = 20; index >= 1; index--)
        {
            spr.sprite = spriteGroup[index % 5];
            yield return new WaitForSeconds(moveTime / 19);
        }
    }

    IEnumerator stoneMoveCor(int dirPos) //dirPos=1,2 중 한 군데로 이동 
    {
        isMoving = true;

        float distance = (pos2 - pos1).magnitude; //움직여야 할 거리 
        Vector3 direction = (pos1 - pos2).normalized; //pos1이 목표일 때 

        for(int index=1; index<=3; index++) //3회에 걸쳐 진동 
        {
            transform.position += direction * 0.05f;
            yield return new WaitForSeconds(0.05f);
            transform.position -= direction * 0.05f;
            yield return new WaitForSeconds(0.05f);
        }
        yield return new WaitForSeconds(0.5f);

        if(dirPos == 2)
        {
            direction = -direction;//방향 반대로 바꿔줘야 함 
        }

        float frameTime = moveTime / 100;
        switch (dirPos) //기어 돌아가는 애니메이션 실행 
        {
            case 1:
                StartCoroutine(moveAni_right());
                break;
            case 2:
                StartCoroutine(moveAni_left());
                break;
        }

        for(int index=1; index<=100; index++)
        {
            transform.position += (direction*distance)/100;
            yield return new WaitForSeconds(frameTime);
        }

        switch (dirPos) //물리엔진 오차가 발생할 수 있으므로 stone이 도착하고 나면 좌표를 별도로 고정해 준다 
        {
            case 1:
                transform.position = pos1;
                curPos = 1;
                break;
            case 2:
                transform.position = pos2;
                curPos = 2;
                break;
        }

        isMoving = false;
    }
}
