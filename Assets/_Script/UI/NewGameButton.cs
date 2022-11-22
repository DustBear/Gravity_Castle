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

    void Awake()
    {
        text = GetComponentInChildren<Text>();
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

            GameManager.instance.gameData.savePointUnlock = new int[100]; //세이브포인트 그룹 새로 초기화 
            GameManager.instance.gameData.collectionUnlock = new bool[240]; //30x8 

            //세이브포인트 및 컬렉션데이터 초기화 
            for (int i = 0; i < GameManager.instance.gameData.savePointUnlock.Length; i++)
            {
                GameManager.instance.gameData.savePointUnlock[i] = 0;
            }
            for (int i = 0; i < GameManager.instance.gameData.collectionUnlock.Length; i++)
            {
                GameManager.instance.gameData.collectionUnlock[i] = false;
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

            UIManager.instance.FadeOut(1f);
            StartCoroutine(loadSceneDelay(1));
        }
        
        //세이브파일이 이미 있으면 불러오기 
        else
        {
            Debug.Log("load");
            //GameManager.instance.curSaveFileNum = GameManager.instance.saveFileSeq.saveFileSeqList.Last(); //마지막으로 실행했던 세이브파일 가져옴 
            
            string filePath = Application.persistentDataPath + GameManager.instance.gameDataFileNames[GameManager.instance.curSaveFileNum];
            string FromJsonData = File.ReadAllText(filePath);

            //GameData curGameData = JsonUtility.FromJson<GameData>(FromJsonData); //선택한 세이브파일의 GameData 불러옴 
           // GameManager.instance.gameData = JsonUtility.FromJson<GameData>(FromJsonData);

            //GM 데이터 갱신 
            GameManager.instance.nextScene = GameManager.instance.gameData.respawnScene;
            GameManager.instance.nextPos = GameManager.instance.gameData.respawnPos;
            GameManager.instance.nextGravityDir = GameManager.instance.gameData.respawnGravityDir;
            GameManager.instance.nextState = Player.States.Walk; //States.Walk 가 기본값 

            GameManager.instance.shouldSpawnSavePoint = true;
            if(GameManager.instance.gameData.curAchievementNum == 0)
            {
                GameManager.instance.shouldSpawnSavePoint = false;
                GameManager.instance.shouldUseOpeningElevator = true; 
            }

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
