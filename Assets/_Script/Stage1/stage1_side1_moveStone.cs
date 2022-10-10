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

    public bool shouldStoneStart;
    public bool shouldStoneMove;
    public bool shouldStoneFall;
    Rigidbody2D rigid;

    public GameObject moveStoneGear;
    float fallDelay;


    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();

        shouldStoneStart = false;
        shouldStoneMove = false;

        moveTime = moveTime - 0.82f;
        moveSpeed = (finishPos - startPos).magnitude / moveTime; //moveSpeed�� moveTime�� ���� �޶����� 
    }

    void Update()
    {
        fallDelay += Time.deltaTime;
        if (shouldStoneStart)
        {
            StartCoroutine(stoneStart());
        }       
    }

    IEnumerator stoneStart()
    {
        shouldStoneStart = false;

        Vector3 vibrateDir = (startPos - finishPos).normalized;

        //stone�� �����̱� �� �� �Ʒ��� ��¦ ������ 
        transform.position += vibrateDir * 0.05f;
        yield return new WaitForSeconds(0.08f);
        transform.position -= vibrateDir * 0.05f;
        yield return new WaitForSeconds(0.08f);
        transform.position -= vibrateDir * 0.05f;
        yield return new WaitForSeconds(0.08f);
        transform.position += vibrateDir * 0.05f;
        yield return new WaitForSeconds(0.08f);

        yield return new WaitForSeconds(0.5f); //������ ���� 0.82�ʰ� ������ ���� �����̱� ����

        //stone ������ ������ ��ǥ�������� �����̱� ���� 
        shouldStoneMove = true;
        while (shouldStoneMove)
        {
            stoneMove();
            yield return null;
        }
        
    }
    void stoneMove()
    {
        rigid.velocity = (finishPos - startPos).normalized * moveSpeed;
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

            StartCoroutine("stoneFloat");
        }
    }
    
    IEnumerator stoneFloat() //stone�� ���߿� �ӹ��� ���¿��� ��� �ӹ����ٰ� ������
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
        fallDelay = 0; //Ÿ�̸� �ʱ�ȭ 

        while (shouldStoneFall)
        {
            stoneFall();
            yield return null;
        }
    }
 
    void stoneFall()
    {
        int gravityScale = 3;
        if (rigid.velocity.y <= -limitSpeed) //�ӵ� ���ѿ� �����ϸ� �ӵ� ���� 
        {
            rigid.velocity = -(finishPos - startPos).normalized * limitSpeed;
        }
        else
        {
            float fallSpeed = gravityScale * 4.9f * fallDelay; //���ӵ� 9.8�� ��ӵ��(1/2*t^2�� ���)
            rigid.velocity = -(finishPos - startPos).normalized * fallSpeed;
        }
        
        
        Vector3 toStartPos = startPos - transform.position;
        Vector3 toFinishPos = finishPos - transform.position;

        if ((Vector3.Dot(toStartPos, toFinishPos) > 0) && rigid.velocity.magnitude > 0)
        //stone���κ��� finishPos ������ ������ ������ stone-startPos ������ ����� ���� stone�� �ӵ��� 0�̻��̸� ��ǥ������ �����Ѱ����� ��
        {
            shouldStoneFall = false;
            rigid.velocity = Vector3.zero;
            transform.position = startPos;
            moveStoneGear.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
}
