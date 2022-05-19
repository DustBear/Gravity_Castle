using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class openingSceneDoor : MonoBehaviour
{
    public float startYpos;
    public float finishYpos;
    public float doorSpeed;
    public float delayTime;
    public GameObject player;
    public Color colorValue;
    public bool isColorBrighten;
    public GameObject stageIntro;
    public GameObject stageIntroDeco;
    float curTime;
    public bool isDoorMove;

    public float Active_Threshold;
    
    public Collider2D[] coll = new Collider2D[3];
    SpriteRenderer spr;
    Rigidbody2D rigid;
    void Start()
    {
        spr = player.GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();

        stageIntro.GetComponent<Text>().color = new Color(1, 1, 1, 0);
        stageIntroDeco.GetComponent<Image>().color = new Color(1, 1, 1, 0);

        if (player.transform.position.y <= Active_Threshold) //���� ó�� �����ϴ� ���� �ƴϸ� �÷��̾� sortingLayer, color �����ؾ� �� 
        {
            spr.sortingLayerID = SortingLayer.NameToID("stageStartPlayer"); //�����ϸ� �÷��̾�� ���������� �� ���̾�� �����ؾ� ��
            spr.color = colorValue; //�����ϸ� �÷��̾� ���� ��ο�
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
                StartCoroutine("canvasFadeIn"); //�÷��̾ ������ ������� ���� �������� ��Ʈ�� ��� 
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

    public float fadeSpeed; //��Ʈ�� �޽��� ���� ������ų� ��ο����� �ӵ� 
    public float IntroDelay; //��Ʈ�� �޽����� ����ϴ� �ð� 
    IEnumerator canvasFadeIn() //��Ʈ�� �޽����� ������� ���� 
    {
        while (true)
        {
            stageIntro.GetComponent<Text>().color = new Color(1, 1, 1, stageIntro.GetComponent<Text>().color.a + fadeSpeed * Time.deltaTime); //a���� Ŀ��(����������)
            stageIntroDeco.GetComponent<Image>().color = new Color(1, 1, 1, stageIntro.GetComponent<Text>().color.a + fadeSpeed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
            if(stageIntro.GetComponent<Text>().color.a >= 1)
            {
                yield return new WaitForSecondsRealtime(IntroDelay);
                StartCoroutine("canvasFadeOut");
                break;
            }
        }     
    }

    IEnumerator canvasFadeOut() //��Ʈ�� �޽����� ��ο����� ���� 
    {
        while (true)
        {
            stageIntro.GetComponent<Text>().color = new Color(1, 1, 1, stageIntro.GetComponent<Text>().color.a - fadeSpeed * Time.deltaTime); //a���� �۾���(��������)
            stageIntroDeco.GetComponent<Image>().color = new Color(1, 1, 1, stageIntro.GetComponent<Text>().color.a - fadeSpeed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
            if (stageIntro.GetComponent<Text>().color.a <= 0)
            {
                break;
            }
        }
    }
}
