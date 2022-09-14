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
            GameManager.instance.gameData.savePointUnlock[GameManager.instance.gameData.curStageNum - 1, 0] = true; //세이브포인트 1 활성화 
            GameManager.instance.gameData.respawnScene = SceneManager.GetActiveScene().buildIndex; // 

            //������ ����
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
            //jumpBlock�� ���������͸� Ÿ�� true. ���������� Ÿ�� �ִ� �߰��� �޴��� ������ 
            // ���� ���� ������ ���� jumpBlock�� �������� �ʾƼ� ���� �Ұ����� ���� �߻� ~> ���� ���� �ذ����� 
        }

        gameObject.SetActive(false);
    }

    public void OnClickMainStage() //인게임 메뉴로 나가기 버튼 누를 때 
    {
        UIManager.instance.clickSoundGen();        
        Time.timeScale = 1f;

        if(GameManager.instance.gameData.curAchievementNum == 0)
        {
            //opening Elevator Ÿ�� �������� ���� ������ ������ ù ���̺�����Ʈ������ Ȱ��ȭ�� ������ ħ 

            GameManager.instance.gameData.curAchievementNum = 1;
            if (GameManager.instance.gameData.finalStageNum == GameManager.instance.gameData.curStageNum)
            {
                if (GameManager.instance.gameData.finalAchievementNum == 0)
                {
                    GameManager.instance.gameData.finalAchievementNum = 1; //���� ���� ���൵�� �ִ� ���൵�� ��� final Ach ���� �ݿ��� 
                }
            }
            GameManager.instance.gameData.savePointUnlock[GameManager.instance.gameData.curStageNum - 1, 0] = true; //ù��° ���̺�����Ʈ Ȱ��ȭ��Ŵ 
            GameManager.instance.gameData.respawnScene = SceneManager.GetActiveScene().buildIndex; //���� �״�� ���� 

            //������ ����
            string ToJsonData = JsonUtility.ToJson(GameManager.instance.gameData);
            string filePath = Application.persistentDataPath + GameManager.instance.gameDataFileNames[GameManager.instance.curSaveFileNum];
            File.WriteAllText(filePath, ToJsonData);

            GameManager.instance.nextScene = GameManager.instance.gameData.respawnScene;
            GameManager.instance.shouldUseOpeningElevator = false;
        }
       
        SceneManager.LoadScene("InGameMenu");
        Cursor.lockState = CursorLockMode.None;

        gameObject.SetActive(false);
    }
}
