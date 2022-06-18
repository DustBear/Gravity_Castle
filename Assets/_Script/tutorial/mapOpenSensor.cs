using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class mapOpenSensor : MonoBehaviour
{
    public GameObject mapCanvas;
    public GameObject map;
    public GameObject chapter_instruction;
    public Button[] stageButton;
    public GameObject iconGroup; //������ �������� �� �ؿ� �־ ���� 

    //��ư ������ Ű���� �� �Ʒ��� ��ũ���ϱ� 
    public Button Button_up;
    public Button Button_down;
    public float mapMaxY;
    public float mapMinY;

    public float mapScrollSpeed; //���� �� �Ʒ��� ��ũ���ϴ� �ӵ�

    [SerializeField]bool isSensorOn;
    [SerializeField]bool shouldMapMoveUp;
    [SerializeField]bool shouldMapMoveDown;
    [SerializeField] bool isScrollButtonDown; //��ũ�� ��ư ������ ���� �� Ű���� �� ���콺 �� �Է� ���� 

    public int selectedStageNum; //���� ���õ� ��������
    public int selectedSavePointNum; //���� ���õ� ���̺�����Ʈ 
    public int savePointCount; //ǥ���ؾ� �ϴ� ���̺�����Ʈ�� ����
    [SerializeField] Vector2 firstIconPos;
    public float iconGap;
    public GameObject stageIcon;
    public GameObject firstIconPosAnchor;
    public float maxIconCount; //��� ���������� ���̺�����Ʈ ���� �� ���� ���� �� �̿�
    [SerializeField] List<GameObject> savePointIcon;

    public int stage0SavePointCount; 
    void Start()
    {
        mapCanvas.SetActive(false); //�����ϸ� ���� ���� ���� 
        shouldMapMoveDown = false;
        shouldMapMoveUp = false;
        selectedStageNum = 0; //�� ó�� ���õ� ���������� 0
        selectedSavePointNum = 0; //�� ó�� ���õ� ���̺�����Ʈ�� 0

        firstIconPos = firstIconPosAnchor.transform.position;      
    }

    void iconInitiate()
    {
        savePointIcon = new List<GameObject>(); //���̺�����Ʈ �迭 �ʱ�ȭ 

        for (int index=0; index<maxIconCount; index++)
        {
            savePointIcon.Add(Instantiate(stageIcon, new Vector3(firstIconPos.x + iconGap * index, firstIconPos.y, 0), Quaternion.identity, iconGroup.transform));
            savePointIcon[index].GetComponent<savePointIconButton>().savePointNum = index; //���̺�����Ʈ ��ȣ�� 1���� ���� 
            //�ִ� ������ ������ŭ ������ ������ �� savePointIcon �׷쿡 �������          
        }
    }

    public void iconMake() //�������� ��ȣ�� �ٲ� �� �ش� ���������� ���̺�����Ʈ ������ �°� �������� ������ Ȱ��ȭ~>stageNum �ٲ� �� ���� ȣ�� 
    {               
        for (int index = 0; index < maxIconCount; index++)
        {
            if (index < savePointCount) //maxIconCount ���� ������ �� �տ������� savePointCount���� �����ܸ� Ȱ��ȭ�ϰ� �������� ��Ȱ��ȭ
            {              
                savePointIcon[index].SetActive(true);
            }
            else
            {
                savePointIcon[index].SetActive(false);
            }
        }
        iconCheck();
    }

    public void iconInputCheck() //Ű���� �Է¿� ���� ���� �����ؾ� �ϴ� ���̺�����Ʈ ��ȣ ����
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (selectedSavePointNum < savePointCount - 1)
            {
                selectedSavePointNum++;
                iconCheck();
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (selectedSavePointNum >= 1)
            {
                selectedSavePointNum--;
                iconCheck();
            }
        }
    }

    public void iconCheck() //���� Ŀ���� ǥ�õǾ� �ִ� ���̺�����Ʈ�� �°� ������ ǥ��~>�������� ��ȣ �ٲ� ��, ���� ������������ ���̺�����Ʈ ��ȣ �ٲ� �� ȣ�� 
    {
        for(int index=0; index<savePointCount; index++)
        {
            if(index == selectedSavePointNum)
            {
                savePointIcon[index].GetComponent<Image>().color = new Color(1, 1, 1, 1);
            }
            else
            {
                savePointIcon[index].GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 1);
            }
            
        }
    }
    
    void Update()
    {
        //���� �۵����
        if(isSensorOn && Input.GetKeyDown(KeyCode.E))
        {
            mapCanvas.SetActive(true);
            InputManager.instance.isInputBlocked = true; //�� �޴� �����ִ� ������ �÷��̾� ������ �� ����

            //�������� ���̺�����Ʈ ������ �ʱ�ȭ �� ������ ���� �� ������ ���� 
            
            iconInitiate();
            iconMake();
            savePointCount = stage0SavePointCount;
        }

        if(isSensorOn && Input.GetKeyDown(KeyCode.Q))
        {
            mapCanvas.SetActive(false);
            InputManager.instance.isInputBlocked = false; 
            Cursor.lockState = CursorLockMode.Locked; //���콺 ���� �Ұ���
        }

        mapMove();
        InputCheck();
        iconInputCheck();

        if (mapCanvas.activeSelf)
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }
   
    void mapMove()
    {
        Vector2 mapPos = map.GetComponent<RectTransform>().position;
        if (shouldMapMoveUp)
        {
            if (mapPos.y <= mapMinY + 540) //�� ��ġ�� ���� ���ؼ����� �������� ����
            {                
                return;
            }
            map.GetComponent<RectTransform>().position = new Vector2(mapPos.x, mapPos.y - mapScrollSpeed * Time.deltaTime);
        }
        if (shouldMapMoveDown)
        {            
            if (mapPos.y >= mapMaxY + 540) //�� ��ġ�� �ְ� ���ؼ����� �������� ����
            {
                return;
            }
            
            map.GetComponent<RectTransform>().position = new Vector2(mapPos.x, mapPos.y + mapScrollSpeed * Time.deltaTime);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            isSensorOn = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            isSensorOn = false;
        }
    }

    //ȭ��� ��ư ������ �۵� 
    public void mapScroll_up() 
    {
        isScrollButtonDown = true;
        shouldMapMoveUp = true;
    }
    public void mapScroll_down()
    {
        isScrollButtonDown = true;
        shouldMapMoveDown = true;
    }
    public void mapScrollPause()
    {
        isScrollButtonDown = false;
        shouldMapMoveUp = false;
        shouldMapMoveDown = false;
    }

    public void InputCheck()
    {
        if (isScrollButtonDown)
        {
            return;
        }

        //���콺 Ŀ���� ���� ���� �ִ��� ����
        float mouseXpos = Input.mousePosition.x;
        float mapMinXpos = map.GetComponent<RectTransform>().position.x - map.GetComponent<RectTransform>().rect.width / 2;
        float mapMaxXpos = map.GetComponent<RectTransform>().position.x + map.GetComponent<RectTransform>().rect.width / 2;

        bool isMouseOnMap = (mapMinXpos < mouseXpos) && (mouseXpos < mapMaxXpos);

        if (Input.GetKey(KeyCode.UpArrow) || (isMouseOnMap && (Input.mouseScrollDelta.y > 0)))
        {
            shouldMapMoveUp = true; //�� ȭ��ǥ�� �����ų� ���� �������� ������ �� �÷���
        }else if (Input.GetKey(KeyCode.DownArrow) || (isMouseOnMap && (Input.mouseScrollDelta.y < 0)))
        {
            shouldMapMoveDown = true; //�Ʒ� ȭ��ǥ�� �����ų� ���� �Ʒ������� ������ �� ������ 
        }else
        {
            shouldMapMoveUp = false;
            shouldMapMoveDown = false;
        }
    }

    public void StageStart() //test()�� ������ ���� ��. GameData ���� ��� ����
    {
        GameObject stageButtonTmp = stageButton[selectedStageNum].gameObject;
        stageMoveButton tmpScript = stageButtonTmp.GetComponent<stageMoveButton>();

        GameManager.instance.gameData.curAchievementNum = selectedSavePointNum+1; //���õ� ���̺�����Ʈ ���ڰ� ���� achievement Num �� ��ȣ(achievement Num �� 1���� ����)
        GameManager.instance.gameData.curStageNum = selectedStageNum;
        GameManager.instance.nextScene = tmpScript.savePointScene[selectedSavePointNum]; //���õ� ������ �̵�
        GameManager.instance.nextPos = tmpScript.savePointPos[selectedSavePointNum]; //���õ� savePoint ��ġ�� �̵� 
        GameManager.instance.nextGravityDir = tmpScript.savePointGravityDir[selectedSavePointNum]; //���õ� savePoint �߷¹��� ����
        GameManager.instance.isCliffChecked = false;

        for (int i = 0; i < 35; i++)
        {
            GameManager.instance.curIsShaked[i] = false;
            GameManager.instance.gameData.storedIsShaked[i] = false;
        }

        GameManager.instance.shouldStartAtSavePoint = true;
        GameManager.instance.nextState = Player.States.Walk;
        GameManager.instance.gameData.respawnScene = GameManager.instance.nextScene;
        GameManager.instance.gameData.respawnPos = GameManager.instance.nextPos;
        GameManager.instance.gameData.respawnGravityDir = GameManager.instance.nextGravityDir;

        Cursor.lockState = CursorLockMode.Locked;
        InputManager.instance.isInputBlocked = false;
        SceneManager.LoadScene(GameManager.instance.nextScene);
    }   
}