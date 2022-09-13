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
        // SaveFileSeqList 에 아무런 값이 없으면 ~> 새로 시작하는 게임 
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
        
        // SaveFile 이 존재하지 않으면 ( 새로 시작하는 게임이면 )
        if (!isSaveFileExist)
        {
            GameManager.instance.curSaveFileNum = 0; // 1번 세이브포인트를 활성화시킴

            GameManager.instance.saveFileSeq.saveFileSeqList.Add(0); 
            GameManager.instance.SaveSaveFileSeq();

            //GameData 갱신 
            GameManager.instance.gameData.curStageNum = 1;
            GameManager.instance.gameData.curAchievementNum = 0; //맨 처음 시작하면 achiNum = 0  
            GameManager.instance.gameData.finalStageNum = 1;
            GameManager.instance.gameData.finalAchievementNum = 0; 

            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 50; j++)
                {
                    GameManager.instance.gameData.savePointUnlock[i, j] = false; //모든 세이브포인트 비활성화 
                }
            }

            //GameData [0] 에 데이터 저장 
            string ToJsonData = JsonUtility.ToJson(GameManager.instance.gameData);
            string filePath = Application.persistentDataPath + GameManager.instance.gameDataFileNames[0];
            File.WriteAllText(filePath, ToJsonData);

            //GameData 초기화했으면 GM 데이터 갱신 
            int tmpNextScene = SceneUtility.GetBuildIndexByScenePath("Assets/_Scenes/_tutorial/tutorial.unity"); //tutorial scene index
            GameManager.instance.nextScene = tmpNextScene;

            GameManager.instance.shouldSpawnSavePoint = false;
            GameManager.instance.shouldUseOpeningElevator = true;

            SceneManager.LoadScene("openingScene");
        }
        
        //세이브파일이 이미 있으면 불러오기 
        else
        {
            GameManager.instance.curSaveFileNum = GameManager.instance.saveFileSeq.saveFileSeqList.Last(); //마지막으로 실행했던 세이브파일 가져옴 
            
            string filePath = Application.persistentDataPath + GameManager.instance.gameDataFileNames[GameManager.instance.curSaveFileNum];
            string FromJsonData = File.ReadAllText(filePath);

            GameData curGameData = JsonUtility.FromJson<GameData>(FromJsonData); //선택한 세이브파일의 GameData 불러옴 
            
            //GM 데이터 갱신 
            GameManager.instance.nextScene = curGameData.respawnScene;
            GameManager.instance.nextPos = curGameData.respawnPos;
            GameManager.instance.nextGravityDir = curGameData.respawnGravityDir;
            GameManager.instance.nextState = Player.States.Walk; //States.Walk 가 기본값 

            GameManager.instance.shouldSpawnSavePoint = true;
            if(curGameData.curAchievementNum == 0)
            {
                GameManager.instance.shouldSpawnSavePoint = false;
                GameManager.instance.shouldUseOpeningElevator = true; 
            }

            SceneManager.LoadScene(GameManager.instance.nextScene);
        }

    }
}
