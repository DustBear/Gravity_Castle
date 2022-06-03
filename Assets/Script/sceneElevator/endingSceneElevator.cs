using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class endingSceneElevator : MonoBehaviour
{
    public GameObject elevatorDoor; //엘리베이터 문 
    public float elevatorSpeed; //엘리베이터 올라가는 속도 
    public float doorSpeed; //엘리베이터 문 올라가는 속도 

    public float eleStartYPos; //엘리베이터의 시작위치 
    public float sceneDelay; //엘리베이터 올라가고 나서 다음 씬으로 넘어가는 데 걸리는 시간 

    public float doorStartYPos; //엘리베이터 문 시작위치 
    public float doorFinishYPos; //엘리베이터 문 도착위치

    public Vector2 nextScenePos; //다음 씬에서 스폰될 위치 설정 
    public int nextScene; //다음 씬 번호 
    public float playerFadeSize; //플레이어는 어느 정도로 어두워 질 것인지 
    public float playerFadeSpeed; //플레이어 어두워지는 속도 

    bool isDoorArrived; 

    public GameObject[] collGroup = new GameObject[4]; //플레이어 가두는 콜라이더 
    GameObject playerObject;
    Player player;
    public GameObject interactText;
    void Start()
    {
        playerObject = GameObject.Find("Player");
        player = playerObject.GetComponent<Player>();       
        interactText.SetActive(false);
 
        GetComponent<BoxCollider2D>().enabled = true;
        isDoorArrived = false;

        transform.position = new Vector2(transform.position.x, eleStartYPos); //엘리베이터 위치 초기화 
        GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0); //엘리베이터 속도 초기화

        elevatorDoor.transform.localPosition = new Vector2(0, doorStartYPos); //엘리베이터 문 위치 초기화
        elevatorDoor.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0); //엘리베이터 문 속도 초기화 

        for (int index = 0; index < 4; index++)
        {
            collGroup[index].SetActive(false);
        }
    }

    void Update()
    {
        if(elevatorDoor.transform.localPosition.y >= doorFinishYPos) //엘리베이터 문이 도착하면 
        {
            if (!isDoorArrived) //최초 1회 실행 
            {
                elevatorDoor.transform.localPosition = new Vector2(0, doorFinishYPos); //엘리베이터 문 위치 고정
                elevatorDoor.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0); //엘리베이터 문 속도 0으로 
                StartCoroutine("playerFadeOut");

                isDoorArrived = true;
            }          
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        interactText.SetActive(true);
        //플레이어가 문 앞에 서면 상호작용 텍스트 활성화 
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        interactText.SetActive(false);
        //플레이어가 문 앞에서 벗어나면 상호작용 텍스트 비활성화  
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (Input.GetKeyDown(KeyCode.E)) //플레이어가 문 앞에 선 상태에서 E 누르면 작동
        {      
            for(int index=0; index<4; index++)
            {
                collGroup[index].SetActive(true); //플레이어가 엘리베이터 밖으로 못나가게 묶음              
            }
            doorOpen(); //엘리베이터 문 열림 
            GetComponent<BoxCollider2D>().enabled = false; //엘리베이터 시퀸스에 들어가면 센서는 꺼야 함 
        }
    }

    void doorOpen()
    {
        elevatorDoor.GetComponent<Rigidbody2D>().velocity = new Vector2(0, doorSpeed);
    }

    void elevatorDepart() //엘리베이터 출발 
    {
        GetComponent<Rigidbody2D>().velocity = new Vector2(0, elevatorSpeed);
        Invoke("moveToNextScene", sceneDelay); //sceneDelay 후 다음 씬으로 이동 
    }

    void moveToNextScene()
    {
        if (!GameManager.instance.shouldStartAtSavePoint)
        {
            GameManager.instance.nextPos = nextScenePos;
            GameManager.instance.nextGravityDir = Physics2D.gravity.normalized;
            GameManager.instance.nextState = Player.curState;
            SceneManager.LoadScene(nextScene);
        }
    }

    IEnumerator playerFadeOut()
    {
        SpriteRenderer spr = playerObject.GetComponent<SpriteRenderer>();
        spr.sortingLayerID = SortingLayer.NameToID("stageStartPlayer");
 
        while (true)
        {
            float newColor = spr.color.r - playerFadeSpeed;
            spr.color = new Color(newColor, newColor, newColor);
            yield return new WaitForFixedUpdate();

            if(spr.color.r <= playerFadeSize) //만약 플레이어가 기준치보다 어두워지면 
            {
                Invoke("elevatorDepart", 2f); //1초 뒤 엘리베이터 출발 
                break;
            }
        }
    }
}
