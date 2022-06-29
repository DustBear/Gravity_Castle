using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stage1_side1_moveStone : MonoBehaviour
{
    [SerializeField] float moveTime; //stone�� �ְ����� �����ϴ� �� �ɸ��� �ð�
    float moveSpeed;
    [SerializeField] float limitSpeed; //������ �� ���ϼӵ� ���� 
    [SerializeField] float floatTime; //stone�� �ְ����� �����ϰ� ������ �ִ� �ð�

    [SerializeField] Vector3 startPos;
    [SerializeField] Vector3 finishPos;
    [SerializeField] GameObject fallingSensor;
    

    public bool shouldStoneMove;
    public bool shouldStoneFall;
    Rigidbody2D rigid;

    public GameObject moveStoneGear;

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        shouldStoneMove = false;
        shouldStoneFall = false;

        moveSpeed = (finishPos - startPos).magnitude / moveTime; //moveSpeed�� moveTime�� ���� �޶����� 
    }

    void Update()
    {
        if (shouldStoneMove)
        {
            stoneMove();
        }
        if (shouldStoneFall)
        {
            stoneFall();
        }
    }

    void stoneMove()
    {
        rigid.velocity = transform.up * moveSpeed;
        moveStoneGear.transform.Rotate(0, 0, 360/moveTime * Time.deltaTime); //stone�� �����̴� ���� ���� �ð�������� �ѹ��� ȸ����

        Vector3 toStartPos = startPos - transform.position;
        Vector3 toFinishPos = finishPos - transform.position;

        if((Vector3.Dot(toStartPos,toFinishPos) > 0) && rigid.velocity.magnitude > 0) 
            //stone���κ��� finishPos ������ ������ ������ stone-startPos ������ ����� ���� stone�� �ӵ��� 0�̻��̸� ��ǥ������ �����Ѱ����� ��
        {
            shouldStoneMove = false;
            rigid.velocity = Vector3.zero;
            transform.position = finishPos;
            moveStoneGear.transform.rotation = Quaternion.Euler(0, 0, 0);

            StartCoroutine("stoneShake");
        }
    }
    
    IEnumerator stoneShake() //stone�� ���߿� �ӹ��� ���¿��� ��鸮�ٰ� �ٽ� ������
    {
        for (int index = 1; index <= 4; index++) //4�� �ݽð�������� 90���� ȸ���� �� ����
        {
            yield return new WaitForSeconds(floatTime / 4 - 0.1f);
            moveStoneGear.transform.Rotate(0, 0, -90);

            yield return new WaitForSeconds(0.05f);
            moveStoneGear.transform.Rotate(0, 0, -10);
            yield return new WaitForSeconds(0.05f);
            moveStoneGear.transform.Rotate(0, 0, 10);           
        }

        shouldStoneFall = true;
        rigid.bodyType = RigidbodyType2D.Dynamic;
    }
 
    void stoneFall()
    {
        rigid.velocity = new Vector3(0, rigid.velocity.y, 0); //�� ������ �̵����� �ʵ��� ���� 
        if(rigid.velocity.y >= limitSpeed)
        {
            rigid.velocity = new Vector3(0, -limitSpeed, 0);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject== fallingSensor && shouldStoneFall) //�����ϴٰ� fallingSensor�� ������
        {
            shouldStoneFall = false;
            rigid.velocity = Vector3.zero;
            transform.position = startPos;
            rigid.bodyType = RigidbodyType2D.Kinematic;
        }
    }
}
