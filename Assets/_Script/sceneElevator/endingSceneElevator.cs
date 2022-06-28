using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class endingSceneElevator : MonoBehaviour
{
    public GameObject elevatorDoor; //���������� �� 
    public float elevatorSpeed; //���������� �ö󰡴� �ӵ� 
    public float doorSpeed; //���������� �� �ö󰡴� �ӵ� 

    public float eleStartYPos; //������������ ������ġ 
    public float sceneDelay; //���������� �ö󰡰� ���� ���� ������ �Ѿ�� �� �ɸ��� �ð� 

    public float doorStartYPos; //���������� �� ������ġ 
    public float doorFinishYPos; //���������� �� ������ġ

    public Vector2 nextScenePos; //���� ������ ������ ��ġ ���� 
    public int nextScene; //���� �� ��ȣ 
    public float playerFadeSize; //�÷��̾�� ��� ������ ��ο� �� ������ 
    public float playerFadeSpeed; //�÷��̾� ��ο����� �ӵ� 

    bool isDoorArrived;
    bool isSensorOn; //�÷��̾ ���� ���� ���� ���������� true

    public GameObject[] collGroup = new GameObject[4]; //�÷��̾� ���δ� �ݶ��̴� 
    GameObject playerObject;
    Player player;
    public GameObject interactText;
    void Start()
    {
        playerObject = GameObject.Find("Player");
        player = playerObject.GetComponent<Player>();       
        interactText.SetActive(false);
 
        GetComponent<BoxCollider2D>().enabled = true;
        isDoorArrived = false;
        isSensorOn = false;

        transform.position = new Vector2(transform.position.x, eleStartYPos); //���������� ��ġ �ʱ�ȭ 
        GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0); //���������� �ӵ� �ʱ�ȭ

        elevatorDoor.transform.localPosition = new Vector2(0, doorStartYPos); //���������� �� ��ġ �ʱ�ȭ
        elevatorDoor.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0); //���������� �� �ӵ� �ʱ�ȭ 

        for (int index = 0; index < 4; index++)
        {
            collGroup[index].SetActive(false);
        }
    }

    void Update()
    {
        if(elevatorDoor.transform.localPosition.y >= doorFinishYPos) //���������� ���� �����ϸ� 
        {
            if (!isDoorArrived) //���� 1ȸ ���� 
            {
                elevatorDoor.transform.localPosition = new Vector2(0, doorFinishYPos); //���������� �� ��ġ ����
                elevatorDoor.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0); //���������� �� �ӵ� 0���� 
                StartCoroutine("playerFadeOut");

                isDoorArrived = true;
            }          
        }

        if (Input.GetKeyDown(KeyCode.E) && isSensorOn) //�÷��̾ �� �տ� �� ���¿��� E ������ �۵�
        {
            for (int index = 0; index < 4; index++)
            {
                collGroup[index].SetActive(true); //�÷��̾ ���������� ������ �������� ����              
            }
            doorOpen(); //���������� �� ���� 
            GetComponent<BoxCollider2D>().enabled = false; //���������� �������� ���� ������ ���� �� 
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            interactText.SetActive(true);
            isSensorOn = true;
            //�÷��̾ �� �տ� ���� ��ȣ�ۿ� �ؽ�Ʈ Ȱ��ȭ 
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            interactText.SetActive(false);
            isSensorOn = false;
            //�÷��̾ �� �տ��� ����� ��ȣ�ۿ� �ؽ�Ʈ ��Ȱ��ȭ  
        }
    }

    void doorOpen()
    {
        elevatorDoor.GetComponent<Rigidbody2D>().velocity = new Vector2(0, doorSpeed);
    }

    void elevatorDepart() //���������� ��� 
    {
        GetComponent<Rigidbody2D>().velocity = new Vector2(0, elevatorSpeed);
        elevatorDoor.GetComponent<Rigidbody2D>().velocity = new Vector2(0, -elevatorSpeed); 
        //���������� ���� ���������� ��ü�� child�� ����Ǿ� �����Ƿ� ���� ������ ���·� �α� ���ؼ� �ݴ� �������� �ӵ� ��� �� 

        Invoke("moveToNextScene", sceneDelay); //sceneDelay �� ���� ������ �̵� 
    }

    void moveToNextScene()
    {
        if (!GameManager.instance.shouldStartAtSavePoint) //���̺�����Ʈ���� �����ؾ� �ϴ� ���� �ƴϸ� 
        {
            GameManager.instance.gameData.curAchievementNum = 1; //���� ���������� �̵��ϴϱ� curAchievement Num �ʱ�ȭ������� �� 
            GameManager.instance.nextPos = nextScenePos;
            GameManager.instance.nextGravityDir = Physics2D.gravity.normalized;
            GameManager.instance.nextState = Player.curState;
            SceneManager.LoadScene(nextScene);
        }
    }

    IEnumerator playerFadeOut()
    {
        SpriteRenderer spr = playerObject.GetComponent<SpriteRenderer>();
        spr.sortingLayerID = SortingLayer.NameToID("stageStartPlayer");
 
        while (true)
        {
            float newColor = spr.color.r - playerFadeSpeed;
            spr.color = new Color(newColor, newColor, newColor);
            yield return new WaitForFixedUpdate();

            if(spr.color.r <= playerFadeSize) //���� �÷��̾ ����ġ���� ��ο����� 
            {
                Invoke("elevatorDepart", 2f); //1�� �� ���������� ��� 
                break;
            }
        }
    }
}