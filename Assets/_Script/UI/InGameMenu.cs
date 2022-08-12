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
        UIManager.instance.clickSoundGen();

        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");

        if (InputManager.instance.isJumpBlocked)
        {
            InputManager.instance.isJumpBlocked = false;
            //jumpBlock은 엘리베이터를 타면 true. 엘리베이터 타고 있는 중간에 메뉴로 나가면 
            // 다음 씬을 시작할 때도 jumpBlock이 해제되지 않아서 점프 불가능한 문제 발생 ~> 직접 꺼서 해결하자 
        }

        gameObject.SetActive(false);
    }

    public void OnClickMainStage() //castleEnterance 맵으로 돌아감 
    {
        UIManager.instance.clickSoundGen();

        Time.timeScale = 1f;

        GameManager.instance.gameData.curAchievementNum = 0;
        GameManager.instance.gameData.curStageNum = 0;
        GameManager.instance.nextScene = 1;
        GameManager.instance.nextPos = boatPos;
        GameManager.instance.nextGravityDir = Vector2.down;

        
        GameManager.instance.shouldStartAtSavePoint = false;
        GameManager.instance.nextState = Player.States.Walk;

        GameManager.instance.gameData.respawnScene = GameManager.instance.nextScene;
        GameManager.instance.gameData.respawnPos = GameManager.instance.nextPos;
        GameManager.instance.gameData.respawnGravityDir = GameManager.instance.nextGravityDir;

        Cursor.lockState = CursorLockMode.Locked;
        SceneManager.LoadScene(GameManager.instance.nextScene);

        if (InputManager.instance.isJumpBlocked)
        {
            InputManager.instance.isJumpBlocked = false;
            //jumpBlock은 엘리베이터를 타면 true. 엘리베이터 타고 있는 중간에 메뉴로 나가면 
            // 다음 씬을 시작할 때도 jumpBlock이 해제되지 않아서 점프 불가능한 문제 발생 ~> 직접 꺼서 해결하자 
        }
        gameObject.SetActive(false);
    }
}
