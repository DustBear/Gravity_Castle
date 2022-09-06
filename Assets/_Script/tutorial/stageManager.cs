using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class stageManager : MonoBehaviour
{    
    public Button[] stageButton;
    public GameObject iconGroup; //������ �������� �� �ؿ� �־ ���� 
    public GameObject betaModeWindow;

    public int selectedStageNum; //���� ���õ� ��������: 1���� ���� 
    public int selectedSavePointNum; //���� ���õ� ���̺�����Ʈ: 1���� ���� 
    public int savePointCount; //ǥ���ؾ� �ϴ� ���̺�����Ʈ�� ����

    Vector2 firstIconPos;
    public float iconGap;
    public GameObject stageIcon;
    public GameObject firstIconPosAnchor;
    public Sprite okIcon; //���� ���൵�� �����̵� ������ ���̺�����Ʈ ������
    public Sprite noIcon; //���� ���൵�� �����̵� �Ұ����� ���̺�����Ʈ ������
    public float maxIconCount; //��� ���������� ���̺�����Ʈ ���� �� ���� ���� �� �̿�
    [SerializeField] List<GameObject> savePointIcon;

    //public GameObject betaModeWindow;

    void Start()
    {       
        selectedStageNum = GameManager.instance.gameData.curStageNum; //�������� �÷����ϴ� ���������� �⺻���� ���õ� 
        selectedSavePointNum = GameManager.instance.gameData.curAchievementNum; //�������� �÷����ϴ� ���̺�����Ʈ�� �⺻���� ���õ� 

        firstIconPos = firstIconPosAnchor.transform.position;

        iconInitiate();
        iconMake();
       
        //�������� ���̺�����Ʈ ������ �ʱ�ȭ �� ������ ���� �� ������ ���� 
    }

    void iconInitiate()
    {
        savePointIcon = new List<GameObject>(); //���̺�����Ʈ �迭 �ʱ�ȭ 

        for (int index=0; index<maxIconCount; index++)
        {
            savePointIcon.Add(Instantiate(stageIcon, new Vector3(firstIconPos.x + iconGap * index, firstIconPos.y, 0), Quaternion.identity, iconGroup.transform));
            savePointIcon[index].GetComponent<savePointIconButton>().savePointNum = index+1; //���̺�����Ʈ ��ȣ�� 1���� ���� 

            //�ִ� ������ ������ŭ ������ ������ �� iconGroup �׷쿡 �������          
        }
    }

    public void iconMake() //�������� ��ȣ�� �ٲ� �� �ش� ���������� ���̺�����Ʈ ������ �°� �������� ������ Ȱ��ȭ~>�� ó�� ������ ��, stageNum �ٲ� �� ���� ȣ�� 
    {
        for (int index = 0; index < maxIconCount; index++)
        {
            if (index < savePointCount) //maxIconCount ���� ������ �� �տ������� savePointCount���� �����ܸ� Ȱ��ȭ�ϰ� �������� ��Ȱ��ȭ
            {              
                //�ϴ� ���� Ȱ��ȭ 
                savePointIcon[index].SetActive(true);
                savePointIcon[index].GetComponent<Image>().sprite = okIcon;

                string filePath = Application.persistentDataPath + GameManager.instance.gameDataFileNames[GameManager.instance.curSaveFileNum];
                string FromJsonData = File.ReadAllText(filePath);
                GameData curGameData = JsonUtility.FromJson<GameData>(FromJsonData);
                
                if (!curGameData.savePointUnlock[selectedStageNum - 1, selectedSavePointNum-1]) //�ش� ���̺�����Ʈ�� ���� Ȱ��ȭ���� �ʾ����� 
                {
                    savePointIcon[index].GetComponent<Image>().sprite = noIcon;
                }
            }
            else
            {
                savePointIcon[index].SetActive(false); //���� ����ϰ� ���� �������� ��Ȱ��ȭ 
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

    public void backToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
    
    void Update()
    {       
        iconInputCheck(); //Ű���� �¿� ȭ��ǥ ������ ���� ���õ� ���̺�����Ʈ ��ȣ �̵� 
        if(betaModeWindow.activeSelf && Input.GetKeyDown(KeyCode.Q))
        {
            betaModeWindow.SetActive(false);
        }
    }
    public void ChapterStart() //é�� �����ϱ� ��ư�� ������ ����Ǵ� �Լ� 
    {
        UIManager.instance.clickSoundGen();

        GameObject stageButtonTmp = stageButton[selectedStageNum-1].gameObject;
        stageMoveButton tmpScript = stageButtonTmp.GetComponent<stageMoveButton>();

        if (GameManager.instance.gameData.finalStageNum == selectedStageNum)
        {
            //선택한 스테이지의 세이브포인트 넘버가 현재 성취도보다 낮으면 이동 불가능 
            //만약 현재 스테이지가 finalStage보다 낮으면 애초에 클릭이 불가능하기에 고려 x 
            if (!GameManager.instance.gameData.savePointUnlock[selectedStageNum-1, selectedSavePointNum-1])
            {
                //Debug.Log("savePoint not activated");
                return;
            }
        }

        //Debug.Log("savePointBackUp: " + (selectedSavePointNum));

        GameManager.instance.gameData.curAchievementNum = selectedSavePointNum; 
        GameManager.instance.gameData.curStageNum = selectedStageNum;

        GameManager.instance.nextScene = tmpScript.savePointScene[selectedSavePointNum-1]; //���õ� ������ �̵�
        GameManager.instance.nextPos = new Vector2(0, 0);
        GameManager.instance.nextGravityDir = new Vector2(0, -1);
        //nextPos와 nextGravityDIr은 어차피 다음 씬에 도착하면 savePointLoad 가 알아서 조정해 줌 

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

        GameManager.instance.shouldSpawnSavePoint = true; //�����̵��� ������ ���̺�����Ʈ�θ� �̵�
        SceneManager.LoadScene(GameManager.instance.nextScene);
    }

    
    public void betaModeActive() //���൵�� �ִ�ġ�� ����ִ� �ý���
    {
        //Debug.Log("beta-mode activated");

        GameManager.instance.gameData.finalAchievementNum = stageButton[2].GetComponent<stageMoveButton>().savePointCount;
        GameManager.instance.gameData.finalStageNum = 3;

        for (int i = 0; i < 7; i++)
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
   
}