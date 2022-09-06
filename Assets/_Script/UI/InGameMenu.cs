using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class InGameMenu : MonoBehaviour
{
    [SerializeField] Vector2 boatPos; //�÷��̾ �����ž� �ϴ� �� ���� ��ǥ 

    // �Ͻ����� ���� �� ���θ޴���
    public void OnClickExit()
    {
        UIManager.instance.clickSoundGen();

        Time.timeScale = 1f;

        if (GameManager.instance.gameData.curAchievementNum == 0)
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

        SceneManager.LoadScene("MainMenu");

        if (InputManager.instance.isJumpBlocked)
        {
            InputManager.instance.isJumpBlocked = false;
            //jumpBlock�� ���������͸� Ÿ�� true. ���������� Ÿ�� �ִ� �߰��� �޴��� ������ 
            // ���� ���� ������ ���� jumpBlock�� �������� �ʾƼ� ���� �Ұ����� ���� �߻� ~> ���� ���� �ذ����� 
        }

        gameObject.SetActive(false);
    }

    public void OnClickMainStage() //inGameMenu ������ ���ư� 
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
       
        SceneManager.LoadScene(1);
        Cursor.lockState = CursorLockMode.None;

        gameObject.SetActive(false);
    }
}
