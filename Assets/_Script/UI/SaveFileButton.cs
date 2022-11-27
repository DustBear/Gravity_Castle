using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SaveFileButton : MonoBehaviour
{
    [SerializeField] int saveFileNum; 
    Text text;
    [SerializeField] bool isSaveFileExist;

    public GameObject betaModeWindow; 
    public GameObject saveDeleteWindow;
    public GameObject gameStartButton;

    public GameObject saveDeleteButton;

    void Awake()
    {
        text = GetComponentInChildren<Text>();
        gameStartButton = GameObject.Find("NewGameButton");
    }

    void Start()
    {      
        KeyValuePair<int, int> keyVal = GameManager.instance.GetSavedData(saveFileNum);
        if (keyVal.Key != -1) 
        {
            text.text = "세이브" + (saveFileNum + 1) + " : 스테이지" + keyVal.Key + "_" + keyVal.Value;
            isSaveFileExist = true;
        }
        else
        {
            text.text = "새 게임";
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
        UIManager.instance.FadeOut(1f);

        GameManager.instance.curSaveFileNum = saveFileNum; //GM의 현재 세이브파일 넘버 바꿔줌 

        // 저장된 saveFile 이 없으면
        if (!isSaveFileExist)
        {
            GameManager.instance.saveFileSeq.saveFileSeqList.Add(saveFileNum); //세이브포인트 실행기록에 현재 index 추가
            GameManager.instance.SaveSaveFileSeq();

            GameManager.instance.curSaveFileNum = saveFileNum; 
            
            //게임데이터 초기화
            GameManager.instance.gameData.curStageNum = 1;
            GameManager.instance.gameData.curAchievementNum = 0; 
            GameManager.instance.gameData.finalStageNum = 1;
            GameManager.instance.gameData.finalAchievementNum = 0;

            GameManager.instance.gameData.savePointUnlock = new int[100]; //savePointGroup 초기화 
            GameManager.instance.gameData.collectionUnlock = new bool[240]; //collectionGroup 초기화 

            for (int i = 0; i < GameManager.instance.gameData.savePointUnlock.Length; i++)
            {
                GameManager.instance.gameData.savePointUnlock[i] = 0;
            }
            for (int i = 0; i < GameManager.instance.gameData.collectionUnlock.Length; i++)
            {
                GameManager.instance.gameData.collectionUnlock[i] = false;
            }

            GameManager.instance.gameData.collectionTmp = new List<int>(); //처음에는 비어 있는 배열이어야 함 

            //세이브포인트 저장
            string ToJsonData = JsonUtility.ToJson(GameManager.instance.gameData);
            string filePath = Application.persistentDataPath + GameManager.instance.gameDataFileNames[saveFileNum];
            File.WriteAllText(filePath, ToJsonData);

            //GameManager 갱신
            int tmpNextScene = SceneUtility.GetBuildIndexByScenePath("Assets/_Scenes/_tutorial/tutorial.unity"); //tutorial scene index
            GameManager.instance.nextScene = tmpNextScene;

            GameManager.instance.shouldSpawnSavePoint = false; //맨 처음 시작하므로 따로 세이브포인트가 없다 
            GameManager.instance.shouldUseOpeningElevator = true; //오프닝 엘리베이터 이용해야 함 

            StartCoroutine(loadSceneDelay(1));
        }
        // 이미 세이브 파일이 있으면 
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

            StartCoroutine(loadSceneDelay(2));
        }       
    }

    IEnumerator loadSceneDelay(int type)
    {
        yield return new WaitForSeconds(1.5f);
        switch (type)
        {
            case 1:
                SceneManager.LoadScene("openingScene"); //오프닝씬으로 들렀다가 nextScene으로 이동 
                break;
            case 2:
                SceneManager.LoadScene(GameManager.instance.nextScene);
                break;
        }
    }

    public void saveDelete() //���̺����� ������ ������ ó�� �����ϴ� ���·� �ʱ�ȭ��Ŵ 
    {
        UIManager.instance.clickSoundGen();
        //Debug.Log("delete");

        string filePath = Application.persistentDataPath + GameManager.instance.gameDataFileNames[saveFileNum];
        File.Delete(filePath);
        
        text.text = "새 게임";
        isSaveFileExist = false;

        bool isEveryFileDelete  = true; //만약 모든 세이브파일이 삭제되었다면 saveFileSeq 도 같이 삭제해야 함 
        for(int index = 0; index < 4; index++)
        {
            string savefilePath = Application.persistentDataPath + GameManager.instance.gameDataFileNames[index];
            if (File.Exists(savefilePath)) //하나라도 존재하는 파일이 있으면 isEveryFileDelete = false;
            {
                isEveryFileDelete = false;
            }
        }

        //모든 데이터를 삭제했다면 saveFileSeq 도 삭제해야 함 
        if (isEveryFileDelete)
        {
            string seqfilePath = Application.persistentDataPath + "/SaveFileSeq.json";
            File.Delete(seqfilePath);
            gameStartButton.GetComponent<NewGameButton>().text.text = "새로하기"; //gameStart 버튼 바꾸기 
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
