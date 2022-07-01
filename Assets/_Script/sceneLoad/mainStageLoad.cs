using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class mainStageLoad : MonoBehaviour
{
    GameObject player;
    Player playerScript;

    public int sceneNum; //���ư� ���� ���������� �� ��ȣ 
    public Vector2 spawnPos; //���ư� �� ������ ��ġ 
    public Vector2 spawnDir; //���ư� �� ������ �߷� ����

    public int respawnScene;
    public Vector2 respawnPos;
    public Vector2 respawnDir;
    //���� ���������� ���ư� �� ���� ���̺�����Ʈ�� ��� �� �׾��� �� ������ ��ġ 
    //�� �ڵ尡 ������ ���� ������������ �׾ �ٽ� ���̵� ���������� ��������Ʈ�� ���ƿ��� �ȴ� 
    //���̵� �������� �� �ٷ� ������ �����ϱ⺸�ٴ� ���̵彺�������� ���ν������� ������ ������ ��ġ�� �����Ѵ�
    //���� ���������� ���ư� ���� ���̵� �������� �� �ٷ� �տ� �����ǹǷ�, ���⼭ respawnPos���� ���� ������ ���� �� ������ ���� ��ġ�Ѵ� 

    public GameObject rightDoor;
    public GameObject leftDoor;
    [SerializeField] float doorOpenDelay; //���� ������ �� �ɸ��� �ð�
    [SerializeField] float doorShakeSize; //���� �¿�� ��鸱 ���� ���� 

    bool isSensorActive; //�÷��̾ ���� ���� �ִ����� ���� 
    void Start()
    {
        player = GameObject.Find("Player");
        playerScript = player.GetComponent<Player>();
    }

    
    void Update()
    {
        if (isSensorActive && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine("doorOpen");
        }
    }

    IEnumerator doorOpen()
    {
        //���� �¿�� ��鸲
        for (int index = 1; index <= 3; index++)
        {
            rightDoor.transform.localPosition += new Vector3(doorShakeSize, 0, 0);
            leftDoor.transform.localPosition += new Vector3(-doorShakeSize, 0, 0);
            yield return new WaitForSeconds(0.08f);
            rightDoor.transform.localPosition += new Vector3(-doorShakeSize, 0, 0);
            leftDoor.transform.localPosition += new Vector3(doorShakeSize, 0, 0);
            yield return new WaitForSeconds(0.08f);
        }

        yield return new WaitForSeconds(1f); //��� ��ٸ� ��

        for (int index = 1; index <= 50; index++)
        {
            rightDoor.transform.localPosition += new Vector3(0.03f, 0, 0);
            leftDoor.transform.localPosition += new Vector3(-0.03f, 0, 0);
            yield return new WaitForSeconds(doorOpenDelay / 50);
        }

        yield return new WaitForSeconds(1f);
        loadMainStage();
    }

    void loadMainStage()
    {
        playerScript.isPlayerInSideStage = false;

        GameManager.instance.nextPos = spawnPos;
        GameManager.instance.nextScene = sceneNum;
        GameManager.instance.nextGravityDir = spawnDir; //�����ϸ� �Ʒ��� ������ �߷� ����
        GameManager.instance.nextState = Player.States.Walk; //�� ó�� ������ �� �÷��̾��� ���´� walk

        GameManager.instance.gameData.respawnScene = respawnScene;
        GameManager.instance.gameData.respawnPos = respawnPos;
        GameManager.instance.gameData.respawnGravityDir = respawnDir;

        SceneManager.LoadScene(GameManager.instance.nextScene);
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
