using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] GameObject inGameMenu;
    [SerializeField] GameObject existSavedGame;
    [SerializeField] GameObject noSavedGame;
    [SerializeField] Image fade;
    [SerializeField] GameObject textObj;
    IEnumerator fadeCoroutine;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        fadeCoroutine = _FadeIn(1f);

        //게임 시작하면 UI 메뉴는 전부 끄고 시작하기 
        inGameMenu.SetActive(false);
        existSavedGame.SetActive(false);
        noSavedGame.SetActive(false);
    }

    // Fade in과 Fade out이 동시에 실행될 수 없게 하였음
    public void FadeIn(float fadeTime)
    {
        StopCoroutine(fadeCoroutine);
        fadeCoroutine = _FadeIn(fadeTime);
        StartCoroutine(fadeCoroutine);
    }

    public void FadeOut(float fadeTime)
    {
        StopCoroutine(fadeCoroutine);
        fadeCoroutine = _FadeOut(fadeTime);
        StartCoroutine(fadeCoroutine);
    }

    IEnumerator _FadeIn(float delayTime)
    {
        var wait = new WaitForSeconds(delayTime/50);
        Color color = fade.color;
        color.a = 1f;
        fade.color = color; 
        while (color.a > 0f)
        {
            color.a -= 0.02f;
            fade.color = color;
            yield return wait;
        }
    }

    IEnumerator _FadeOut(float delayTime)
    {
        var wait = new WaitForSeconds(delayTime/50);
        Color color = fade.color;
        while (fade.color.a < 1f)
        {           
            color.a += 0.02f;
            fade.color = color;
            yield return wait;
        }
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

    public void showText(string content)
    {
        textObj.SetActive(true);
        textObj.GetComponent<TextMeshProUGUI>().text = content;
    }

    public void hideText()
    {
        textObj.SetActive(false);
    }
}
