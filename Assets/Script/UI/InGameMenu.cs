using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameMenu : MonoBehaviour
{
    public void OnClickExit() {
        DataManager.instance.SaveData();
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
        gameObject.SetActive(false);
    }
}
