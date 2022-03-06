using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Test : MonoBehaviour
{
    [SerializeField] int curAchievementNum;
    [SerializeField] int nextScene;
    [SerializeField] Vector2 nextPos;
    [SerializeField] Vector2 nextGravityDir;

    public void OnClickButton()
    {
        GameManager.instance.curAchievementNum = curAchievementNum;
        GameManager.instance.nextScene = nextScene;
        GameManager.instance.nextPos = nextPos;
        GameManager.instance.nextGravityDir = nextGravityDir;
        GameManager.instance.isCliffChecked = false;
        for (int i = 0; i < 36; i++)
        {
            GameManager.instance.curIsShaked[i] = false;
            GameManager.instance.storedIsShaked[i] = false;
        }
        GameManager.instance.isDie = false;
        GameManager.instance.isChangeGravityDir = false;
        GameManager.instance.nextRopingState = Player.RopingState.idle;
        GameManager.instance.nextLeveringState = Player.LeveringState.idle;
        GameManager.instance.nextIsJumping = false;
        GameManager.instance.respawnScene = GameManager.instance.nextScene;
        GameManager.instance.respawnPos = GameManager.instance.nextPos;
        GameManager.instance.respawnGravityDir = GameManager.instance.nextGravityDir;
        GameManager.instance.respawnRopingState = GameManager.instance.nextRopingState;

        Cursor.lockState = CursorLockMode.Locked;
        SceneManager.LoadScene(GameManager.instance.nextScene);
    }
}
