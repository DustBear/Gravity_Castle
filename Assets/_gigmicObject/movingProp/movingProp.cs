using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movingProp : MonoBehaviour
{
    public GameObject prop;

    public Sprite[] leverSprite = new Sprite[3];
    //[0] 이 가운데, [1]이 왼쪽, [2]이 오른쪽 
    SpriteRenderer spr;
    Animator anim;

    bool isPlayerOn;
    bool isLeverActivated;

    public Vector2 pos1, pos2; //prop이 움직이는 경로의 양 끝점 
    public int initPos; // 몇번째 posGroup에서 시작할 것인지 
    public int periodNum; //pos1 ~ pos2 구간을 몇 조각으로 나눌지 
    Vector2[] posGroup;
    [SerializeField] int curPos; //0 ~ periodNum 까지의 pos 번호 
    public float propMoveSpeed;


    bool isCorWork; //코루틴 작동중에는 다른 코루틴 실행 막아야 함 
    bool isPropMove; //prop 움직이는 중에는 코루틴 실행 막아야 함 

    private void Awake()
    {       
        spr = GetComponent<SpriteRenderer>();
        anim = prop.GetComponent<Animator>();       
    }
    void Start()
    {
        spr.sprite = leverSprite[0];
        posGroup = new Vector2[periodNum+1]; 
        // ex) 전체 구간이 4등분된다면 정점은 5개가 있어야 함
        //[0] = pos1,  [periodNum] = pos2

        for(int index=0; index<=periodNum; index++)
        {
            posGroup[index] = pos1 + index * (pos2 - pos1) / periodNum;
        }

        prop.transform.position = posGroup[initPos]; //prop 위치 초기화 
        anim.SetFloat("animSpeed", 0); //시작하면 톱니바퀴 회전x 
    }

    
    void Update()
    {
        if (isPlayerOn && Input.GetKeyDown(KeyCode.E))
        {
            isLeverActivated = !isLeverActivated; //작동중이었으면 해제, 해제된 상태면 작동 
            InputManager.instance.isInputBlocked = isLeverActivated; //레버 작동중에는 플레이어 조작 금지 
        }

        if (isLeverActivated)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                StartCoroutine(leverAct(-1));
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                StartCoroutine(leverAct(1));
            }
        }
    }
    IEnumerator leverAct(int leverDir) //-1 이면 왼쪽, +1이면 오른쪽으로 레버 움직임 
    {
        if (isCorWork) yield break; ; //코루틴 작동중이면 다른 명령 무시 

        isCorWork = true;

        if(leverDir == -1)
        {
            spr.sprite = leverSprite[1];
            StartCoroutine(propMove(1));
            yield return new WaitForSeconds(0.4f);
        }
        else if(leverDir == 1)
        {
            spr.sprite = leverSprite[2];
            StartCoroutine(propMove(2));
            yield return new WaitForSeconds(0.4f);
        }
        spr.sprite = leverSprite[0];

        isCorWork = false;
    }

    
    IEnumerator propMove(int aimPos) //1이면 pos1 쪽으로 움직임 , 2이면 pos2 쪽으로 움직임 
    {
        if (isPropMove) yield break; ; //작동중이면 무시

        isPropMove = true;

        if(aimPos == 1) 
        {
            if (curPos == 0)
            {
                isPropMove = false;
                yield break;
            }

            anim.SetFloat("animSpeed", 3f);
            while (true)
            {               
                prop.transform.position = Vector3.MoveTowards(prop.transform.position, posGroup[curPos - 1], Time.deltaTime * propMoveSpeed);
                if((new Vector2(prop.transform.position.x, prop.transform.position.y) - posGroup[curPos -1]).magnitude <= 0.05f)
                {
                    prop.transform.position = posGroup[curPos - 1]; //오차범위 수정 
                    curPos--;

                    anim.SetFloat("animSpeed", 0);
                    break;
                }
                yield return null;
            }              
        }
        else
        {
            if (curPos == periodNum)
            {
                isPropMove = false;
                yield break;
            }

            anim.SetFloat("animSpeed", -3f);
            while (true)
            {
                prop.transform.position = Vector3.MoveTowards(prop.transform.position, posGroup[curPos + 1], Time.deltaTime * propMoveSpeed);
                if ((new Vector2(prop.transform.position.x, prop.transform.position.y) - posGroup[curPos + 1]).magnitude <= 0.05f)
                {
                    prop.transform.position = posGroup[curPos + 1]; //오차범위 수정 
                    curPos++;

                    anim.SetFloat("animSpeed", 0);
                    break;
                }
                yield return null;
            }
        }

        isPropMove = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && collision.transform.up == transform.up)
        {
            isPlayerOn = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            isPlayerOn = false;
        }
    }
}
