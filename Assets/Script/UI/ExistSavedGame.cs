using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExistSavedGame : MonoBehaviour
{
    // Yes 버튼 클릭 -> 새 게임 시작
    public void OnClickYes()
    {
        GameManager.instance.StartGame(true);
        gameObject.SetActive(false);
    }

    // No 버튼 클릭 -> 다시 메인메뉴로
    public void OnClickNo()
    {
        gameObject.SetActive(false);
    }
}
