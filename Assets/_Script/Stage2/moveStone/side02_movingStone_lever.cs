using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class side02_movingStone_lever : MonoBehaviour
{
    public GameObject movingStone;
    public Sprite[] leverSprite; //0이면 불 꺼진 상태, 1이면 오른쪽 화살표 켜짐, 2이면 왼쪽 화살표 켜짐
    //0~>1~>2 로 가면서 레버가 활성화되고 3이면 불이 들어와서 stone이 움직임 

    SpriteRenderer spr;

    bool isPlayerOn; //플레이어가 센서 내에 들어와 있는가? 
    [SerializeField] int moveType; //타입이 1이면 가로 이동, 2이면 세로 이동 
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
            if (msScript.isOnMove) return; //박스가 움직이는 동안은 명령어 인식 x 
            msScript.stoneMove(moveType, targetPos);

            spr.sprite = leverSprite[3];
            spriteNum = 3;
        }

        if (isStoneMoveOnLastFrame && !msScript.isOnMove) //이전 프레임에서는 움직이고 있었는데 현재 프레임에서는x ~> 방금 stone이 정지한 것 
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

    IEnumerator leverAni(int type) //1이면 활성화 2이면 비활성화 
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
