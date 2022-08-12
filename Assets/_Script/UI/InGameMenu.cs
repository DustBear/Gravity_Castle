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
        UIManager.instance.clickSoundGen();

        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");

        if (InputManager.instance.isJumpBlocked)
        {
            InputManager.instance.isJumpBlocked = false;
            //jumpBlock�� ���������͸� Ÿ�� true. ���������� Ÿ�� �ִ� �߰��� �޴��� ������ 
            // ���� ���� ������ ���� jumpBlock�� �������� �ʾƼ� ���� �Ұ����� ���� �߻� ~> ���� ���� �ذ����� 
        }

        gameObject.SetActive(false);
    }

    public void OnClickMainStage() //castleEnterance ������ ���ư� 
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
            //jumpBlock�� ���������͸� Ÿ�� true. ���������� Ÿ�� �ִ� �߰��� �޴��� ������ 
            // ���� ���� ������ ���� jumpBlock�� �������� �ʾƼ� ���� �Ұ����� ���� �߻� ~> ���� ���� �ذ����� 
        }
        gameObject.SetActive(false);
    }
}
