using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameMenu : MonoBehaviour
{
    // �Ͻ����� ���� �� ���θ޴���
    public void OnClickExit()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
        gameObject.SetActive(false);
    }
}
