using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movingProp : MonoBehaviour
{
    public GameObject prop;

    public Sprite[] leverSprite = new Sprite[3];
    //[0]: 레버가 정중앙에 위치, [1]: 오른쪽, [2]: 왼쪽  
    SpriteRenderer spr;
    Animator anim;

    bool isPlayerOn;
    bool isLeverActivated;
    
    public Vector2 pos1, pos2; //prop이 이동할 수 있는 범위의 양 끝 
    public int initPos; // posGroup의 인덱스로 지정
    public int periodNum; //pos1 ~ pos2 를 몇등분으로 나눌 것인지 
    Vector2[] posGroup;
    [SerializeField] int curPos; 
    public float propMoveSpeed;

    bool isCorWork; 
    bool isPropMove;

    public GameObject arrow;

    private void Awake()
    {       
        spr = GetComponent<SpriteRenderer>();
        anim = prop.GetComponent<Animator>();       
    }
    void Start()
    {
        spr.sprite = leverSprite[0];
        posGroup = new Vector2[periodNum+1]; 
        //전체 구간을 4등분 했다면 prop이 위치할 수 있는 중간지점은 5개가 생긴다 
        //[0] = pos1,  [periodNum] = pos2

        for(int index=0; index<=periodNum; index++)
        {
            posGroup[index] = pos1 + index * (pos2 - pos1) / periodNum;
        }

        prop.transform.position = posGroup[initPos]; 
        curPos = initPos;
        anim.SetFloat("animSpeed", 0);
        arrow.SetActive(false);
    }

    
    void Update()
    {
        if (isPlayerOn && Input.GetKeyDown(KeyCode.E))
        {
            isLeverActivated = !isLeverActivated; //활성화상태면 비활성화하고 비활성화상태면 활성화시킴 
            InputManager.instance.isInputBlocked = isLeverActivated;

            if (isLeverActivated)
            {
                arrow.SetActive(true);
            }
            else
            {
                arrow.SetActive(false);
            }
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
    IEnumerator leverAct(int leverDir) //-1 이면 이전 pos로, +1 이면 이후 pos 로 이동 
    {
        if (isCorWork) yield break; //아직 동작이 끝나지 않았으면 작동 무시 

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

    
    IEnumerator propMove(int aimPos) //1�̸� pos1 ������ ������ , 2�̸� pos2 ������ ������ 
    {
        if (isPropMove) yield break; ; //�۵����̸� ����

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
                    prop.transform.position = posGroup[curPos - 1]; //�������� ���� 
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
                    prop.transform.position = posGroup[curPos + 1]; //�������� ���� 
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
