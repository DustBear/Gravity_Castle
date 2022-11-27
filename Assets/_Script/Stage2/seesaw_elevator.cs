using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class seesaw_elevator : MonoBehaviour
{
    [SerializeField] Vector2 pos1; //pos1 �� �⺻ ����Ʈ ��ġ 
    [SerializeField] Vector2 pos2;
    [SerializeField] float moveSpeed;
    //pos1�� pos2 ���� �����ǥ�� ���� �� ��ġ�� Ŀ�� �� 

    Rigidbody2D rigid;
    [SerializeField] GameObject weight; //�ݴ��� ������ 
    [SerializeField] Vector2 weightDefaultPos; //�������� ����Ʈ ��ġ 

    float moveDistance; //pos1 �� pos2 ������ �Ÿ� 
    public bool isPlayerOn; //���� �÷��̾ ���������� ���� �ö� �ִ��� 

    private void Awake()
    {
        //���� pos1�� pos2�� ��ġ�� �߸� �Է��ߴٸ� �ڵ����� �ٲ� �� 
        if(pos1.x < pos2.x || pos1.y < pos2.y)
        {
            Vector2 tempPos = pos1;
            pos1 = pos2;
            pos2 = tempPos;
        }

        rigid = GetComponent<Rigidbody2D>();
        moveDistance = (pos1 - pos2).magnitude;
    }

    void Start()
    {
        
    }


    void Update()
    {
        elevatorMove();
        weightMove();
    }

    void elevatorMove()
    {
        if (isPlayerOn) //�÷��̾ ���������� ���� �ö� ���� �� 
        {            
            //�̹� ���������Ͱ� pos2 �� ���������� �÷��̾ ��� ���� �ö�Ÿ �ִٸ� pos2���� �����ؾ� �� 
            if (pos1.x == pos2.x && transform.position.y <= pos2.y) //y�� �������� �����̴� ��� 
            {
                rigid.velocity = Vector2.zero;
                transform.position = pos2;
                return;
            }
            if (pos1.y == pos2.y && transform.position.x <= pos2.x) //x�� �������� �����̴� ��� 
            {
                rigid.velocity = Vector2.zero;
                transform.position = pos2;
                return;
            }

            //���������ʹ� pos2 �������� ��ӿ�ϸ� �������� �� 
            rigid.velocity = (pos2 - pos1).normalized * moveSpeed;
        }
        else //�÷��̾ ���������Ϳ��� �������� �� 
        {            
            //�̹� ���������Ͱ� pos1 �� ���������� �÷��̾ ���� �ö� ���� �ʴٸ� pos1���� �����ؾ� �� 
            if (pos1.x == pos2.x && transform.position.y >= pos1.y) //y�� �������� �����̴� ��� 
            {
                rigid.velocity = Vector2.zero;
                transform.position = pos1;
                return;
            }
            if (pos1.y == pos2.y && transform.position.x >= pos1.x) //x�� �������� �����̴� ��� 
            {
                rigid.velocity = Vector2.zero;
                transform.position = pos1;
                return;
            }

            //���������ʹ� pos1 �������� ��ӿ�ϸ� �ö󰡾� �� 
            rigid.velocity = (pos1 - pos2).normalized * moveSpeed;
        }
    }

    void weightMove()
    {
        weight.transform.position = weightDefaultPos - new Vector2(transform.position.x - pos1.x, transform.position.y - pos1.y);
    }
}
