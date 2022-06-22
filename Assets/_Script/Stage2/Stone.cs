using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stone : MonoBehaviour
{
    Rigidbody2D rigid;
    
    [SerializeField] Vector2 firstStonePos; //���̺�8 ���� ���� 
    [SerializeField] Vector2 secondStonePos; //���̺�9 ���� 
    [SerializeField] Vector2 thirdStonePos; //���̺�10 ���� ���� 

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
            rigid.bodyType = RigidbodyType2D.Static; //���� stone�� �������� �ȵ� 
        }else if(achieveNum == 9)
        {
            transform.position = secondStonePos;
            rigid.bodyType = RigidbodyType2D.Dynamic; //stone�� �߷� �ۿ��� ����
        }
        else
        {
            transform.position = thirdStonePos;
            rigid.bodyType = RigidbodyType2D.Dynamic;
        }
    }

    void Update()
    {
        // Restrict velocity ~> �߶��ϰų� ������ �� �ӵ��� �ʹ� �������� �ʵ��� ����
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
