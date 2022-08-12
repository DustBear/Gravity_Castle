using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class mapOpenSensor : MonoBehaviour
{
    public GameObject mapCanvas;
    public GameObject map;
    public GameObject chapter_instruction;
    public Button[] stageButton;
    public GameObject iconGroup; //������ �������� �� �ؿ� �־ ���� 

    public Sprite okIcon; //���� ���൵�� �����̵� ������ ���̺�����Ʈ ������
    public Sprite noIcon; //���� ���൵�� �����̵� �Ұ����� ���̺�����Ʈ ������

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

    public int selectedStageNum; //���� ���õ� ��������: 1���� ���� 
    public int selectedSavePointNum; //���� ���õ� ���̺�����Ʈ: 1���� ���� 
    public int savePointCount; //ǥ���ؾ� �ϴ� ���̺�����Ʈ�� ����
    [SerializeField] Vector2 firstIconPos;
    public float iconGap;
    public GameObject stageIcon;
    public GameObject firstIconPosAnchor;
    public float maxIconCount; //��� ���������� ���̺�����Ʈ ���� �� ���� ���� �� �̿�
    [SerializeField] List<GameObject> savePointIcon;

    public GameObject betaModeWindow;

    public int stage0SavePointCount; 
    void Start()
    {
        mapCanvas.SetActive(false); //�����ϸ� ���� ���� ���� 
        shouldMapMoveDown = false;
        shouldMapMoveUp = false;
        selectedStageNum = 1; //�� ó�� ���õ� ���������� 1
        selectedSavePointNum = 1; //�� ó�� ���õ� ���̺�����Ʈ�� 1

        firstIconPos = firstIconPosAnchor.transform.position;
        betaModeWindow.SetActive(false);
    }

    void iconInitiate()
    {
        savePointIcon = new List<GameObject>(); //���̺�����Ʈ �迭 �ʱ�ȭ 

        for (int index=0; index<maxIconCount; index++)
        {
            savePointIcon.Add(Instantiate(stageIcon, new Vector3(firstIconPos.x + iconGap * index, firstIconPos.y, 0), Quaternion.identity, iconGroup.transform));
            savePointIcon[index].GetComponent<savePointIconButton>().savePointNum = index+1; //���̺�����Ʈ ��ȣ�� 1���� ���� 

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
                savePointIcon[index].GetComponent<Image>().sprite = okIcon;

                if (GameManager.instance.gameData.finalStageNum == selectedStageNum)
                {
                    if(!GameManager.instance.gameData.savePointUnlock[selectedStageNum-1,selectedSavePointNum]) //�ش� ���̺�����Ʈ�� ���� Ȱ��ȭ���� �ʾ����� 
                    {
                        savePointIcon[index].GetComponent<Image>().sprite = noIcon;
                    }                   
                }
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
            if (selectedSavePointNum < savePointCount)
            {
                selectedSavePointNum++;
                iconCheck();
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (selectedSavePointNum > 1)
            {
                selectedSavePointNum--;
                iconCheck();
            }
        }
    }

    public void iconCheck() //���� Ŀ���� ǥ�õǾ� �ִ� ���̺�����Ʈ�� �°� ������ ���� ǥ��~>�������� ��ȣ �ٲ� ��, ���� ������������ ���̺�����Ʈ ��ȣ �ٲ� �� ȣ�� 
    {
        for(int index=1; index<=savePointCount; index++)
        {
            if(index == selectedSavePointNum)
            {
                savePointIcon[index-1].GetComponent<Image>().color = new Color(1, 1, 1, 1);
            }
            else
            {
                savePointIcon[index-1].GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 1);
            }
            
        }
    }
    
    void Update()
    {
        //���� �۵����
        if(isSensorOn && Input.GetKeyDown(KeyCode.E))
        {
            mapCanvas.SetActive(true);

            if (GameManager.instance.gameData.mapStageNum < GameManager.instance.gameData.finalStageNum)
            {
                GameManager.instance.gameData.mapStageNum = GameManager.instance.gameData.finalStageNum;
                Debug.Log("���� ����");
            }
            
            InputManager.instance.isInputBlocked = true; //�� �޴� �����ִ� ������ �÷��̾� ������ �� ����

            //�������� ���̺�����Ʈ ������ �ʱ�ȭ �� ������ ���� �� ������ ���� 
            
            iconInitiate();
            iconMake();
            savePointCount = stage0SavePointCount;
        }

        if(isSensorOn && Input.GetKeyDown(KeyCode.Q))
        {
            if (betaModeWindow.activeSelf == true)
            {
                betaModeWindow.SetActive(false);
            }
            else
            {
                mapCanvas.SetActive(false);
                InputManager.instance.isInputBlocked = false;
                Cursor.lockState = CursorLockMode.Locked; //���콺 ���� �Ұ���
            }          
        }

        mapMove();
        InputCheck();
        iconInputCheck();

        if (mapCanvas.activeSelf)
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }
    public void ChapterStart() //é�� �����ϱ� ��ư�� ������ ����Ǵ� �Լ� 
    {
        UIManager.instance.clickSoundGen();

        GameObject stageButtonTmp = stageButton[selectedStageNum-1].gameObject;
        stageMoveButton tmpScript = stageButtonTmp.GetComponent<stageMoveButton>();

        if (GameManager.instance.gameData.finalStageNum == selectedStageNum)
        {
            //���� �̵��ϰ��� �ϴ� ���̺�����Ʈ ���ڰ� ���뵵���� ������ �̵�x 
            //���뵵���� ���� ���������� ���ʿ� Ŭ���� �Ұ����ϹǷ� ���� ����� �ʿ�x
            if (selectedSavePointNum > GameManager.instance.gameData.finalAchievementNum)
            {
                Debug.Log("not enough achievement");
                return;
            }
        }

        Debug.Log("savePointBackUp: " + (selectedSavePointNum));

        GameManager.instance.gameData.curAchievementNum = selectedSavePointNum; 
        GameManager.instance.gameData.curStageNum = selectedStageNum;

        /*
        if (GameManager.instance.gameData.finalStageNum < selectedStageNum)
        {
            GameManager.instance.gameData.finalStageNum = selectedStageNum; //��� ���̺� �� ���������� finalStage���� �ռ� ������ finalStage ����
            GameManager.instance.gameData.finalAchievementNum = selectedSavePointNum; //���������� ���ŵǾ����� achievement Num�� ������ �����ؾ� �� 
        }
        else if (GameManager.instance.gameData.finalStageNum == selectedStageNum)
        {
            if (GameManager.instance.gameData.finalAchievementNum < selectedSavePointNum)
            {
                GameManager.instance.gameData.finalAchievementNum = selectedSavePointNum;
                //���� ������������ �� ū achNum���� �̵��� �� ������ �� ex) (1,10) ~> (1,11)
                //���� ���������� �� �۴ٸ� finalAch���� ū achNum�� ���͵� ���� ex) (2,10) ~> (1,13)���� �̵��� ���� finalAchNum ����x 
            }
        }
        */

        GameManager.instance.nextScene = tmpScript.savePointScene[selectedSavePointNum-1]; //���õ� ������ �̵�
        GameManager.instance.nextPos = new Vector2(0, 0);
        GameManager.instance.nextGravityDir = new Vector2(0, -1);
        //������ �ش� ������ �̵��� �� �ٽ� �������� �� �״� ���� �Ű澵 �ʿ�x


        GameManager.instance.shouldStartAtSavePoint = true;
        GameManager.instance.nextState = Player.States.Walk;

        //gameData �ʱ�ȭ
        GameManager.instance.gameData.respawnScene = GameManager.instance.nextScene;
        GameManager.instance.gameData.respawnPos = GameManager.instance.nextPos;
        GameManager.instance.gameData.respawnGravityDir = GameManager.instance.nextGravityDir;

        Cursor.lockState = CursorLockMode.Locked;
        InputManager.instance.isInputBlocked = false;

        string ToJsonData = JsonUtility.ToJson(GameManager.instance.gameData);
        string filePath = Application.persistentDataPath + GameManager.instance.gameDataFileNames[GameManager.instance.curSaveFileNum];
        File.WriteAllText(filePath, ToJsonData);

        SceneManager.LoadScene(GameManager.instance.nextScene);
    }

    public void betaModeActive() //���൵�� �ִ�ġ�� �����ִ� �ý���
    {
        Debug.Log("beta-mode activated");

        GameManager.instance.gameData.finalAchievementNum = 13;
        GameManager.instance.gameData.finalStageNum = 4;

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 50; j++)
            {
                GameManager.instance.gameData.savePointUnlock[i, j] = true; //��� ���̺�����Ʈ Ȱ��ȭ 
            }
        }
        string ToJsonData = JsonUtility.ToJson(GameManager.instance.gameData);
        string filePath = Application.persistentDataPath + GameManager.instance.gameDataFileNames[GameManager.instance.curSaveFileNum];
        File.WriteAllText(filePath, ToJsonData);

        betaModeWindow.SetActive(false);
    }

    public void betaModeWindowOpen()
    {
        betaModeWindow.SetActive(true);
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

        //bool isMouseOnMap = (mapMinXpos < mouseXpos) && (mouseXpos < mapMaxXpos);

        if (Input.GetKey(KeyCode.UpArrow))
        {
            shouldMapMoveUp = true; //�� ȭ��ǥ�� �����ų� ���� �������� ������ �� �÷���
        }else if (Input.GetKey(KeyCode.DownArrow))
        {
            shouldMapMoveDown = true; //�Ʒ� ȭ��ǥ�� �����ų� ���� �Ʒ������� ������ �� ������ 
        }else
        {
            shouldMapMoveUp = false;
            shouldMapMoveDown = false;
        }
    }
    

    
}