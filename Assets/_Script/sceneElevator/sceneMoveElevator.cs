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

        rigid.velocity = new Vector2(0, -moveSpeed); //���������� ��ӿ ���� 
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
        GameManager.instance.nextGravityDir = Physics2D.gravity.normalized; //������ �߷»��� ���� 
        GameManager.instance.nextState = Player.States.Walk;

        if (moveToNextStage)
        {
            GameManager.instance.gameData.curAchievementNum = 0; //���� ���������� ���� ������������ ��쿣 achNum=0���� �ʱ�ȭ���� 
            GameManager.instance.gameData.curStageNum = stageNum; //�������� �ѹ� ���� �ʱ�ȭ                                                                                      
        }
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
