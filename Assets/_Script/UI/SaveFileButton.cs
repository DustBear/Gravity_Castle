using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using UnityEngine.SceneManagement;

public class SaveFileButton : MonoBehaviour
{
    [SerializeField] int saveFileNum; //�� ��ư�� ���̺����� ��ȣ
    TextMeshProUGUI text;
    bool isSaveFileExist;

    public GameObject betaModeWindow; //��Ÿ�׽�Ʈ �������� �ٲٽðڽ��ϱ�? â ��� 
    public GameObject saveDeleteWindow;
    public GameObject gameStartButton;

    public GameObject saveDeleteButton;

    void Awake()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
        gameStartButton = GameObject.Find("NewGameButton");
    }

    void Start()
    {      
        KeyValuePair<int, int> keyVal = GameManager.instance.GetSavedData(saveFileNum);
        if (keyVal.Key != -1) 
        {
            text.text = "Save" + (saveFileNum + 1) + " : ��������" + keyVal.Key + "_" + keyVal.Value;
            isSaveFileExist = true;
        }
        else
        {
            text.text = "�� ����";
            isSaveFileExist = false;
        }

        saveDeleteWindow.SetActive(false);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q) && saveDeleteWindow.activeSelf == true)
        {
            saveDeleteButton.SetActive(false);
            saveDeleteWindow.SetActive(false);
        }
    }

    public void OnClickButton()
    {
        UIManager.instance.clickSoundGen();

        GameManager.instance.curSaveFileNum = saveFileNum; //���� �÷������� ���̺����� = �� ���̺����� �ε� ��ư�� ������ 

        // SaveFile�� ������ �� ���� ���� 
        if (!isSaveFileExist)
        {
            GameManager.instance.saveFileSeq.saveFileSeqList.Add(saveFileNum); //���̺����� ���೻���� �÷����� ���̺����� ���� 
            GameManager.instance.SaveSaveFileSeq();

            GameManager.instance.curSaveFileNum = saveFileNum; //���̺����� �ѹ� ���� 
            
            //GM �� ���� ���� �� ���� ���� Ȱ��ȭ�ǹǷ� �̹� GameData �� ��������ִ� ���� ~> ���⼭ �ʱ�ȭ��Ű�� ������ 
            GameManager.instance.gameData.curStageNum = 1;
            GameManager.instance.gameData.curAchievementNum = 0; //ó�� �����ϴ� ���� ~> ���� ������������ �ٽ� ���� 
            GameManager.instance.gameData.finalStageNum = 1;
            GameManager.instance.gameData.finalAchievementNum = 0; //���������Ȳ�� ���� 
            
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 50; j++)
                {
                    GameManager.instance.gameData.savePointUnlock[i, j] = false; //��� ���̺�����Ʈ ��Ȱ��ȭ 
                }
            }
            //�ʱ�ȭ��Ų GameData [saveFileNum]�� ���� 
            string ToJsonData = JsonUtility.ToJson(GameManager.instance.gameData);
            string filePath = Application.persistentDataPath + GameManager.instance.gameDataFileNames[saveFileNum];
            File.WriteAllText(filePath, ToJsonData);

            //���� �÷��̾ �� ��ġ�� ��ȯ�� �� �ֵ��� GameManager �� �ʱ�ȭ���� �� 
            GameManager.instance.nextScene = 30;
            GameManager.instance.shouldSpawnSavePoint = false;
            GameManager.instance.shouldUseOpeningElevator = true;

            //nextPos, nextDir �� ������ �̵��� ���� savePointManager���� �˾Ƽ� ������ �� 

            SceneManager.LoadScene(GameManager.instance.nextScene);
        }
        // SaveFile�� ������ Load
        else
        {
            GameManager.instance.saveFileSeq.saveFileSeqList.Remove(saveFileNum);
            GameManager.instance.saveFileSeq.saveFileSeqList.Add(saveFileNum);            
            GameManager.instance.SaveSaveFileSeq();
            //���� �������� �÷����� ���̺����� ���� 

            GameManager.instance.curSaveFileNum = saveFileNum; //���̺����� �ѹ� ����

            string filePath = Application.persistentDataPath + GameManager.instance.gameDataFileNames[GameManager.instance.curSaveFileNum];
            string FromJsonData = File.ReadAllText(filePath);
            GameData curGameData = JsonUtility.FromJson<GameData>(FromJsonData); //���� ������ ���̺������� GameData ������

            //GM ������ Ȱ��ȭ 
            GameManager.instance.nextScene = curGameData.respawnScene;
            GameManager.instance.nextPos = curGameData.respawnPos;
            GameManager.instance.nextGravityDir = curGameData.respawnGravityDir;
            GameManager.instance.nextState = Player.States.Walk; //States.Walk �� �� ���� 

            GameManager.instance.shouldSpawnSavePoint = true;

            SceneManager.LoadScene(GameManager.instance.nextScene);
        }

        
    }

    public void saveDelete() //���̺����� ������ ������ ó�� �����ϴ� ���·� �ʱ�ȭ��Ŵ 
    {
        UIManager.instance.clickSoundGen();
        Debug.Log("delete");

        string filePath = Application.persistentDataPath + GameManager.instance.gameDataFileNames[saveFileNum];
        File.Delete(filePath);
        
        text.text = "�� ����";

        bool isEveryFileDelete  = true; //���� ��� ���̺������� �����Ǿ��ٸ� saveFileSeq ���ϵ� �����ؾ� �� 
        for(int index = 0; index < 4; index++)
        {
            string savefilePath = Application.persistentDataPath + GameManager.instance.gameDataFileNames[index];
            if (File.Exists(savefilePath)) //�� ���̺����� �� �ϳ��� �����Ͱ� �ִٸ� isEveryFileDelete = false;
            {
                isEveryFileDelete = false;
            }
        }

        if (isEveryFileDelete)
        {
            string seqfilePath = Application.persistentDataPath + "/SaveFileSeq.json";
            File.Delete(seqfilePath);
            gameStartButton.GetComponent<NewGameButton>().text.text = "�����ϱ�"; //��� �����Ͱ� �����Ǿ����Ƿ� �ٽ� �����ϱ� ��ư ���� 
            gameStartButton.GetComponent<NewGameButton>().isSaveFileExist = false;
        }

        saveDeleteWindow.SetActive(false);
    }

    public void saveDeleteWindowOpen()
    {
        UIManager.instance.clickSoundGen();

        saveDeleteWindow.SetActive(true);

        for(int index=0; index<4; index++)
        {
            saveDeleteButton.SetActive(true);
        }
    }
}
