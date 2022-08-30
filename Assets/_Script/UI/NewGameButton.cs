using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.SceneManagement;
using System.IO;

public class NewGameButton : MonoBehaviour
{
    public TextMeshProUGUI text;
    public bool isSaveFileExist;

    void Awake()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
    }


    void Start()
    {
        // SaveFile이 하나라도 존재하면 "이어하기"로 텍스트 변경
        if (GameManager.instance.saveFileSeq.saveFileSeqList.Count != 0)
        {
            text.text = "이어하기";
            isSaveFileExist = true;
        }
        else
        {
            text.text = "새로하기";
            isSaveFileExist = false;
        }
    }

    public void OnClickButton() 
    {
        UIManager.instance.clickSoundGen();
        
        // SaveFile이 없으면 새로하기 ~> GameData 새로 만들고 stageNum, achNum 넣어준 뒤 튜토리얼씬으로 이동시킴 
        if (!isSaveFileExist)
        {
            Debug.Log("new game");

            GameManager.instance.curSaveFileNum = 0; //첫 세이브 파일 생성 (1번 세이브) 
            GameManager.instance.saveFileSeq.saveFileSeqList.Add(0); //마지막으로 실행한 세이브번호 갱신 
            GameManager.instance.SaveSaveFileSeq();

            //GM 은 게임 실행 시 가장 먼저 활성화되므로 이미 GameData 는 만들어져있는 상태 ~> 여기서 초기화시키고 진행함 
            GameManager.instance.gameData.curStageNum = 1;
            GameManager.instance.gameData.curAchievementNum = 0; //처음 시작하는 게임 ~> 최초 스테이지에서 다시 시작 
            GameManager.instance.gameData.finalStageNum = 1;
            GameManager.instance.gameData.finalAchievementNum = 0; //최종진행상황은 동일 

            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 50; j++)
                {
                    GameManager.instance.gameData.savePointUnlock[i, j] = false; //모든 세이브포인트 비활성화 (게임을 처음 시작하므로) 
                }
            }

            //초기화시킨 GameData [0]에 저장 
            string ToJsonData = JsonUtility.ToJson(GameManager.instance.gameData);
            string filePath = Application.persistentDataPath + GameManager.instance.gameDataFileNames[0];
            File.WriteAllText(filePath, ToJsonData);

            GameManager.instance.nextScene = 30; //스테이지1 
            //nextPos, nextDir 은 씬으로 이동한 다음 savePointManager에서 알아서 조정해 줌 

            GameManager.instance.shouldSpawnSavePoint = false;
            GameManager.instance.shouldUseOpeningElevator = true; //씬을 새로 시작하므로 openingElevator 에서 내려와야 함 

            SceneManager.LoadScene(GameManager.instance.nextScene);
        }
        // SaveFile이 있으면 가장 최근 SaveFile을 Load
        else
        {
            Debug.Log("load game");

            GameManager.instance.curSaveFileNum = GameManager.instance.saveFileSeq.saveFileSeqList.Last(); //가장 마지막에 플레이했던 세이브파일 번호 가져옴 
            
            string filePath = Application.persistentDataPath + GameManager.instance.gameDataFileNames[GameManager.instance.curSaveFileNum];
            string FromJsonData = File.ReadAllText(filePath);

            GameData curGameData = JsonUtility.FromJson<GameData>(FromJsonData); //현재 선택한 세이브파일의 GameData 가져옴

            //가져온 GameData 에서 데이터 가져와서 GM에 집어넣음 
            
            GameManager.instance.nextScene = curGameData.respawnScene;
            GameManager.instance.nextPos = curGameData.respawnPos;
            GameManager.instance.nextGravityDir = curGameData.respawnGravityDir;
            GameManager.instance.nextState = Player.States.Walk; //States.Walk 로 씬 시작 

            GameManager.instance.shouldSpawnSavePoint = true;
            if(curGameData.curAchievementNum == 0)
            {
                GameManager.instance.shouldSpawnSavePoint = false;
                GameManager.instance.shouldUseOpeningElevator = true; //해당 스테이지를 처음 시작할 땐 opening Elevator 작동해야 함 
            }

            SceneManager.LoadScene(GameManager.instance.nextScene);
        }

    }
}
