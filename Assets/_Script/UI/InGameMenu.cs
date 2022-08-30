using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class InGameMenu : MonoBehaviour
{
    [SerializeField] Vector2 boatPos; //플레이어가 스폰돼야 하는 배 안의 좌표 

    // 일시정지 해제 후 메인메뉴로
    public void OnClickExit()
    {
        UIManager.instance.clickSoundGen();

        Time.timeScale = 1f;

        if (GameManager.instance.gameData.curAchievementNum == 0)
        {
            //opening Elevator 타고 내려가는 도중 게임을 나오면 첫 세이브포인트까지는 활성화한 것으로 침 

            GameManager.instance.gameData.curAchievementNum = 1;
            if (GameManager.instance.gameData.finalStageNum == GameManager.instance.gameData.curStageNum)
            {
                if (GameManager.instance.gameData.finalAchievementNum == 0)
                {
                    GameManager.instance.gameData.finalAchievementNum = 1; //만약 현재 진행도가 최대 진행도일 경우 final Ach 에도 반영함 
                }
            }
            GameManager.instance.gameData.savePointUnlock[GameManager.instance.gameData.curStageNum - 1, 0] = true; //첫번째 세이브포인트 활성화시킴 
            GameManager.instance.gameData.respawnScene = SceneManager.GetActiveScene().buildIndex; //씬은 그대로 유지 

            //데이터 저장
            string ToJsonData = JsonUtility.ToJson(GameManager.instance.gameData);
            string filePath = Application.persistentDataPath + GameManager.instance.gameDataFileNames[GameManager.instance.curSaveFileNum];
            File.WriteAllText(filePath, ToJsonData);

            GameManager.instance.nextScene = GameManager.instance.gameData.respawnScene;
            GameManager.instance.shouldUseOpeningElevator = false;
        }

        SceneManager.LoadScene("MainMenu");

        if (InputManager.instance.isJumpBlocked)
        {
            InputManager.instance.isJumpBlocked = false;
            //jumpBlock은 엘리베이터를 타면 true. 엘리베이터 타고 있는 중간에 메뉴로 나가면 
            // 다음 씬을 시작할 때도 jumpBlock이 해제되지 않아서 점프 불가능한 문제 발생 ~> 직접 꺼서 해결하자 
        }

        gameObject.SetActive(false);
    }

    public void OnClickMainStage() //inGameMenu 맵으로 돌아감 
    {
        UIManager.instance.clickSoundGen();        
        Time.timeScale = 1f;

        if(GameManager.instance.gameData.curAchievementNum == 0)
        {
            //opening Elevator 타고 내려가는 도중 게임을 나오면 첫 세이브포인트까지는 활성화한 것으로 침 

            GameManager.instance.gameData.curAchievementNum = 1;
            if (GameManager.instance.gameData.finalStageNum == GameManager.instance.gameData.curStageNum)
            {
                if (GameManager.instance.gameData.finalAchievementNum == 0)
                {
                    GameManager.instance.gameData.finalAchievementNum = 1; //만약 현재 진행도가 최대 진행도일 경우 final Ach 에도 반영함 
                }
            }
            GameManager.instance.gameData.savePointUnlock[GameManager.instance.gameData.curStageNum - 1, 0] = true; //첫번째 세이브포인트 활성화시킴 
            GameManager.instance.gameData.respawnScene = SceneManager.GetActiveScene().buildIndex; //씬은 그대로 유지 

            //데이터 저장
            string ToJsonData = JsonUtility.ToJson(GameManager.instance.gameData);
            string filePath = Application.persistentDataPath + GameManager.instance.gameDataFileNames[GameManager.instance.curSaveFileNum];
            File.WriteAllText(filePath, ToJsonData);

            GameManager.instance.nextScene = GameManager.instance.gameData.respawnScene;
            GameManager.instance.shouldUseOpeningElevator = false;
        }
       
        SceneManager.LoadScene(1);
        Cursor.lockState = CursorLockMode.None;

        gameObject.SetActive(false);
    }
}
