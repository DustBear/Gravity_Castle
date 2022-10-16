using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dynamicBox : MonoBehaviour
{   
    public GameObject player; //�÷��̾��� ȸ������ �������� ���Ϲ����� ���� 

    public bool isMoveHorizontal; //�÷����� ���η� �����̸� true, ���η� �����̸� false
    int moveDirection;
    /*
    _ 1 _
    4 _ 2 //�÷��̾� �Ӹ��� ���ϴ� ���� 
    _ 3 _   
    */

    Rigidbody2D rigid;
    public bool isCollide = false;
    void Start()
    {        
        rigid = GetComponent<Rigidbody2D>();
        rigid.bodyType = RigidbodyType2D.Kinematic;
    }

    // Update is called once per frame
    void Update()
    {      
        directionCheck();
        bodyTypeCheck();
    }
    void directionCheck()
    {       
        if (player.transform.up == new Vector3(0, 1, 0)) //�÷��̾� �Ӹ��� ������ ���� 
        {
            moveDirection = 1;
        }
        else if (player.transform.up == new Vector3(1, 0, 0)) //�÷��̾� �Ӹ��� �������� ���� 
        {
            moveDirection = 2;
        }
        else if (player.transform.up == new Vector3(0, -1, 0)) //�÷��̾� �Ӹ��� �Ʒ����� ���� 
        {
            moveDirection = 3;
        }
        else //�÷��̾� �Ӹ��� ������ ���� 
        {
            moveDirection = 4;
        }
    }

    void bodyTypeCheck()
    {
        if (isMoveHorizontal) //���η� �����̴� �ڽ��� �� ~> moveDirection = 2,4 ���� �߷��ۿ� �޾ƾ� �� 
        {
            if(moveDirection == 2 || moveDirection == 4)
            {
                if (isCollide)
                {
                    return;
                }
                rigid.bodyType = RigidbodyType2D.Dynamic;              
            }
            else
            {
                rigid.bodyType = RigidbodyType2D.Kinematic;
            }
        }
        else //���η� �����̴� �ڽ��� �� ~> moveDirection = 1,3 ���� �߷��ۿ� �޾ƾ� �� 
        {
            if (moveDirection == 1 || moveDirection == 3)
            {
                if (isCollide)
                {
                    return;
                }
                rigid.bodyType = RigidbodyType2D.Dynamic;
            }
            else
            {
                rigid.bodyType = RigidbodyType2D.Kinematic;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        /*
        if (collision.gameObject.tag == "Platform")
        {
            isCollide = true;
            rigid.bodyType = RigidbodyType2D.Kinematic;
        }
        */
    }
}
