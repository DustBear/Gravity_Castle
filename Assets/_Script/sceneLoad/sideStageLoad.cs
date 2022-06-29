using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class sideStageLoad : MonoBehaviour
{
    GameObject player;
    Player playerScript;

    public int sideStageNum; //1������ 24������ 
    public int sceneNum; //�� ���̵� ���������� scene ��ȣ 
    public Vector2 spawnPos; //�� ���̵� ���������� ������ ��, ���̵� �������� �ȿ��� �׾��� �� ������ ��ġ 

    bool isSensorActive; //�÷��̾ ���� ���� �ִ����� ���� 
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
            GameManager.instance.gameData.sideStageUnlock[sideStageNum - 1] = true; //�ش� �������� ����
            GameManager.instance.nextPos = spawnPos;
            GameManager.instance.nextScene = sceneNum;
            GameManager.instance.nextGravityDir = Vector2.down; //�����ϸ� �Ʒ��� ������ �߷� ����
            GameManager.instance.nextState = Player.States.Walk; //�� ó�� ������ �� �÷��̾��� ���´� walk

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
