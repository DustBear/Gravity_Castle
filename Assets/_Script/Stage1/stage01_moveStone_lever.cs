using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stage01_moveStone_lever : MonoBehaviour
{
    public GameObject moveStone;
    public stage1_side1_moveStone moveStoneScript;
    public Sprite[] leverSprite;

    [SerializeField] bool isPlayerOn;
    SpriteRenderer spr;

    void Start()
    {
        moveStoneScript = moveStone.GetComponent<stage1_side1_moveStone>();
        spr = GetComponent<SpriteRenderer>();
        spr.sprite = leverSprite[0];
    }

    void Update()
    {
        if (isPlayerOn && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine("spriteAni"); //�۵����ο� ������� ���� �ִϸ��̼��� �۵�
            if (moveStone.GetComponent<Rigidbody2D>().velocity.magnitude <= 0.05f) //���� �ٴڿ� ������ ���� ���� �۵� ����
            {
                moveStoneScript.shouldStoneMove = true;
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && collision.transform.rotation == transform.rotation)
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
    IEnumerator spriteAni() //��������Ʈ ���������� �ִϸ��̼� ����
    {
        spr.sprite = leverSprite[1];
        yield return new WaitForSeconds(0.4f);

        spr.sprite = leverSprite[0];
    }
}
