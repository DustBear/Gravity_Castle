using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameMenu : MonoBehaviour
{

    public void OnClickContinue() {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        gameObject.SetActive(false);
    }

    public void OnClickSave() {
    }

    public void OnClickExit() {
        Time.timeScale = 1f;
        GameManager.instance.curAchievementNum = -1;
        SceneManager.LoadScene(0);
        gameObject.SetActive(false);
    }
}
