using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using UnityEngine.SceneManagement;

public class SaveFileButton : MonoBehaviour
{
    [SerializeField] int saveFileNum; //이 버튼의 세이브파일 번호
    TextMeshProUGUI text;
    bool isSaveFileExist;

    public GameObject betaModeWindow; //베타테스트 버전으로 바꾸시겠습니까? 창 띄움 
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
            text.text = "Save" + (saveFileNum + 1) + " : 스테이지" + keyVal.Key + "_" + keyVal.Value;
            isSaveFileExist = true;
        }
        else
        {
            text.text = "새 파일";
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

        GameManager.instance.curSaveFileNum = saveFileNum; //현재 플레이중인 세이브파일 = 이 세이브파일 로드 버튼의 설정값 

        // SaveFile이 없으면 새 게임 생성 
        if (!isSaveFileExist)
        {
            GameManager.instance.saveFileSeq.saveFileSeqList.Add(saveFileNum); //세이브파일 실행내역에 플레이한 세이브파일 저장 
            GameManager.instance.SaveSaveFileSeq();

            GameManager.instance.curSaveFileNum = saveFileNum; //세이브파일 넘버 설정 
            
            //GM 은 게임 실행 시 가장 먼저 활성화되므로 이미 GameData 는 만들어져있는 상태 ~> 여기서 초기화시키고 진행함 
            GameManager.instance.gameData.curStageNum = 1;
            GameManager.instance.gameData.curAchievementNum = 0; //처음 시작하는 게임 ~> 최초 스테이지에서 다시 시작 
            GameManager.instance.gameData.finalStageNum = 1;
            GameManager.instance.gameData.finalAchievementNum = 0; //최종진행상황은 동일 
            
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 50; j++)
                {
                    GameManager.instance.gameData.savePointUnlock[i, j] = false; //모든 세이브포인트 비활성화 
                }
            }
            //초기화시킨 GameData [saveFileNum]에 저장 
            string ToJsonData = JsonUtility.ToJson(GameManager.instance.gameData);
            string filePath = Application.persistentDataPath + GameManager.instance.gameDataFileNames[saveFileNum];
            File.WriteAllText(filePath, ToJsonData);

            //실제 플레이어가 제 위치에 소환될 수 있도록 GameManager 도 초기화시켜 줌 
            GameManager.instance.nextScene = 30;
            GameManager.instance.shouldSpawnSavePoint = false;
            GameManager.instance.shouldUseOpeningElevator = true;

            //nextPos, nextDir 은 씬으로 이동한 다음 savePointManager에서 알아서 조정해 줌 

            SceneManager.LoadScene(GameManager.instance.nextScene);
        }
        // SaveFile이 있으면 Load
        else
        {
            GameManager.instance.saveFileSeq.saveFileSeqList.Remove(saveFileNum);
            GameManager.instance.saveFileSeq.saveFileSeqList.Add(saveFileNum);            
            GameManager.instance.SaveSaveFileSeq();
            //가장 마지막에 플레이한 세이브파일 갱신 

            GameManager.instance.curSaveFileNum = saveFileNum; //세이브파일 넘버 설정

            string filePath = Application.persistentDataPath + GameManager.instance.gameDataFileNames[GameManager.instance.curSaveFileNum];
            string FromJsonData = File.ReadAllText(filePath);
            GameData curGameData = JsonUtility.FromJson<GameData>(FromJsonData); //현재 선택한 세이브파일의 GameData 가져옴

            //GM 데이터 활성화 
            GameManager.instance.nextScene = curGameData.respawnScene;
            GameManager.instance.nextPos = curGameData.respawnPos;
            GameManager.instance.nextGravityDir = curGameData.respawnGravityDir;
            GameManager.instance.nextState = Player.States.Walk; //States.Walk 로 씬 시작 

            GameManager.instance.shouldSpawnSavePoint = true;

            SceneManager.LoadScene(GameManager.instance.nextScene);
        }

        
    }

    public void saveDelete() //세이브파일 정보를 게임을 처음 시작하는 상태로 초기화시킴 
    {
        UIManager.instance.clickSoundGen();
        Debug.Log("delete");

        string filePath = Application.persistentDataPath + GameManager.instance.gameDataFileNames[saveFileNum];
        File.Delete(filePath);
        
        text.text = "새 파일";

        bool isEveryFileDelete  = true; //만약 모든 세이브파일이 삭제되었다면 saveFileSeq 파일도 삭제해야 함 
        for(int index = 0; index < 4; index++)
        {
            string savefilePath = Application.persistentDataPath + GameManager.instance.gameDataFileNames[index];
            if (File.Exists(savefilePath)) //네 세이브파일 중 하나라도 데이터가 있다면 isEveryFileDelete = false;
            {
                isEveryFileDelete = false;
            }
        }

        if (isEveryFileDelete)
        {
            string seqfilePath = Application.persistentDataPath + "/SaveFileSeq.json";
            File.Delete(seqfilePath);
            gameStartButton.GetComponent<NewGameButton>().text.text = "새로하기"; //모든 데이터가 삭제되었으므로 다시 새로하기 버튼 생성 
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
