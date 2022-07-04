using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stage01_side2_moveWall : MonoBehaviour
{
    [SerializeField] Vector3 pos1;
    [SerializeField] Vector3 pos2;
    [SerializeField] float moveTime; //�����̴� �� �ɸ��� �ð�
    public int curPos; //���� stone�� ��ġ�� ������� 
    public int initialPos; //������ ���� ��ġ 
    public bool isMoving; //�����̰� �ִ� ������ ���� ����x 

    public Sprite[] spriteGroup;
    SpriteRenderer spr;
    void Start()
    {
        spr = GetComponent<SpriteRenderer>();
        spr.sprite = spriteGroup[0];

        switch (initialPos)
        {
            case 1:
                transform.position = pos1;
                curPos = 1;
                break;
            case 2:
                transform.position = pos2;
                curPos = 2;
                break;
        }
    }
    
    void Update()
    {
        
    }

    public void stoneMove()
    {
        if (!isMoving)
        {
            if (curPos == 1) StartCoroutine(stoneMoveCor(2));
            else if (curPos == 2) StartCoroutine(stoneMoveCor(1));
        }
    }

    IEnumerator moveAni_right()
    {
        //moveTime ���� 4���� ȸ��(�ð����)
        for(int index=1; index<=20; index++)
        {
            spr.sprite = spriteGroup[index % 5];
            yield return new WaitForSeconds(moveTime / 19);
        }
    }
    IEnumerator moveAni_left()
    {
        //moveTime ���� 4���� ȸ��(�ݽð����)
        for (int index = 20; index >= 1; index--)
        {
            spr.sprite = spriteGroup[index % 5];
            yield return new WaitForSeconds(moveTime / 19);
        }
    }

    IEnumerator stoneMoveCor(int dirPos) //dirPos=1,2 �� �� ������ �̵� 
    {
        isMoving = true;

        float distance = (pos2 - pos1).magnitude; //�������� �� �Ÿ� 
        Vector3 direction = (pos1 - pos2).normalized; //pos1�� ��ǥ�� �� 

        for(int index=1; index<=3; index++) //3ȸ�� ���� ���� 
        {
            transform.position += direction * 0.05f;
            yield return new WaitForSeconds(0.05f);
            transform.position -= direction * 0.05f;
            yield return new WaitForSeconds(0.05f);
        }
        yield return new WaitForSeconds(0.5f);

        if(dirPos == 2)
        {
            direction = -direction;//���� �ݴ�� �ٲ���� �� 
        }

        float frameTime = moveTime / 100;
        switch (dirPos) //��� ���ư��� �ִϸ��̼� ���� 
        {
            case 1:
                StartCoroutine(moveAni_right());
                break;
            case 2:
                StartCoroutine(moveAni_left());
                break;
        }

        for(int index=1; index<=100; index++)
        {
            transform.position += (direction*distance)/100;
            yield return new WaitForSeconds(frameTime);
        }

        switch (dirPos) //�������� ������ �߻��� �� �����Ƿ� stone�� �����ϰ� ���� ��ǥ�� ������ ������ �ش� 
        {
            case 1:
                transform.position = pos1;
                curPos = 1;
                break;
            case 2:
                transform.position = pos2;
                curPos = 2;
                break;
        }

        isMoving = false;
    }
}
