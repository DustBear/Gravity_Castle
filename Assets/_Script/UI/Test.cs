using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

// �׽�Ʈ�� ���ϰ� �ϱ� ���� ��ũ��Ʈ
// ���ϴ� ���������� �ٷ� �̵� ����
// ���� ����� ���� ���־��� 
public class Test : MonoBehaviour
{
    [SerializeField] int curStageNum;
    [SerializeField] int nextScene;
    [SerializeField] Vector2 nextPos;
    [SerializeField] Vector2 nextGravityDir;

    public void OnClickButton()
    {
        GameManager.instance.gameData.curAchievementNum = 0;
        GameManager.instance.gameData.curStageNum = curStageNum;
        GameManager.instance.nextScene = nextScene;
        GameManager.instance.nextPos = nextPos;
        GameManager.instance.nextGravityDir = nextGravityDir;
        
        GameManager.instance.shouldSpawnSavePoint = false;
        GameManager.instance.nextState = Player.States.Walk;
        GameManager.instance.gameData.respawnScene = GameManager.instance.nextScene;
        GameManager.instance.gameData.respawnPos = GameManager.instance.nextPos;
        GameManager.instance.gameData.respawnGravityDir = GameManager.instance.nextGravityDir;

        Cursor.lockState = CursorLockMode.Locked;
        SceneManager.LoadScene(GameManager.instance.nextScene);
    }   
}
