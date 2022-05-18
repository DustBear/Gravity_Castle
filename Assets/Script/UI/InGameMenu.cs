using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameMenu : MonoBehaviour
{
    // 일시정지 해제 후 메인메뉴로
    public void OnClickExit()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
        gameObject.SetActive(false);
    }
}
