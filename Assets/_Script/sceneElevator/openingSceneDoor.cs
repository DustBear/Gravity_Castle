using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class openingSceneDoor : MonoBehaviour
{
    SpriteRenderer spr;
    Rigidbody2D rigid;

    public GameObject sceneElavator; //�� ���������� ~> ���� ó�� �����ϴ� ������ �ƴ��� ���� �Ǵ�
    public GameObject sceneElevatorCover; //���������� �ö�� ���� Ŀ�� ��� ��� �Ⱥ��̰� �� 
    openingSceneEle seScript; //�� ������������ ��ũ��Ʈ

    public GameObject player;
    public GameObject stageIntro;
    public GameObject stageIntroDeco;

    public float startYpos; //���� ���������� �� Y��ǥ 
    public float finishYpos; //���� �ö����� �� Y��ǥ
    public float doorSpeed; //���� �̵��ӵ� 
    public float delayTime; //���������� ���� �� ���� �ö󰡴� �ð�
    public float playerFadeSpeed; //�÷��̾��� ���� ������ų� ��ο����� �ӵ�
  
    public bool isDoorMove;
    public float fadeSpeed; //��Ʈ�� �޽��� ���� ������ų� ��ο����� �ӵ� 
    public float IntroDelay; //��Ʈ�� �޽����� ����ϴ� �ð� 

    public Collider2D[] coll = new Collider2D[3];
    //���� ������ �� �� ������� �ݶ��̴�


    void Start()
    {
        //���� ã�Ƽ� �ֱ�
        spr = player.GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player");
        seScript = sceneElavator.GetComponent<openingSceneEle>();

        //�������� ��Ʈ�� 
        stageIntro.GetComponent<Text>().color = new Color(1, 1, 1, 0); //��Ʈ�� �� 
        stageIntroDeco.GetComponent<Image>().color = new Color(1, 1, 1, 0); //��Ʈ�� ���� ��        

        sceneElevatorCover.SetActive(true);
        sceneElevatorCover.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
        isDoorMove = false; //���� �ʱ�ȭ 
    }

    public void inactive() //���� �ö� ����+��Ȱ��ȭ�� ���·� ������Ű�� �Լ�. scene elevator���� �����Ŵ
    {
        Debug.Log("door inactive");
        transform.position= new Vector2(transform.position.x, finishYpos);
        isDoorMove = false;

        for (int index = 0; index < 3; index++)
        {
            coll[index].gameObject.SetActive(false); //collider�� ����� �־�� ��    
        }
    }

    public void active() //���� ���� �Լ� ��ü ������. scene elevator���� �����Ŵ 
    {
        Debug.Log("door active");
        transform.position = new Vector2(transform.position.x, startYpos);
        isDoorMove = true;
        StartCoroutine(doorMove());
    }

    void Update()
    {
        if(isDoorMove &&(transform.position.y >= finishYpos))
        {
            isDoorMove = false;
            rigid.velocity = new Vector2(0, 0); //�� ���� 
            Debug.Log("doorArrived");

            InputManager.instance.isJumpBlocked = false; //���������� �����ϸ� ���� ���� ���� 

            StartCoroutine("coverFadeOut");
            StartCoroutine("playerFadeIn"); //�� �ö󰡰� �÷��̾� ������� ���� 
        }
    }

    IEnumerator doorMove()
    {
        yield return new WaitForSeconds(delayTime); //���������Ͱ� �����ϰ� delayTime ��ŭ ���� ���Ŀ� ���� �ö󰡱� ���� 
        rigid.velocity = new Vector2(0, doorSpeed); //���� rigidBody�� ���� velocity �ְ� 

        /*
        while (true) //���� ���ڸ��� �����ߴ��� ��� Ȯ�� 
        {
            if(transform.position.y >= finishYpos)
            {
                rigid.velocity = new Vector2(0, 0); //�� ���� 
                Debug.Log("doorArrived");
                break; //���� ���������� while�� ��������                
            }
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(1.5f);
        StartCoroutine("playerFadeIn"); //�� �ö󰡰� 1.5�� �� �÷��̾� ������� ���� 
        */
    }
    IEnumerator playerFadeIn() //�� �ö󰡰� �÷��̾� ������� �Լ� 
    {
        spr = player.GetComponent<SpriteRenderer>();
        spr.sortingLayerID = SortingLayer.NameToID("Player"); //�÷��̾� ���̾� �� ���̾�� �ٲ���

        while (true)
        {
            float newColor = spr.color.r + playerFadeSpeed;
            spr.color = new Color(newColor, newColor, newColor);
            yield return new WaitForFixedUpdate();

            if (spr.color.r >= 1) //���� �÷��̾� ��Ⱑ ����ġ�� �����ϸ�
            {
                for (int index = 0; index < 3; index++)
                {
                    coll[index].gameObject.SetActive(false); //collider�� ������� ��  
                }

                StartCoroutine("canvasFadeIn"); //��Ʈ�� �޽��� ��� 
                break;
            }
        }
    }


    IEnumerator canvasFadeIn() //��Ʈ�� �޽����� ������� ���� 
    {
        yield return new WaitForSecondsRealtime(1f);//�ٷ� �ߴ� ���� �ƴ϶� �÷��̾� �� ������� 1�� �� �۵� 
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

    IEnumerator coverFadeOut() //���������� �����ϰ� ���� ��� ��ο������� ����
    {
        int index = 1;
        while (index <= 20)
        {
            sceneElevatorCover.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0.05f*index);
            index++;

            yield return new WaitForSeconds(0.05f);
        }
    }
}
