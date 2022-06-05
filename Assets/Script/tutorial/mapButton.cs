using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class mapButton : MonoBehaviour
{
    [SerializeField] int curStageNum;
    [SerializeField] int nextScene;
    [SerializeField] int nextCurAchievementNum;
    [SerializeField] Vector2 nextPos;
    [SerializeField] Vector2 nextGravityDir;

    public void OnClickButton()
    {
        GameManager.instance.gameData.curAchievementNum = nextCurAchievementNum;
        GameManager.instance.gameData.curStageNum = curStageNum;
        GameManager.instance.nextScene = nextScene;
        GameManager.instance.nextPos = nextPos;
        GameManager.instance.nextGravityDir = nextGravityDir;

        //stage5
        GameManager.instance.isCliffChecked = false;
        for (int i = 0; i < 36; i++)
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
