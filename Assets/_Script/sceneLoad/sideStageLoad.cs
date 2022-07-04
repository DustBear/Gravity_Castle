using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class sideStageLoad : MonoBehaviour
{
    GameObject player;
    Player playerScript;

    public int sideStageNum; //1번부터 24번까지 
    public int sceneNum; //이 사이드 스테이지의 scene 번호 
    public Vector2 spawnPos; //이 사이드 스테이지에 스폰될 때, 사이드 스테이지 안에서 죽었을 때 스폰될 위치 
    public Vector2 spawnDir;

    public GameObject rightDoor;
    public GameObject leftDoor;
    [SerializeField] float doorOpenDelay; //문이 열리는 데 걸리는 시간
    [SerializeField] float doorShakeSize; //문이 좌우로 흔들릴 때의 진폭 

    bool isSensorActive; //플레이어가 센서 내에 있는지의 여부 
    public bool isCorActive; //코루틴 실행중에는 새로 실행 x 
    private void Awake()
    {
        player = GameObject.Find("Player");
        playerScript = player.GetComponent<Player>();
    }
    void Start()
    {
        isCorActive = false;
    }

    void Update()
    {
        if(isSensorActive && Input.GetKeyDown(KeyCode.E))
        {
            if (isCorActive) return;
            StartCoroutine("doorOpen");
        }
    }

    IEnumerator doorOpen()
    {
        isCorActive = true;

        //문이 좌우로 흔들림
        for(int index=1; index<=3; index++)
        {
            rightDoor.transform.localPosition += new Vector3(doorShakeSize, 0, 0);
            leftDoor.transform.localPosition += new Vector3(-doorShakeSize, 0, 0);
            yield return new WaitForSeconds(0.08f);
            rightDoor.transform.localPosition += new Vector3(-doorShakeSize, 0, 0);
            leftDoor.transform.localPosition += new Vector3(doorShakeSize, 0, 0);
            yield return new WaitForSeconds(0.08f);
        }

        yield return new WaitForSeconds(1f); //잠시 기다린 뒤

        for(int index=1; index<=50; index++)
        {
            rightDoor.transform.localPosition += new Vector3(0.03f, 0, 0);
            leftDoor.transform.localPosition += new Vector3(-0.03f, 0, 0);
            yield return new WaitForSeconds(doorOpenDelay / 50);
        }

        yield return new WaitForSeconds(0.5f);

        UIManager.instance.FadeOut(1f);
        yield return new WaitForSeconds(1f);
        isCorActive = false;

        loadSideStage();
    }

    void loadSideStage()
    {
        playerScript.isPlayerInSideStage = true;
        GameManager.instance.gameData.sideStageUnlock[sideStageNum - 1] = true; //해당 스테이지 개방
        GameManager.instance.nextPos = spawnPos;
        GameManager.instance.nextScene = sceneNum;
        GameManager.instance.nextGravityDir = spawnDir; //시작하면 아래쪽 보도록 중력 적용
        GameManager.instance.nextState = Player.States.Walk; //맨 처음 시작할 때 플레이어의 상태는 walk

        GameManager.instance.gameData.respawnScene = sceneNum;
        GameManager.instance.gameData.respawnPos = spawnPos;
        GameManager.instance.gameData.respawnGravityDir = spawnDir;

        SceneManager.LoadScene(GameManager.instance.nextScene);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            isSensorActive = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            isSensorActive = false;
        }
    }
}
