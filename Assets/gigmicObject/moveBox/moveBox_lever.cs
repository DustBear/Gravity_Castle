using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveBox_lever : MonoBehaviour
{
    public GameObject moveStone;
    Rigidbody2D rigid;
    SpriteRenderer spr;
    public Sprite[] spriteGroup; //[0]=idle  [1]=left  [2]=right

    public float moveSpeed;

    bool isPlayerOn;
    bool isLeverAct;

    public Vector3 leftArrowDir; //���� ȭ��ǥ�� ���� �� �̵��� ���� 
    public Vector3 rightArrowDir; //������ ȭ��ǥ�� ���� �� �̵��� ����

    public float minXPos;
    public float maxXPos;
    void Start()
    {
        rigid = moveStone.GetComponent<Rigidbody2D>();
        spr = GetComponent<SpriteRenderer>();

        spr.sprite = spriteGroup[0];
    }

    
    void Update()
    {
        if(isPlayerOn && Input.GetKeyDown(KeyCode.E))
        {
            if (!isLeverAct)
            {
                isLeverAct = true;
                InputManager.instance.isInputBlocked = true;
            }
            else
            {
                isLeverAct = false;
                InputManager.instance.isInputBlocked = false;
            }
        }

        if(isLeverAct && Input.GetKey(KeyCode.LeftArrow))
        {
            if (moveStone.transform.position.x < minXPos)
            {
                rigid.velocity = Vector3.zero;
                return;
            }

            rigid.velocity = leftArrowDir * moveSpeed;
            spr.sprite = spriteGroup[1];
        }
        else if(isLeverAct && Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.LeftArrow))
        {
            if (moveStone.transform.position.x > maxXPos)
            {
                rigid.velocity = Vector3.zero;
                return;
            }

            rigid.velocity = rightArrowDir * moveSpeed;
            spr.sprite = spriteGroup[2];
        }
        else if(isLeverAct && Input.GetKeyUp(KeyCode.LeftArrow) && !Input.GetKeyDown(KeyCode.RightArrow))
        {
            rigid.velocity = Vector3.zero;//���� ȭ��ǥ���� ���� �ð� ������ ȭ��ǥ ���� ������ �ʰ� ���� �� 
            spr.sprite = spriteGroup[0];
        }
        else if (isLeverAct && !Input.GetKeyDown(KeyCode.LeftArrow) && Input.GetKeyUp(KeyCode.RightArrow))
        {
            rigid.velocity = Vector3.zero;//������ ȭ��ǥ���� ���� �ð� ���� ȭ��ǥ ���� ������ �ʰ� ���� �� 
            spr.sprite = spriteGroup[0];
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player") isPlayerOn = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player") isPlayerOn = false;
        rigid.velocity = Vector3.zero; //�÷��̾ ���� ���� ������ ������ stone �� ���� 
        spr.sprite = spriteGroup[0];
    }
}
