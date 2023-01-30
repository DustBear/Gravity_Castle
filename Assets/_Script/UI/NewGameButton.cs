using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.UI;

public class NewGameButton : MonoBehaviour
{
    public Text text;
    public bool isSaveFileExist;

    int languageIndex_cur = 0;
    int languageIndex_last = 0;

    void Awake()
    {
        text = GetComponentInChildren<Text>();
    }


    void Start()
    {
        // SaveFileSeqList의 길이가 0보다만 크면 ~> 플레이한 기록이 있는 게임
        if (GameManager.instance.saveFileSeq.saveFileSeqList.Count != 0)
        {
            int lastPlayedSaveNum = GameManager.instance.saveFileSeq.saveFileSeqList.Last();
            //마지막으로 실행했던 세이브 번호 
            string savefilePath = Application.persistentDataPath + GameManager.instance.gameDataFileNames[lastPlayedSaveNum];

            if (File.Exists(savefilePath)) //만약 마지막으로 플레이했던 파일이 존재한다면 
            {
                text.text = gameTextManager.instance.systemTextManager(2);
                isSaveFileExist = true;
            }
            else
            {
                //플레이한 기록이 있더라도 이전에 플레이하던 세이브를 삭제했다면 새로하기 해야 함 
                text.text = gameTextManager.instance.systemTextManager(1);
                isSaveFileExist = false;
            }
        }
        else
        {
            //플레이한 기록이 없다면 새로 하기 
            text.text = gameTextManager.instance.systemTextManager(1);
            isSaveFileExist = false;
        }
    }

    private void Update()
    {
        languageIndex_cur = gameTextManager.instance.selectedLanguageNum;
        if (languageIndex_cur != languageIndex_last) //언어 세팅이 바뀌면 
        {
            if (isSaveFileExist)
            {
                text.text = gameTextManager.instance.systemTextManager(2);
            }
            else
            {
                text.text = gameTextManager.instance.systemTextManager(1);
            }
        }
        languageIndex_last = languageIndex_cur;
    }

