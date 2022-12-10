using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class sceneMoveElevator : MonoBehaviour
{
    public int sceneNum; //���� ���������Ͱ� �̵��ϰ��� �ϴ� �� �ѹ�
    public int stageNum; //���� ���������Ͱ� �̵��ϰ��� �ϴ� �������� ��ȣ 
    public bool moveToNextStage; //���� ���������Ͱ� �������� ��ü�� �ٲٴ����� ���� ex) stage3 ~> stage4

    public Vector2 startPos; //���������Ͱ� ó�� ���õǾ� �ִ� ���� 
    public float moveTime; //���������Ͱ� �������� �ð� 
    public float moveSpeed; //������������ �ϰ� �ӵ� 

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

        rigid.velocity = new Vector2(0, -moveSpeed); //���������� ��ӿ ���� 
        yield return new WaitForSeconds(moveTime-2f);

        UIManager.instance.FadeOut(1.5f);

        yield return new WaitForSeconds(2f);
        moveToNextScene();
    }

    void moveToNextScene()
    {       
        GameManager.instance.nextScene = sceneNum;
        GameManager.instance.nextGravityDir = Physics2D.gravity.normalized; //������ �߷»��� ���� 
        GameManager.instance.nextState = Player.States.Walk;

        if (moveToNextStage)
        {
            GameManager.instance.gameData.curAchievementNum = 0; //���� ���������� ���� ������������ ��쿣 achNum=0���� �ʱ�ȭ���� 
            GameManager.instance.gameData.curStageNum = stageNum; //�������� �ѹ� ���� �ʱ�ȭ                                                                                      
        }

        //�� ���������� Ÿ�� ���� ���������� �Ѿ ���� tmpCollection ����         
        if (GameManager.instance.gameData.collectionTmp.Count != 0) //���� ���̺��ؾ� �� ������Ұ� �ִٸ�
        {
            UIManager.instance.collectionAlarm(2);
            //�˶� ���� 

            for (int index = 0; index < GameManager.instance.gameData.collectionTmp.Count; index++)
            {
                int colNum = GameManager.instance.gameData.collectionTmp[index];
                GameManager.instance.gameData.collectionUnlock[colNum] = true; //�ش� ������� ���� �Ϸ� 
            }

            GameManager.instance.gameData.collectionTmp.Clear();
        }

        GameManager.instance.gameData.UseOpeningElevetor_bool = true;
        GameManager.instance.gameData.SpawnSavePoint_bool = false;

        // gameData class�� �����͵��� ��� Json ���Ͽ� ����
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
