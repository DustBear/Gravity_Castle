using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class openingSceneDoor : MonoBehaviour
{
    public float startYpos;
    public float finishYpos;
    public float doorSpeed;
    public float delayTime;
    public GameObject player;
    public float colorValue;
    public bool isColorBrighten;
    float curTime;
    public bool isDoorMove;
    
    public Collider2D[] coll = new Collider2D[3];
    SpriteRenderer spr;
    Rigidbody2D rigid;
    void Start()
    {
        spr = player.GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();

        if (player.transform.position.y <= -8) //���� ó�� �����ϴ� ���� �ƴϸ� �÷��̾� sortingLayer, color �����ؾ� �� 
        {
            spr.sortingLayerID = SortingLayer.NameToID("stageStartPlayer"); //�����ϸ� �÷��̾�� ���������� �� ���̾�� �����ؾ� ��
            spr.color = new Color(colorValue, colorValue, colorValue); //�����ϸ� �÷��̾� ���� ��ο�
            transform.localPosition = new Vector2(0, startYpos);
        }
        else
        {
            transform.localPosition = new Vector2(0, finishYpos); //�÷��̾ ���� ó�������� �� �ƴϸ� ���� �ö� ä�� �־�� �� 
        }
        
        isDoorMove = false;
    }

    void Update()
    {
        if (isDoorMove) 
        {
            StartCoroutine(doorMove()); 
        }
    
        if (transform.localPosition.y >= finishYpos)
        {
            transform.localPosition = new Vector2(0, finishYpos); 
            isDoorMove = false;

            spr.sortingLayerID = SortingLayer.NameToID("Player"); //�ݶ��̴� ������� ���̾� �ٽ� player �� �ٲ���

            for (int index = 0; index < 3; index++)
            {
                coll[index].gameObject.SetActive(false); //collider�� ���� �����ϰ� �� ���Ŀ� �����                
            }
        }

        if (isColorBrighten)
        {
            float tmpColor = spr.color.r;
            spr.color = new Color(tmpColor+ 0.01f, tmpColor + 0.01f, tmpColor + 0.01f); //20������ ���� ������ �����

            if(tmpColor >= 1)
            {
                isColorBrighten = false; //���� 1�� �ǰ� ���� ���̻� ����� �ʿ� ����
            }
        }
    }

    IEnumerator doorMove()
    {
        yield return new WaitForSeconds(delayTime); //���������Ͱ� �����ϰ� delayTime ��ŭ ���� ���Ŀ� ���� �ö󰡱� ���� 
        rigid.velocity = new Vector2(0, doorSpeed);

        yield return new WaitForSeconds(1.1f);    
        isColorBrighten = true; //���������� �� �ö󰡰� 1.1�� �� ���� ������� ������
        
    }
}
