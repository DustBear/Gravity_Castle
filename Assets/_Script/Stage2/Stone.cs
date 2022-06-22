using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stone : MonoBehaviour
{
    Rigidbody2D rigid;
    
    [SerializeField] Vector2 firstStonePos; //세이브8 이전 시점 
    [SerializeField] Vector2 secondStonePos; //세이브9 시점 
    [SerializeField] Vector2 thirdStonePos; //세이브10 이후 시점 

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        int achieveNum = GameManager.instance.gameData.curAchievementNum;
        if(achieveNum <= 8)
        {
            transform.position = firstStonePos;
            rigid.bodyType = RigidbodyType2D.Static; //아직 stone이 떨어지면 안됨 
        }else if(achieveNum == 9)
        {
            transform.position = secondStonePos;
            rigid.bodyType = RigidbodyType2D.Dynamic; //stone은 중력 작용을 받음
        }
        else
        {
            transform.position = thirdStonePos;
            rigid.bodyType = RigidbodyType2D.Dynamic;
        }
    }

    void Update()
    {
        // Restrict velocity ~> 추락하거나 움직일 때 속도가 너무 빨라지지 않도록 조절
        if (rigid.bodyType == RigidbodyType2D.Dynamic)
        {
            rigid.velocity = new Vector2(Mathf.Clamp(rigid.velocity.x, -20f, 20f), Mathf.Clamp(rigid.velocity.y, -20f, 20f));
        }

    }
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            rigid.bodyType = RigidbodyType2D.Static;
        }
        if (other.gameObject.tag == "Cannon")
        {
            rigid.gravityScale = 10;
        }
    }

    void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            rigid.bodyType = RigidbodyType2D.Dynamic;
        }
    }
}
