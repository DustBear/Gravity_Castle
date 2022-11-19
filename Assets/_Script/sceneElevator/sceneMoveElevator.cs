using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class sceneMoveElevator : MonoBehaviour
{
    public int sceneNum; //현재 엘리베이터가 이동하고자 하는 씬 넘버
    public int stageNum; //현재 엘리베이터가 이동하고자 하는 스테이지 번호 
    public bool moveToNextStage; //현재 엘리베이터가 스테이지 자체를 바꾸는지의 여부 ex) stage3 ~> stage4

    public Vector2 startPos; //엘리베이터가 처음 세팅되어 있는 높이 
    public float moveTime; //엘리베이터가 내려가는 시간 
    public float moveSpeed; //엘리베이터의 하강 속도 

    bool isPlayerOn;    

    Rigidbody2D rigid;
    GameObject cameraObj;

    public GameObject elevatorLever;

    AudioSource sound;

    public AudioClip startSound;
    public AudioClip moveSound;

    private void Awake()
    {
        sound = GetComponent<AudioSource>();
        rigid = GetComponent<Rigidbody2D>();
        cameraObj = GameObject.FindWithTag("MainCamera");
    }
    void Start()
    {
        
    }

    void Update()
    {
        if(isPlayerOn && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(elevatorLeverAct());
            StartCoroutine(elevatorMove());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player" && collision.transform.up == collision.transform.up)
        {
            isPlayerOn = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            isPlayerOn = false;
        }
    }

    IEnumerator elevatorMove()
    {
        sound.PlayOneShot(startSound);
        UIManager.instance.cameraShake(0.5f, 0.4f);
        yield return new WaitForSeconds(1.5f);

        sound.clip = moveSound;
        sound.Play();

        rigid.velocity = new Vector2(0, -moveSpeed); //엘리베이터 등속운동 시작 
        yield return new WaitForSeconds(moveTime-2f);

        UIManager.instance.FadeOut(1.5f);

        yield return new WaitForSeconds(2f);
        moveToNextScene();
    }

    void moveToNextScene()
    {
        GameManager.instance.shouldUseOpeningElevator = true;
        GameManager.instance.shouldSpawnSavePoint = false;

        GameManager.instance.nextScene = sceneNum;
        GameManager.instance.nextGravityDir = Physics2D.gravity.normalized; //현재의 중력상태 유지 
        GameManager.instance.nextState = Player.States.Walk;

        if (moveToNextStage)
        {
            GameManager.instance.gameData.curAchievementNum = 0; //다음 스테이지로 가는 엘리베이터일 경우엔 achNum=0으로 초기화해줌 
            GameManager.instance.gameData.curStageNum = stageNum; //스테이지 넘버 역시 초기화                                                                                      
        }
        // gameData class의 데이터들을 모두 Json 파일에 저장
        string ToJsonData = JsonUtility.ToJson(GameManager.instance.gameData);
        string filePath = Application.persistentDataPath + GameManager.instance.gameDataFileNames[GameManager.instance.curSaveFileNum];
        File.WriteAllText(filePath, ToJsonData);

        SceneManager.LoadScene(sceneNum);
    }

    IEnumerator elevatorLeverAct()
    {
        elevatorLever.transform.rotation = Quaternion.Euler(0, 0, 20f);
        yield return new WaitForSeconds(0.1f);
        elevatorLever.transform.rotation = Quaternion.Euler(0, 0, -15f);
        yield return new WaitForSeconds(0.1f);
        elevatorLever.transform.rotation = Quaternion.Euler(0, 0, 0f);
    }
}