    public void OnClickButton() 
    {       
        Cursor.lockState = CursorLockMode.Locked;
        //버튼 중복입력 막기 위해 버튼 누르자 마자 락 검 

        UIManager.instance.clickSoundGen();
        //버튼 입력소리 재생 
        
        // SaveFile 이 존재하지 않으면 : 새로 시작하는 게임이면 
        if (!isSaveFileExist)
        {
            for(int index=0; index<4; index++)
            {
                //0부터 3까지 인덱스를 올라가면서 가장 먼저 나오는 빈 세이브에서 자동으로 시작함
                string savefilePath = Application.persistentDataPath + GameManager.instance.gameDataFileNames[index];
                if (!File.Exists(savefilePath)) //해당 인덱스의 세이브파일이 비어 있다면 
                {
                    GameManager.instance.curSaveFileNum = index;
                    //새로운 인덱스의 세이브파일 생성 

                    GameManager.instance.saveFileSeq.saveFileSeqList.Remove(index);
                    GameManager.instance.saveFileSeq.saveFileSeqList.Add(index);
                    GameManager.instance.SaveSaveFileSeq();
                    //세이브 실행기록에 반영 

                    break;
                }
            }

            //GameData 갱신 
            GameManager.instance.gameData.curStageNum = 1;
            GameManager.instance.gameData.curAchievementNum = 0; //맨 처음 시작하면 achiNum = 0  

            GameManager.instance.gameData.finalStageNum = 1;
            GameManager.instance.gameData.finalAchievementNum = 0;

            GameManager.instance.gameData.savePointUnlock = new int[100]; //세이브포인트 그룹 새로 초기화 
            GameManager.instance.gameData.collectionUnlock = new bool[240]; //30x8 

            GameManager.instance.gameData.SpawnSavePoint_bool = false; //맨 처음 시작할 땐 세이브포인트가 아니라 엘리베이터에서 시작 
            GameManager.instance.gameData.UseOpeningElevetor_bool = true;

            //세이브포인트 및 컬렉션데이터 초기화 
            for (int i = 0; i < GameManager.instance.gameData.savePointUnlock.Length; i++)
            {
                GameManager.instance.gameData.savePointUnlock[i] = 0;
            }
            for (int i = 0; i < GameManager.instance.gameData.collectionUnlock.Length; i++)
            {
                GameManager.instance.gameData.collectionUnlock[i] = false;
            }

            GameManager.instance.gameData.collectionTmp = new List<int>(); //처음에는 비어 있는 배열이어야 함 
            GameManager.instance.gameData.respawnScene = SceneUtility.GetBuildIndexByScenePath("Assets/_Scenes/_tutorial/tutorial.unity"); //tutorial scene index

            //GameData 에 데이터 저장 
            string ToJsonData = JsonUtility.ToJson(GameManager.instance.gameData);
            string filePath = Application.persistentDataPath + GameManager.instance.gameDataFileNames[GameManager.instance.curSaveFileNum];
            File.WriteAllText(filePath, ToJsonData);

            //GameData 초기화했으면 당장 플레이에 필요한 GM 데이터 갱신 
            GameManager.instance.nextScene = GameManager.instance.gameData.respawnScene;

            //nextPos, nextDir 등은 일단 다음 씬으로 보내고 나면 오프닝 엘리베이터 매니져가 알아서 조정해 줄 테니 지금 설정할 필요x

            UIManager.instance.FadeOut(1f);
            StartCoroutine(loadSceneDelay(1));
        }
        
        //세이브파일이 이미 있으면 불러오기 
        else
        {
            Debug.Log("load");
            GameManager.instance.curSaveFileNum = GameManager.instance.saveFileSeq.saveFileSeqList.Last(); //마지막으로 실행했던 세이브파일 번호 가져옴 

            //세이브파일 실행 순서 기록 초기화 
            GameManager.instance.saveFileSeq.saveFileSeqList.Remove(GameManager.instance.curSaveFileNum);
            GameManager.instance.saveFileSeq.saveFileSeqList.Add(GameManager.instance.curSaveFileNum);
            GameManager.instance.SaveSaveFileSeq();

            string filePath = Application.persistentDataPath + GameManager.instance.gameDataFileNames[GameManager.instance.curSaveFileNum];
            string FromJsonData = File.ReadAllText(filePath);
            GameData curGameData = JsonUtility.FromJson<GameData>(FromJsonData); //선택한 세이브파일 번호에 맞는 GameData 불러옴 

            //불러온 데이터를 GM의 게임 데이터에 덮어쓰기 함
            GameManager.instance.gameData.curStageNum = curGameData.curStageNum;
            GameManager.instance.gameData.curAchievementNum = curGameData.curAchievementNum;

            GameManager.instance.gameData.finalStageNum = curGameData.finalStageNum;
            GameManager.instance.gameData.finalAchievementNum = curGameData.finalAchievementNum;

            GameManager.instance.gameData.savePointUnlock = curGameData.savePointUnlock;
            GameManager.instance.gameData.collectionUnlock = curGameData.collectionUnlock;

            GameManager.instance.gameData.SpawnSavePoint_bool = true;
            //세이브파일이 있다는 것은 이미 한 번 게임을 종료한 적이 있다는 뜻 
            //어떤 방법으로든 종료했다면 그 시점에서 다음 판은 무조건 세이브에서 이어져야 한다

            GameManager.instance.gameData.UseOpeningElevetor_bool = curGameData.UseOpeningElevetor_bool;

            GameManager.instance.gameData.respawnScene = curGameData.respawnScene;

            //에러로 인해 achievement가 0에서 1로 올라가지 않았을 때 자동으로 보정해 줌 ~> 사실 필요없지만 리스크 관리로 일단 둠 
            if (GameManager.instance.gameData.curAchievementNum == 0)
            {
                GameManager.instance.gameData.SpawnSavePoint_bool = false;
                GameManager.instance.gameData.UseOpeningElevetor_bool = true;
            }

            //GameData 에 데이터 저장 
            string ToJsonData = JsonUtility.ToJson(GameManager.instance.gameData);
            string _filePath = Application.persistentDataPath + GameManager.instance.gameDataFileNames[GameManager.instance.curSaveFileNum];
            File.WriteAllText(_filePath, ToJsonData);

            //collectionTmp는 게임을 시작할 땐 항상 비워둬야 함
            GameManager.instance.gameData.collectionTmp = new List<int>();

            //GM 자체의 데이터 갱신 
            GameManager.instance.nextScene = curGameData.respawnScene;
            GameManager.instance.nextPos = curGameData.respawnPos;
            GameManager.instance.nextGravityDir = curGameData.respawnGravityDir;
            GameManager.instance.nextState = Player.States.Walk; //States.Walk 가 기본값 

            UIManager.instance.FadeOut(1f);
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
}
