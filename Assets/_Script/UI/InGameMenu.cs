using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameMenu : MonoBehaviour
{
    [SerializeField] Vector2 boatPos; //플레이어가 스폰돼야 하는 배 안의 좌표 

    // 일시정지 해제 후 메인메뉴로
    public void OnClickExit()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
        gameObject.SetActive(false);
    }

    public void OnClickMainStage() //castleEnterance 맵으로 돌아감 
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
