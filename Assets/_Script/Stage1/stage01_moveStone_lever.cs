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
            StartCoroutine("spriteAni"); //작동여부에 상관없이 레버 애니메이션은 작동
            if (moveStone.GetComponent<Rigidbody2D>().velocity.magnitude <= 0.05f) //돌이 바닥에 정지해 있을 때만 작동 가능
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
    IEnumerator spriteAni() //스프라이트 움직임으로 애니메이션 구현
    {
        spr.sprite = leverSprite[1];
        yield return new WaitForSeconds(0.4f);

        spr.sprite = leverSprite[0];
    }
}
