using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{
    public void OnClickNewGame()
    {
        GameManager.instance.CheckSavedGame(false);
    }

    public void OnClickLoadGame()
    {
        GameManager.instance.CheckSavedGame(true);
    }

    public void OnClickTutorial()
    {
        Debug.Log("Start Tutorial!");
    }

    public void OnClickSetting()
    {
        Debug.Log("Start Setting!");
    }

    // 게임 종료 시 Editor 상에서의 종료인지, 빌드 파일 상에서의 종료인지 구분
    public void OnClickExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
