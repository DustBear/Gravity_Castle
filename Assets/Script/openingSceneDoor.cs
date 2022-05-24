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
    public GameObject stageIntro;
    public GameObject stageIntroDeco;
    float curTime;
    public bool isDoorMove;
    public float playerFadeSpeed;

    public float Active_Threshold;
    bool isElevatorArrived = true;
    
    public Collider2D[] coll = new Collider2D[3];
    SpriteRenderer spr;
    Rigidbody2D rigid;
    void Start()
    {
        spr = player.GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();
        
        stageIntro.GetComponent<Text>().color = new Color(1, 1, 1, 0); //��Ʈ�� �� 
        stageIntroDeco.GetComponent<Image>().color = new Color(1, 1, 1, 0); //��Ʈ�� ���� ��        

        player = GameObject.Find("Player");

        if (player.transform.position.y <= Active_Threshold) //���� ó�� �����ϴ� ���̸�
        {
            Debug.Log(player.transform.position.y);
            Debug.Log("playertoStart");
            spr.sortingLayerID = SortingLayer.NameToID("stageStartPlayer"); //�÷��̾�� ���������� �� ���̾�� �����ؾ� ��
            isElevatorArrived = false; //���� ���� ����
            spr.color = colorValue; //�����ϸ� �÷��̾� ���� ��ο�
            transform.localPosition = new Vector2(transform.position.x, startYpos); //���� ���� �ö��� ���� ���·� ����
        }
        else
        {
            Debug.Log(player.transform.position.y);
            Debug.Log("playertoPlayer");
            spr.sortingLayerID = SortingLayer.NameToID("Player"); //�÷��̾� ���̾� �ʱ�ȭ(�÷��̾��)
            isElevatorArrived = true;
            transform.position = new Vector2(transform.position.x, finishYpos); //�÷��̾ ���� ó�������� �� �ƴϸ� ���� �ö� ä�� �־�� �� 
            for (int index = 0; index < 3; index++)
            {
                coll[index].gameObject.SetActive(false); //collider�� ����� �־�� ��    
            }
        }
        
        isDoorMove = false;
    }

    void Update()
    {
        if (isDoorMove) 
        {
            StartCoroutine(doorMove()); 
        }
    
        if (transform.position.y >= finishYpos) //���� ������ ������ �� ����. ���� 1ȸ�� ����
        {
            if (!isElevatorArrived)
            {
                transform.position = new Vector2(transform.position.x, finishYpos); //�� ��ġ ����
                isDoorMove = false;

                spr.sortingLayerID = SortingLayer.NameToID("Player"); //�ݶ��̴� ������� ���̾� �ٽ� player �� �ٲ���

                for (int index = 0; index < 3; index++)
                {
                    coll[index].gameObject.SetActive(false); //collider�� ���� �����ϰ� �� ���Ŀ� �����                
                }

                isElevatorArrived = true;
            }          
        }        
    }

    IEnumerator doorMove()
    {
        yield return new WaitForSeconds(delayTime); //���������Ͱ� �����ϰ� delayTime ��ŭ ���� ���Ŀ� ���� �ö󰡱� ���� 
        rigid.velocity = new Vector2(0, doorSpeed);

        yield return new WaitForSeconds(2f);
        StartCoroutine("playerFadeIn"); //�� �ö󰡰� 2�� �� �÷��̾� ������� ���� 
    }

    public float fadeSpeed; //��Ʈ�� �޽��� ���� ������ų� ��ο����� �ӵ� 
    public float IntroDelay; //��Ʈ�� �޽����� ����ϴ� �ð� 
    IEnumerator canvasFadeIn() //��Ʈ�� �޽����� ������� ���� 
    {
        yield return new WaitForSecondsRealtime(1.5f);//�ٷ� �ߴ� ���� �ƴ϶� �÷��̾� �� ������� 1.5�� �� �۵� 
        while (true)
        {
            stageIntro.GetComponent<Text>().color = new Color(1, 1, 1, stageIntro.GetComponent<Text>().color.a + fadeSpeed * Time.deltaTime); //a���� Ŀ��(����������)
            stageIntroDeco.GetComponent<Image>().color = new Color(1, 1, 1, stageIntro.GetComponent<Text>().color.a + fadeSpeed * Time.deltaTime);
            yield return new WaitForFixedUpdate();
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
            yield return new WaitForFixedUpdate();
            if (stageIntro.GetComponent<Text>().color.a <= 0)
            {
                break;
            }
        }
    }

    IEnumerator playerFadeIn()
    {
        SpriteRenderer spr = player.GetComponent<SpriteRenderer>();
        while (true)
        {
            spr.sortingLayerID = SortingLayer.NameToID("Player"); //�÷��̾� ���̾� �� ���̾�� �ٲ���

            float newColor = spr.color.r + playerFadeSpeed;
            spr.color = new Color(newColor, newColor, newColor);
            yield return new WaitForFixedUpdate();

            if (spr.color.r >= 1) //���� �÷��̾� ��Ⱑ ����ġ�� �����ϸ�
            {
                StartCoroutine("canvasFadeIn"); //��Ʈ�� �޽��� ��� 
                break;
            }
        }
    }
}
