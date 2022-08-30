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
    public AnimationCurve velCurve; //�ʹ� ���������� �ϰ��� ���� ���ӵ� � 
    public float accelPeriod; //���ӵ� ��� �̿��ϴ� �ð� 

    Rigidbody2D rigid;
    GameObject cameraObj;

    private void Awake()
    {
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
            StartCoroutine(elevatorMove());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
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
        for(int index=1; index<=200; index++) //���������� ��� 
        {
            float moveVel = moveSpeed * velCurve.Evaluate(index * accelPeriod / 200);
            rigid.velocity = new Vector2(0, -moveVel);

            yield return new WaitForSeconds(accelPeriod / 200);
        }

        rigid.velocity = new Vector2(0, -moveSpeed); //���������� ��ӿ ���� 
        yield return new WaitForSeconds(moveTime - accelPeriod);

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
}
