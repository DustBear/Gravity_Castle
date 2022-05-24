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
        
        stageIntro.GetComponent<Text>().color = new Color(1, 1, 1, 0); //인트로 끔 
        stageIntroDeco.GetComponent<Image>().color = new Color(1, 1, 1, 0); //인트로 데코 끔        

        player = GameObject.Find("Player");

        if (player.transform.position.y <= Active_Threshold) //씬이 처음 시작하는 것이면
        {
            Debug.Log(player.transform.position.y);
            Debug.Log("playertoStart");
            spr.sortingLayerID = SortingLayer.NameToID("stageStartPlayer"); //플레이어는 엘리베이터 뒤 레이어에서 시작해야 함
            isElevatorArrived = false; //아직 도착 안함
            spr.color = colorValue; //시작하면 플레이어 색은 어두움
            transform.localPosition = new Vector2(transform.position.x, startYpos); //아직 문은 올라가지 않은 상태로 고정
        }
        else
        {
            Debug.Log(player.transform.position.y);
            Debug.Log("playertoPlayer");
            spr.sortingLayerID = SortingLayer.NameToID("Player"); //플레이어 레이어 초기화(플레이어로)
            isElevatorArrived = true;
            transform.position = new Vector2(transform.position.x, finishYpos); //플레이어가 씬을 처음시작한 게 아니면 문은 올라간 채로 있어야 함 
            for (int index = 0; index < 3; index++)
            {
                coll[index].gameObject.SetActive(false); //collider는 사라져 있어야 함    
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
    
        if (transform.position.y >= finishYpos) //문이 완전히 열렸을 때 실행. 최초 1회만 실행
        {
            if (!isElevatorArrived)
            {
                transform.position = new Vector2(transform.position.x, finishYpos); //문 위치 고정
                isDoorMove = false;

                spr.sortingLayerID = SortingLayer.NameToID("Player"); //콜라이더 사라지면 레이어 다시 player 로 바꿔줌

                for (int index = 0; index < 3; index++)
                {
                    coll[index].gameObject.SetActive(false); //collider는 문이 도착하고 난 이후에 사라짐                
                }

                isElevatorArrived = true;
            }          
        }        
    }

    IEnumerator doorMove()
    {
        yield return new WaitForSeconds(delayTime); //엘리베이터가 도착하고 delayTime 만큼 지난 이후에 문이 올라가기 시작 
        rigid.velocity = new Vector2(0, doorSpeed);

        yield return new WaitForSeconds(2f);
        StartCoroutine("playerFadeIn"); //문 올라가고 2초 뒤 플레이어 밝아지기 시작 
    }

    public float fadeSpeed; //인트로 메시지 색이 밝아지거나 어두워지는 속도 
    public float IntroDelay; //인트로 메시지를 출력하는 시간 
    IEnumerator canvasFadeIn() //인트로 메시지를 밝아지게 만듦 
    {
        yield return new WaitForSecondsRealtime(1.5f);//바로 뜨는 것이 아니라 플레이어 색 밝아지고 1.5초 후 작동 
        while (true)
        {
            stageIntro.GetComponent<Text>().color = new Color(1, 1, 1, stageIntro.GetComponent<Text>().color.a + fadeSpeed * Time.deltaTime); //a값이 커짐(불투명해짐)
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

    IEnumerator canvasFadeOut() //인트로 메시지를 어두워지게 만듦 
    {
        while (true)
        {
            stageIntro.GetComponent<Text>().color = new Color(1, 1, 1, stageIntro.GetComponent<Text>().color.a - fadeSpeed * Time.deltaTime); //a값이 작아짐(투명해짐)
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
            spr.sortingLayerID = SortingLayer.NameToID("Player"); //플레이어 레이어 앞 레이어로 바꿔줌

            float newColor = spr.color.r + playerFadeSpeed;
            spr.color = new Color(newColor, newColor, newColor);
            yield return new WaitForFixedUpdate();

            if (spr.color.r >= 1) //만약 플레이어 밝기가 기준치에 도달하면
            {
                StartCoroutine("canvasFadeIn"); //인트로 메시지 출력 
                break;
            }
        }
    }
}
