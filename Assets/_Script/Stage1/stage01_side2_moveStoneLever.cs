using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stage01_side2_moveStoneLever : MonoBehaviour
{
    public Sprite[] leverSprite;
    public GameObject moveWall;

    [SerializeField] bool isPlayerOn;
    SpriteRenderer spr;

    void Start()
    {
        spr = GetComponent<SpriteRenderer>();
        spr.sprite = leverSprite[0];
    }
    
    void Update()
    {
        if (isPlayerOn && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine("spriteAni"); //작동여부에 상관없이 레버 애니메이션은 작동
            moveWall.GetComponent<stage01_side2_moveWall>().stoneMove();
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
