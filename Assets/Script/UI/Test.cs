using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// �׽�Ʈ�� ���ϰ� �ϱ� ���� ��ũ��Ʈ
// ���ϴ� ���������� �ٷ� �̵� ����
// ���� ����� ���� ���־��� 
public class Test : MonoBehaviour
{
    [SerializeField] int curAchievementNum;
    [SerializeField] int nextScene;
    [SerializeField] Vector2 nextPos;
    [SerializeField] Vector2 nextGravityDir;

    public void OnClickButton()
    {
        GameManager.instance.gameData.curAchievementNum = curAchievementNum;
        GameManager.instance.nextScene = nextScene;
        GameManager.instance.nextPos = nextPos;
        GameManager.instance.nextGravityDir = nextGravityDir;
        GameManager.instance.isCliffChecked = false;
        for (int i = 0; i < 36; i++)
        {
            GameManager.instance.curIsShaked[i] = false;
            GameManager.instance.gameData.storedIsShaked[i] = false;
        }
        GameManager.instance.shouldStartAtSavePoint = false;
        GameManager.instance.isChangeGravityDir = false;
        GameManager.instance.nextRopingState = Player.RopingState.idle;
        GameManager.instance.nextLeveringState = Player.LeveringState.idle;
        GameManager.instance.nextIsJumping = false;
        GameManager.instance.gameData.respawnScene = GameManager.instance.nextScene;
        GameManager.instance.gameData.respawnPos = GameManager.instance.nextPos;
        GameManager.instance.gameData.respawnGravityDir = GameManager.instance.nextGravityDir;

        Cursor.lockState = CursorLockMode.Locked;
        SceneManager.LoadScene(GameManager.instance.nextScene);
    }
}
