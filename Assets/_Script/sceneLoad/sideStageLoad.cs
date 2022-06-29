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

    bool isSensorActive; //플레이어가 센서 내에 있는지의 여부 
    private void Awake()
    {
        player = GameObject.Find("Player");
        playerScript = player.GetComponent<Player>();
    }
    void Start()
    {
        
    }

    void Update()
    {
        if(isSensorActive && Input.GetKeyDown(KeyCode.E))
        {
            playerScript.isPlayerInSideStage = true;
            GameManager.instance.gameData.sideStageUnlock[sideStageNum - 1] = true; //해당 스테이지 개방
            GameManager.instance.nextPos = spawnPos;
            GameManager.instance.nextScene = sceneNum;
            GameManager.instance.nextGravityDir = Vector2.down; //시작하면 아래쪽 보도록 중력 적용
            GameManager.instance.nextState = Player.States.Walk; //맨 처음 시작할 때 플레이어의 상태는 walk

            GameManager.instance.gameData.respawnScene = sceneNum;
            GameManager.instance.gameData.respawnPos = spawnPos;

            SceneManager.LoadScene(GameManager.instance.nextScene);
        }
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
