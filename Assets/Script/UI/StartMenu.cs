using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{
    public void OnClickNewGame()
    {
        DataManager.instance.CheckSavedGame(true);
    }

    public void OnClickLoadGame()
    {
        DataManager.instance.CheckSavedGame(false);
    }

    public void OnClickTutorial()
    {
        Debug.Log("Start Tutorial!");
    }

    public void OnClickSetting()
    {
        Debug.Log("Start Setting!");
    }

    public void OnClickExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
