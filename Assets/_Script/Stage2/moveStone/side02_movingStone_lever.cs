using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class side02_movingStone_lever : MonoBehaviour
{
    public GameObject movingStone;
    public Sprite[] leverSprite; //0�̸� �� ���� ����, 1�̸� ������ ȭ��ǥ ����, 2�̸� ���� ȭ��ǥ ����
    //0~>1~>2 �� ���鼭 ������ Ȱ��ȭ�ǰ� 3�̸� ���� ���ͼ� stone�� ������ 

    SpriteRenderer spr;

    bool isPlayerOn; //�÷��̾ ���� ���� ���� �ִ°�? 
    [SerializeField] int moveType; //Ÿ���� 1�̸� ���� �̵�, 2�̸� ���� �̵� 
    [SerializeField] float targetPos;

    bool isStoneMoveOnLastFrame;
    side02_movingStone msScript;

    int spriteNum;
    void Start()
    {
        spr = GetComponent<SpriteRenderer>();
        msScript = movingStone.GetComponent<side02_movingStone>();

        spr.sprite = leverSprite[0];
        spriteNum = 0;
    }

    void Update()
    {
        if(isPlayerOn && Input.GetKeyDown(KeyCode.E))
        {
            if (msScript.isOnMove) return; //�ڽ��� �����̴� ������ ��ɾ� �ν� x 
            msScript.stoneMove(moveType, targetPos);

            spr.sprite = leverSprite[3];
            spriteNum = 3;
        }

        if (isStoneMoveOnLastFrame && !msScript.isOnMove) //���� �����ӿ����� �����̰� �־��µ� ���� �����ӿ�����x ~> ��� stone�� ������ �� 
        {
            if (isPlayerOn)
            {
                spr.sprite = leverSprite[2];
                spriteNum = 2;
            }
            else if (!isPlayerOn)
            {
                spr.sprite = leverSprite[0];
            }
        }
        isStoneMoveOnLastFrame = msScript.isOnMove;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            isPlayerOn = true;
            if (!msScript.isOnMove)
            {
                spr.sprite = leverSprite[2];
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            isPlayerOn = false;
            if (!msScript.isOnMove)
            {
                spr.sprite = leverSprite[0];
            }           
        }
    }

    IEnumerator leverActive()
    {
        StopAllCoroutines();

        spr.sprite = leverSprite[0];
        yield return new WaitForSeconds(0.05f);
        spr.sprite = leverSprite[1];
        yield return new WaitForSeconds(0.05f);
        spr.sprite = leverSprite[2];
    }
    IEnumerator leverDeActive()
    {
        StopAllCoroutines();

        spr.sprite = leverSprite[2];
        yield return new WaitForSeconds(0.05f);
        spr.sprite = leverSprite[1];
        yield return new WaitForSeconds(0.05f);
        spr.sprite = leverSprite[0];
    }

    IEnumerator leverAni(int type) //1�̸� Ȱ��ȭ 2�̸� ��Ȱ��ȭ 
    {
        if(type == 1)
        {
            for(int index = spriteNum; index<=2; index++)
            {
                spr.sprite = leverSprite[index];
                yield return new WaitForSeconds(0.05f);
            }
        }
    }
}
