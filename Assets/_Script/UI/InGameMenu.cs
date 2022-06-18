using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameMenu : MonoBehaviour
{
    [SerializeField] Vector2 boatPos; //�÷��̾ �����ž� �ϴ� �� ���� ��ǥ 

    // �Ͻ����� ���� �� ���θ޴���
    public void OnClickExit()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
        gameObject.SetActive(false);
    }

    public void OnClickMainStage() //castleEnterance ������ ���ư� 
    {
        Time.timeScale = 1f;

        GameManager.instance.gameData.curAchievementNum = 0;
        GameManager.instance.gameData.curStageNum = 0;
        GameManager.instance.nextScene = 1;
        GameManager.instance.nextPos = boatPos;
        GameManager.instance.nextGravityDir = Vector2.down;

        GameManager.instance.isCliffChecked = false;
        for (int i = 0; i < 35; i++)
        {
            GameManager.instance.curIsShaked[i] = false;
            GameManager.instance.gameData.storedIsShaked[i] = false;
        }
        GameManager.instance.shouldStartAtSavePoint = false;
        GameManager.instance.nextState = Player.States.Walk;

        GameManager.instance.gameData.respawnScene = GameManager.instance.nextScene;
        GameManager.instance.gameData.respawnPos = GameManager.instance.nextPos;
        GameManager.instance.gameData.respawnGravityDir = GameManager.instance.nextGravityDir;

        Cursor.lockState = CursorLockMode.Locked;
        SceneManager.LoadScene(GameManager.instance.nextScene);
    }
}
