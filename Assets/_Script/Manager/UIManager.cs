using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] GameObject inGameMenu;
    [SerializeField] GameObject existSavedGame;
    [SerializeField] GameObject noSavedGame;
    [SerializeField] Image fade;
    IEnumerator fadeCoroutine;

    [SerializeField] float fadeDelay; //화면 밝아지거나 어두워지는데 걸리는 시간 

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        fadeCoroutine = _FadeIn();

        //게임 시작하면 UI 메뉴는 전부 끄고 시작하기 
        inGameMenu.SetActive(false);
        existSavedGame.SetActive(false);
        noSavedGame.SetActive(false);
    }

    // Fade in과 Fade out이 동시에 실행될 수 없게 하였음
    public void FadeIn()
    {
        StopCoroutine(fadeCoroutine);
        fadeCoroutine = _FadeIn();
        StartCoroutine(fadeCoroutine);
    }

    public void FadeOut()
    {
        StopCoroutine(fadeCoroutine);
        fadeCoroutine = _FadeOut();
        StartCoroutine(fadeCoroutine);
    }

    IEnumerator _FadeIn()
    {
        var wait = new WaitForSeconds(fadeDelay/20);
        Color color = fade.color;
        color.a = 1f;
        fade.color = color; 
        while (color.a > 0f)
        {
            color.a -= 0.05f;
            fade.color = color;
            yield return wait;
        }
    }

    IEnumerator _FadeOut()
    {
        InputManager.instance.isInputBlocked = true;

        var wait = new WaitForSeconds(0.1f);
        while (fade.color.a < 1f)
        {
            Color color = fade.color;
            color.a += 0.05f;
            fade.color = color;
            yield return wait;
        }

        GameManager.instance.StartGame(false);
    }

    public void OnOffInGameMenu() //메뉴창이 켜져있으면 끄고 꺼져있으면 킨다 
    {
        inGameMenu.SetActive(!inGameMenu.activeSelf);
        // 인게임 메뉴 팝업창 활성화 시 일시정지
        if (inGameMenu.activeSelf)
        {
            Time.timeScale = 0f; //시간 정지
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None; 
        }
        // 인게임 메뉴 팝업창 비활성화 시 다시 재생
        else
        {
            Time.timeScale = 1f;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked; //게임 도중에는 마우스 조작 불가능 
        }
    }

    // New Game 버튼을 눌렀는데 이미 저장된 게임이 있을 경우 팝업창 활성화
    public void ExistSavedGame()
    {
        existSavedGame.SetActive(true);
    }

    // Load Game 버튼을 눌렀는데 저장된 게임이 없을 경우 팝업창 활성화
    public void NoSavedGame()
    {
        noSavedGame.SetActive(true);
    }
}
