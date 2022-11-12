using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : Singleton<UIManager>
{
    public GameObject inGameMenu;
    public GameObject fadeObj;
    public Image fade;

    IEnumerator fadeCoroutine;

    AudioSource sound;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        sound = GetComponent<AudioSource>();

        fade = fadeObj.GetComponent<Image>();

        fadeCoroutine = _FadeIn(1f);
        fadeObj.SetActive(true);
        fade.color = new Color(0, 0, 0, 0); //맨 처음 시작하면 fade는 투명화 

        //게임 시작하면 UI 메뉴는 전부 끄고 시작하기 
        inGameMenu.SetActive(false);       
    }

    private void Update()
    {
        
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

    IEnumerator _FadeIn(float delayTime) //화면 밝아짐 
    {
        var wait = new WaitForSeconds(delayTime / 50f);

        while (fade.color.a > 0f)
        {
            fade.color = new Color(0, 0, 0, fade.color.a - 0.02f);
            yield return wait;
        }
        fade.color = new Color(0, 0, 0, 0);
    }

    IEnumerator _FadeOut(float delayTime) //화면 어두워짐 
    {
        var wait = new WaitForSeconds(delayTime/50f);

        while (fade.color.a < 1f)
        {
            fade.color = new Color(0, 0, 0, fade.color.a + 0.02f);
            yield return wait;
        }
        fade.color = new Color(0, 0, 0, 1);
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

    public void clickSoundGen() //UI 클릭할 때 딸깍 소리 냄 
    {
        sound.Play();
    }

    public void cameraShake()
    {

    }
}
