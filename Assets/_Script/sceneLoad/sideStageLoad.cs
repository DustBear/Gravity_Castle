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
    public Vector2 spawnDir;

    public GameObject rightDoor;
    public GameObject leftDoor;
    [SerializeField] float doorOpenDelay; //���� ������ �� �ɸ��� �ð�
    [SerializeField] float doorShakeSize; //���� �¿�� ��鸱 ���� ���� 

    bool isSensorActive; //�÷��̾ ���� ���� �ִ����� ���� 
    public bool isCorActive; //�ڷ�ƾ �����߿��� ���� ���� x 
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

        //���� �¿�� ��鸲
        for(int index=1; index<=3; index++)
        {
            rightDoor.transform.localPosition += new Vector3(doorShakeSize, 0, 0);
            leftDoor.transform.localPosition += new Vector3(-doorShakeSize, 0, 0);
            yield return new WaitForSeconds(0.08f);
            rightDoor.transform.localPosition += new Vector3(-doorShakeSize, 0, 0);
            leftDoor.transform.localPosition += new Vector3(doorShakeSize, 0, 0);
            yield return new WaitForSeconds(0.08f);
        }

        yield return new WaitForSeconds(1f); //��� ��ٸ� ��

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
        GameManager.instance.gameData.sideStageUnlock[sideStageNum - 1] = true; //�ش� �������� ����
        GameManager.instance.nextPos = spawnPos;
        GameManager.instance.nextScene = sceneNum;
        GameManager.instance.nextGravityDir = spawnDir; //�����ϸ� �Ʒ��� ������ �߷� ����
        GameManager.instance.nextState = Player.States.Walk; //�� ó�� ������ �� �÷��̾��� ���´� walk

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
