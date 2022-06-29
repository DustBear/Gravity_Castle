using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class mainStageLoad : MonoBehaviour
{
    GameObject player;
    Player playerScript;

    public int sceneNum; //돌아갈 메인 스테이지의 씬 번호 
    public Vector2 spawnPos; //돌아갈 때 스폰될 위치 
    public Vector2 spawnDir; //돌아갈 때 스폰될 중력 방향

    public int respawnScene;
    public Vector2 respawnPos;
    public Vector2 respawnDir;
    //메인 스테이지로 돌아간 후 다음 세이브포인트에 닿기 전 죽었을 때 스폰될 위치 
    //이 코드가 없으면 메인 스테이지에서 죽어도 다시 사이드 스테이지의 스폰포인트로 돌아오게 된다 
    //사이드 스테이지 문 바로 앞으로 설정하기보다는 사이드스테이지와 메인스테이지 경계부의 적당한 위치에 설정한다
    //메인 스테이지로 돌아갈 때는 사이드 스테이지 문 바로 앞에 스폰되므로, 여기서 respawnPos까지 가는 동안은 죽을 수 없도록 맵을 배치한다 

    bool isSensorActive; //플레이어가 센서 내에 있는지의 여부 
    void Start()
    {
        player = GameObject.Find("Player");
        playerScript = player.GetComponent<Player>();
    }

    
    void Update()
    {
        if (isSensorActive && Input.GetKeyDown(KeyCode.E))
        {
            playerScript.isPlayerInSideStage = false;

            GameManager.instance.nextPos = spawnPos;
            GameManager.instance.nextScene = sceneNum;
            GameManager.instance.nextGravityDir = spawnDir; //시작하면 아래쪽 보도록 중력 적용
            GameManager.instance.nextState = Player.States.Walk; //맨 처음 시작할 때 플레이어의 상태는 walk

            GameManager.instance.gameData.respawnScene = respawnScene;
            GameManager.instance.gameData.respawnPos = respawnPos;
            GameManager.instance.gameData.respawnGravityDir = respawnDir;

            SceneManager.LoadScene(GameManager.instance.nextScene);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
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
