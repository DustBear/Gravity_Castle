using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{
    public void OnClickStartGame() {
        GameManager.instance.Init();
        Cursor.lockState = CursorLockMode.Locked;
        SceneManager.LoadScene(1);
    }

    public void OnClickTutorial() {
        Debug.Log("Start Tutorial!");
    }

    public void OnClickOption() {
        Debug.Log("Start Option!");
    }

    public void OnClickExit() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
