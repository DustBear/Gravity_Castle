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

    public GameObject E_icon;

    private void Awake()
    {
        sound = GetComponent<AudioSource>();
        rigid = GetComponent<Rigidbody2D>();
        cameraObj = GameObject.FindWithTag("MainCamera");
    }
    void Start()
    {
        E_icon.SetActive(false);
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
            E_icon.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            isPlayerOn = false;
            E_icon.SetActive(false);
        }
    }

    IEnumerator elevatorMove()
    {
        InputManager.instance.isInputBlocked = true;
        E_icon.SetActive(false);

        sound.volume = GameManager.instance.optionSettingData.masterVolume_setting * GameManager.instance.optionSettingData.effectVolume_setting;
        sound.PlayOneShot(startSound);
        UIManager.instance.cameraShake(0.5f, 0.4f);
        yield return new WaitForSeconds(1.5f);

        sound.volume = GameManager.instance.optionSettingData.masterVolume_setting * GameManager.instance.optionSettingData.effectVolume_setting;
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
        GameManager.instance.nextScene = sceneNum;
        GameManager.instance.nextGravityDir = Physics2D.gravity.normalized; //현재의 중력상태 유지 
        GameManager.instance.nextState = Player.States.Walk;

        if (moveToNextStage)
        {
            GameManager.instance.gameData.curAchievementNum = 0; //다음 스테이지로 가는 엘리베이터일 경우엔 achNum=0으로 초기화해줌 
            GameManager.instance.gameData.curStageNum = stageNum; //스테이지 넘버 역시 초기화                                                                                      
        }

        //씬 엘리베이터 타고 다음 스테이지로 넘어갈 때도 tmpCollection 비우기         
        if (GameManager.instance.gameData.collectionTmp.Count != 0) //만약 세이브해야 할 수집요소가 있다면
        {
            UIManager.instance.collectionAlarm(2);
            //알람 띄우고 

            for (int index = 0; index < GameManager.instance.gameData.collectionTmp.Count; index++)
            {
                int colNum = GameManager.instance.gameData.collectionTmp[index];
                GameManager.instance.gameData.collectionUnlock[colNum] = true; //해당 수집요소 수집 완료 
            }

            GameManager.instance.gameData.collectionTmp.Clear();
        }

        GameManager.instance.gameData.UseOpeningElevetor_bool = true;
        GameManager.instance.gameData.SpawnSavePoint_bool = false;

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
