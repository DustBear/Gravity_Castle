using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movingProp : MonoBehaviour
{
    public GameObject prop;

    public Sprite[] leverSprite = new Sprite[3];
    //[0] �� ���, [1]�� ����, [2]�� ������ 
    SpriteRenderer spr;
    Animator anim;

    bool isPlayerOn;
    bool isLeverActivated;
    
    public Vector2 pos1, pos2; //prop�� �����̴� ����� �� ���� 
    public int initPos; // ���° posGroup���� ������ ������ 
    public int periodNum; //pos1 ~ pos2 ������ �� �������� ������ 
    Vector2[] posGroup;
    [SerializeField] int curPos; //0 ~ periodNum ������ pos ��ȣ 
    public float propMoveSpeed;


    bool isCorWork; //�ڷ�ƾ �۵��߿��� �ٸ� �ڷ�ƾ ���� ���ƾ� �� 
    bool isPropMove; //prop �����̴� �߿��� �ڷ�ƾ ���� ���ƾ� �� 

    private void Awake()
    {       
        spr = GetComponent<SpriteRenderer>();
        anim = prop.GetComponent<Animator>();       
    }
    void Start()
    {
        spr.sprite = leverSprite[0];
        posGroup = new Vector2[periodNum+1]; 
        // ex) ��ü ������ 4��еȴٸ� ������ 5���� �־�� ��
        //[0] = pos1,  [periodNum] = pos2

        for(int index=0; index<=periodNum; index++)
        {
            posGroup[index] = pos1 + index * (pos2 - pos1) / periodNum;
        }

        prop.transform.position = posGroup[initPos]; //prop ��ġ �ʱ�ȭ 
        curPos = initPos;
        anim.SetFloat("animSpeed", 0); //�����ϸ� ��Ϲ��� ȸ��x 
    }

    
    void Update()
    {
        if (isPlayerOn && Input.GetKeyDown(KeyCode.E))
        {
            isLeverActivated = !isLeverActivated; //�۵����̾����� ����, ������ ���¸� �۵� 
            InputManager.instance.isInputBlocked = isLeverActivated; //���� �۵��߿��� �÷��̾� ���� ���� 
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
    IEnumerator leverAct(int leverDir) //-1 �̸� ����, +1�̸� ���������� ���� ������ 
    {
        if (isCorWork) yield break; ; //�ڷ�ƾ �۵����̸� �ٸ� ��� ���� 

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
