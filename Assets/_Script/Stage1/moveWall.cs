using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveWall : MonoBehaviour
{
    [SerializeField] Vector3 pos1;
    [SerializeField] Vector3 pos2;
    public float moveTime; //움직이는 데 걸리는 시간
    public int curPos; //현재 stone의 위치가 어디인지 
    public int initialPos; //시작할 때의 위치 
    public bool isMoving; //움직이고 있는 동안은 레버 조작x 

    public Sprite[] spriteGroup;
    public int cycleNum; //spriteGroup 주기를 몇 번 돌릴지 
    SpriteRenderer spr;

    AudioSource sound;
    Rigidbody2D rigid;

    public AudioClip startSound;
    public AudioClip movingSound;
    public AudioClip arriveSound;

    private void Awake()
    {
        sound = GetComponent<AudioSource>();
        rigid = GetComponent<Rigidbody2D>();
    }
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
        //moveTime 동안 1바퀴 회전(시계방향)

        var waitFrame = new WaitForSeconds(moveTime / (spriteGroup.Length * cycleNum + 1));
        for(int count=1; count<=cycleNum; count++)
        {
            for (int index = 0; index < spriteGroup.Length; index++)
            {
                spr.sprite = spriteGroup[index];
                yield return waitFrame;
            }            
        }       
    }
    IEnumerator moveAni_left()
    {
        //moveTime 동안 1바퀴 회전(반시계방향)

        var waitFrame = new WaitForSeconds(moveTime / (spriteGroup.Length * cycleNum + 1));
        for (int count=1; count<=cycleNum; count++)
        {
            for (int index = spriteGroup.Length - 1; index >= 0; index--)
            {
                spr.sprite = spriteGroup[index];
                yield return waitFrame;
            }
        }       
    }

    IEnumerator stoneMoveCor(int dirPos) //dirPos=1,2 중 한 군데로 이동 
    {
        isMoving = true;

        float distance = (pos2 - pos1).magnitude; //움직여야 할 거리 
        Vector3 direction = (pos1 - pos2).normalized; //pos1이 목표일 때 

        sound.PlayOneShot(startSound);

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
       
        switch (dirPos) //기어 돌아가는 애니메이션 실행 
        {
            case 1:
                StartCoroutine(moveAni_right());
                break;
            case 2:
                StartCoroutine(moveAni_left());
                break;
        }

        sound.clip = movingSound;
        sound.Play();

        float moveSpeed = distance / moveTime;
        rigid.velocity = moveSpeed * direction;

        yield return new WaitForSeconds(moveTime-0.1f);

        sound.Stop();
        sound.PlayOneShot(arriveSound);

        yield return new WaitForSeconds(0.1f);
        rigid.velocity = Vector3.zero;

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
