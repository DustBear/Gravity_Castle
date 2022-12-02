using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class SaveFileButton : MonoBehaviour
{
    [SerializeField] int saveFileNum; 
    Text text;
    [SerializeField] bool isSaveFileExist;

    public GameObject betaModeWindow; 
    public GameObject saveDeleteWindow;
    public GameObject gameStartButton;

    public GameObject saveDeleteButton;
    public NewGameButton _newGameButton;

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
            text.text = "스테이지" +"\n"+ keyVal.Key + "_" + keyVal.Value;
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
        Cursor.lockState = CursorLockMode.Locked;
        //버튼 중복입력 막기 위해 버튼 누르자 마자 락 검 

        UIManager.instance.clickSoundGen();
        UIManager.instance.FadeOut(1f);

        // 저장된 saveFile 이 없으면
        if (!isSaveFileExist)
        {
            GameManager.instance.saveFileSeq.saveFileSeqList.Remove(saveFileNum); //기존에 플레이한 기록 있으면 지우고 
            GameManager.instance.saveFileSeq.saveFileSeqList.Add(saveFileNum); //세이브포인트 실행기록에 현재 index 추가
            GameManager.instance.SaveSaveFileSeq(); //저장

            GameManager.instance.curSaveFileNum = saveFileNum; //현재 플레이중인 세이브번호 GM에 저장

            //게임데이터 초기화
            GameManager.instance.gameData.curStageNum = 1;
            GameManager.instance.gameData.curAchievementNum = 0; 
            GameManager.instance.gameData.finalStageNum = 1;
            GameManager.instance.gameData.finalAchievementNum = 0;

            GameManager.instance.gameData.savePointUnlock = new int[100]; //savePointGroup 초기화 
            GameManager.instance.gameData.collectionUnlock = new bool[240]; //collectionGroup 초기화 

            GameManager.instance.gameData.SpawnSavePoint_bool = false; //맨 처음 시작할 땐 세이브포인트가 아니라 엘리베이터에서 시작 
            GameManager.instance.gameData.UseOpeningElevetor_bool = true;

            for (int i = 0; i < GameManager.instance.gameData.savePointUnlock.Length; i++)
            {
                GameManager.instance.gameData.savePointUnlock[i] = 0;
            }
            for (int i = 0; i < GameManager.instance.gameData.collectionUnlock.Length; i++)
            {
                GameManager.instance.gameData.collectionUnlock[i] = false;
            }

            GameManager.instance.gameData.collectionTmp = new List<int>(); //처음에는 비어 있는 배열이어야 함 

            //새로 만든 GM 데이터를 알맞은 번호의 gameData에 저장 
            string ToJsonData = JsonUtility.ToJson(GameManager.instance.gameData);
            string filePath = Application.persistentDataPath + GameManager.instance.gameDataFileNames[saveFileNum];
            File.WriteAllText(filePath, ToJsonData);

            //gameData 값으로 당장 필요한 GameManager 갱신
            int tmpNextScene = SceneUtility.GetBuildIndexByScenePath("Assets/_Scenes/_tutorial/tutorial.unity"); //tutorial scene index
            GameManager.instance.nextScene = tmpNextScene;

            GameManager.instance.gameData.SpawnSavePoint_bool = false; //맨 처음 시작하므로 따로 세이브포인트가 없다 
            GameManager.instance.gameData.UseOpeningElevetor_bool = true; //오프닝 엘리베이터 이용해야 함 

            GameManager.instance.shouldSpawnSavePoint = GameManager.instance.gameData.SpawnSavePoint_bool;
            GameManager.instance.shouldUseOpeningElevator = GameManager.instance.gameData.UseOpeningElevetor_bool;

            StartCoroutine(loadSceneDelay(1));
        }
        // 이미 세이브 파일이 있으면 
        else
        {
            GameManager.instance.saveFileSeq.saveFileSeqList.Remove(saveFileNum);
            GameManager.instance.saveFileSeq.saveFileSeqList.Add(saveFileNum);            
            GameManager.instance.SaveSaveFileSeq();
            //마지막으로 플레이한 세이브번호 반영 

            GameManager.instance.curSaveFileNum = saveFileNum; //현재 플레이중인 세이브번호 GM에 저장

            //알맞은 번호의 gameData에서 값 가져와 curGameData에 보관 
            string filePath = Application.persistentDataPath + GameManager.instance.gameDataFileNames[GameManager.instance.curSaveFileNum];
            string FromJsonData = File.ReadAllText(filePath);
            GameData curGameData = JsonUtility.FromJson<GameData>(FromJsonData); 

            //불러온 데이터를 GM의 게임 데이터에 덮어쓰기 함
            GameManager.instance.gameData.curStageNum = curGameData.curStageNum;
            GameManager.instance.gameData.curAchievementNum = curGameData.curAchievementNum;

            GameManager.instance.gameData.finalStageNum = curGameData.finalStageNum;
            GameManager.instance.gameData.finalAchievementNum = curGameData.finalAchievementNum;

            GameManager.instance.gameData.savePointUnlock = curGameData.savePointUnlock;
            GameManager.instance.gameData.collectionUnlock = curGameData.collectionUnlock;

            GameManager.instance.gameData.SpawnSavePoint_bool = curGameData.SpawnSavePoint_bool;
            GameManager.instance.gameData.UseOpeningElevetor_bool = curGameData.UseOpeningElevetor_bool;

            //collectionTmp는 게임을 시작할 땐 항상 비워둬야 함
            GameManager.instance.gameData.collectionTmp = new List<int>();

            //GM 에 필요한 데이터 gameData에서 불러와 초기화하기 
            GameManager.instance.nextScene = GameManager.instance.gameData.respawnScene;
            GameManager.instance.nextPos = GameManager.instance.gameData.respawnPos;
            GameManager.instance.nextGravityDir = GameManager.instance.gameData.respawnGravityDir;
            GameManager.instance.nextState = Player.States.Walk;

            GameManager.instance.shouldSpawnSavePoint = GameManager.instance.gameData.SpawnSavePoint_bool;
            GameManager.instance.shouldUseOpeningElevator = GameManager.instance.gameData.UseOpeningElevetor_bool;

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

    public void saveDelete() //세이브파일 지우기  
    {       
        UIManager.instance.clickSoundGen();

        string filePath = Application.persistentDataPath + GameManager.instance.gameDataFileNames[saveFileNum];
        File.Delete(filePath);
        
        text.text = "새 게임";
        isSaveFileExist = false;

        if (saveFileNum == GameManager.instance.saveFileSeq.saveFileSeqList.Last())
        {
            _newGameButton.isSaveFileExist = false;
            _newGameButton.text.text = "새로하기";
            //마지막으로 플레이했던 세이브를 삭제했기 때문에 세이브 없음 
        }

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
