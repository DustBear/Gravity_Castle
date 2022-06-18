using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class openingSceneDoor : MonoBehaviour
{
    SpriteRenderer spr;
    Rigidbody2D rigid;

    public GameObject sceneElavator; //씬 엘리베이터 ~> 씬이 처음 시작하는 것인지 아닌지 여부 판단
    public GameObject sceneElevatorCover; //엘리베이터 올라온 이후 커버 덮어서 통로 안보이게 함 
    openingSceneEle seScript; //씬 엘리베이터의 스크립트

    public GameObject player;
    public GameObject stageIntro;
    public GameObject stageIntroDeco;

    public float startYpos; //문이 내려가있을 때 Y좌표 
    public float finishYpos; //문이 올라가있을 때 Y좌표
    public float doorSpeed; //문의 이동속도 
    public float delayTime; //엘리베이터 도착 후 문이 올라가는 시간
    public float playerFadeSpeed; //플레이어의 색이 밝아지거나 어두워지는 속도
  
    public bool isDoorMove;
    public float fadeSpeed; //인트로 메시지 색이 밝아지거나 어두워지는 속도 
    public float IntroDelay; //인트로 메시지를 출력하는 시간 

    public Collider2D[] coll = new Collider2D[3];
    //문이 열리고 난 뒤 사라지는 콜라이더


    void Start()
    {
        //변수 찾아서 넣기
        spr = player.GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player");
        seScript = sceneElavator.GetComponent<openingSceneEle>();

        //스테이지 인트로 
        stageIntro.GetComponent<Text>().color = new Color(1, 1, 1, 0); //인트로 끔 
        stageIntroDeco.GetComponent<Image>().color = new Color(1, 1, 1, 0); //인트로 데코 끔        

        sceneElevatorCover.SetActive(true);
        sceneElevatorCover.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
        isDoorMove = false; //인자 초기화 
    }

    public void inactive() //문이 올라간 상태+비활성화된 상태로 유지시키는 함수. scene elevator에서 실행시킴
    {
        Debug.Log("door inactive");
        transform.position= new Vector2(transform.position.x, finishYpos);
        isDoorMove = false;

        for (int index = 0; index < 3; index++)
        {
            coll[index].gameObject.SetActive(false); //collider는 사라져 있어야 함    
        }
    }

    public void active() //문을 여는 함수 전체 시퀸스. scene elevator에서 실행시킴 
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
            rigid.velocity = new Vector2(0, 0); //문 정지 
            Debug.Log("doorArrived");

            InputManager.instance.isJumpBlocked = false; //엘리베이터 도착하면 점프 제한 해제 

            StartCoroutine("coverFadeOut");
            StartCoroutine("playerFadeIn"); //문 올라가고 플레이어 밝아지기 시작 
        }
    }

    IEnumerator doorMove()
    {
        yield return new WaitForSeconds(delayTime); //엘리베이터가 도착하고 delayTime 만큼 지난 이후에 문이 올라가기 시작 
        rigid.velocity = new Vector2(0, doorSpeed); //문의 rigidBody에 일정 velocity 주고 

        /*
        while (true) //문이 제자리에 도착했는지 계속 확인 
        {
            if(transform.position.y >= finishYpos)
            {
                rigid.velocity = new Vector2(0, 0); //문 정지 
                Debug.Log("doorArrived");
                break; //만약 도착했으면 while문 빠져나감                
            }
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(1.5f);
        StartCoroutine("playerFadeIn"); //문 올라가고 1.5초 뒤 플레이어 밝아지기 시작 
        */
    }
    IEnumerator playerFadeIn() //문 올라가고 플레이어 밝아지는 함수 
    {
        spr = player.GetComponent<SpriteRenderer>();
        spr.sortingLayerID = SortingLayer.NameToID("Player"); //플레이어 레이어 앞 레이어로 바꿔줌

        while (true)
        {
            float newColor = spr.color.r + playerFadeSpeed;
            spr.color = new Color(newColor, newColor, newColor);
            yield return new WaitForFixedUpdate();

            if (spr.color.r >= 1) //만약 플레이어 밝기가 기준치에 도달하면
            {
                for (int index = 0; index < 3; index++)
                {
                    coll[index].gameObject.SetActive(false); //collider는 사라지게 함  
                }

                StartCoroutine("canvasFadeIn"); //인트로 메시지 출력 
                break;
            }
        }
    }


    IEnumerator canvasFadeIn() //인트로 메시지를 밝아지게 만듦 
    {
        yield return new WaitForSecondsRealtime(1f);//바로 뜨는 것이 아니라 플레이어 색 밝아지고 1초 후 작동 
        while (true)
        {
            stageIntro.GetComponent<Text>().color = new Color(1, 1, 1, stageIntro.GetComponent<Text>().color.a + fadeSpeed * Time.deltaTime); //a값이 커짐(불투명해짐)
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

    IEnumerator canvasFadeOut() //인트로 메시지를 어두워지게 만듦 
    {
        while (true)
        {
            stageIntro.GetComponent<Text>().color = new Color(1, 1, 1, stageIntro.GetComponent<Text>().color.a - fadeSpeed * Time.deltaTime); //a값이 작아짐(투명해짐)
            stageIntroDeco.GetComponent<Image>().color = new Color(1, 1, 1, stageIntro.GetComponent<Text>().color.a - fadeSpeed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
            if (stageIntro.GetComponent<Text>().color.a <= 0)
            {
                break;
            }
        }
    }

    IEnumerator coverFadeOut() //엘리베이터 도착하고 나면 통로 어두워지도록 만듦
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
