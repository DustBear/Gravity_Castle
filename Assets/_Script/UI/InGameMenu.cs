using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class InGameMenu : MonoBehaviour
{   
    public void OnClickExit() //메인메뉴로 나가기 버튼 누를 때 
    {
        UIManager.instance.clickSoundGen();

        Time.timeScale = 1f;

        if (GameManager.instance.gameData.curAchievementNum == 0)
        {
            //opening elevator에서 내리고 첫 번째 세이브를 활성화하기 전에 죽으면 그냥 save1 까지는 활성화시킨 것으로 치고 넘어감  

            GameManager.instance.gameData.curAchievementNum = 1;
            if (GameManager.instance.gameData.finalStageNum == GameManager.instance.gameData.curStageNum)
            {
                if (GameManager.instance.gameData.finalAchievementNum == 0)
                {
                    GameManager.instance.gameData.finalAchievementNum = 1; 
                    //만약 현재 최고 진행도 이상이라면 final data 역시 갱신해줘야 함 
                }
            }
            GameManager.instance.gameData.savePointUnlock[GameManager.instance.saveNumCalculate(new Vector2(GameManager.instance.gameData.curStageNum, 1))] = 1; //세이브포인트 1 활성화 
            GameManager.instance.gameData.respawnScene = SceneManager.GetActiveScene().buildIndex; // 

            //세이브파일에 데이터 저장 
            string ToJsonData = JsonUtility.ToJson(GameManager.instance.gameData);
            string filePath = Application.persistentDataPath + GameManager.instance.gameDataFileNames[GameManager.instance.curSaveFileNum];
            File.WriteAllText(filePath, ToJsonData);

            GameManager.instance.nextScene = GameManager.instance.gameData.respawnScene;
            GameManager.instance.shouldUseOpeningElevator = false;
        }

        UIManager.instance.FadeOut(1f);
        Invoke("loadMainMenu", 1.5f);
    }

    public void OnClickMainStage() //인게임 메뉴로 나가기 버튼 누를 때 
    {
        UIManager.instance.clickSoundGen();        
        Time.timeScale = 1f;

        if(GameManager.instance.gameData.curAchievementNum == 0)
        {
            //opening Elevator 작동중이고 아직 첫 세이브포인트를 활성화시키지 않았을 때 

            GameManager.instance.gameData.curAchievementNum = 1; 
            if (GameManager.instance.gameData.finalStageNum == GameManager.instance.gameData.curStageNum)
            {
                if (GameManager.instance.gameData.finalAchievementNum == 0)
                {
                    GameManager.instance.gameData.finalAchievementNum = 1; //진행도 갱신한 거면 데이터 반영해 줌
                }
            }
            GameManager.instance.gameData.savePointUnlock[GameManager.instance.saveNumCalculate(new Vector2(GameManager.instance.gameData.curStageNum, 1))] = 1; 
            GameManager.instance.gameData.respawnScene = SceneManager.GetActiveScene().buildIndex;

            //세이브파일에 데이터 저장 
            string ToJsonData = JsonUtility.ToJson(GameManager.instance.gameData);
            string filePath = Application.persistentDataPath + GameManager.instance.gameDataFileNames[GameManager.instance.curSaveFileNum];
            File.WriteAllText(filePath, ToJsonData);

            GameManager.instance.nextScene = GameManager.instance.gameData.respawnScene;
            GameManager.instance.shouldUseOpeningElevator = false;
        }

        UIManager.instance.FadeOut(1f);
        Invoke("loadInGameMenu", 1.5f);
    }

    void loadInGameMenu()
    {
        SceneManager.LoadScene("InGameMenu");
        Cursor.lockState = CursorLockMode.None;

        gameObject.SetActive(false);
    }

    void loadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");

        gameObject.SetActive(false);
    }
}
