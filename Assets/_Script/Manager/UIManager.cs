using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : Singleton<UIManager>
{
    public GameObject inGameMenu;
    public GameObject optionMenu;
    public GameObject fadeObj;
    public Image fade;

    public GameObject collectionMenu;
    public GameObject collectionMenu_fade;
    public GameObject col_alarm;

    string col_getText = "탐험가 상자를 확보했습니다.\n가까운 영혼 비석에서 안전하게 보관할 수 있습니다." ;
    string col_saveText = "수집한 탐험가 상자를 영혼 비석에 저장했습니다.";

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
        optionMenu.SetActive(false);

        //탐험가상자 수집 메뉴는 켜 놔야 함. 단, 어두운 배경은 없애서 보이지 않게 만들기 
        collectionMenu.SetActive(true);
        collectionMenu_fade.SetActive(false);
        col_alarm.GetComponent<Text>().color = new Color(1, 1, 1, 0); //알람 텍스트 투명화 
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

    float fadeTimer;
    IEnumerator _FadeIn(float delayTime) //밝아짐 ~> 알파가 낮아져야 함 
    {
        fade.color = new Color(0, 0, 0, 1);

        while(fade.color.a > 0f)
        {
            float alphaValue = fade.color.a;

            alphaValue -= Time.deltaTime / delayTime;
            fade.color = new Color(0, 0, 0, alphaValue);
            yield return null;
        }

        fade.color = new Color(0, 0, 0, 0);
    }

    IEnumerator _FadeOut(float delayTime) //어두워짐 ~> 알파가 높아져야 함 
    {
        while (fade.color.a < 1f)
        {
            float alphaValue = fade.color.a;

            alphaValue += Time.deltaTime / delayTime;
            fade.color = new Color(0, 0, 0, alphaValue);
            yield return null;
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
            inGameMenu.GetComponent<InGameMenu>().collectionIconDel(); //수집요소 아이콘 지우기 
        }
    }

    public void collectionAlarm(int type)
    {
        switch (type)
        {
            case 1: //탐험가상자를 처음 수집했을 때 
                StopCoroutine("collectionGet");
                StopCoroutine("collectionSave");

                StartCoroutine(collectionGet());
                break;

            case 2: //탐험가상자를 저장했을 때 
                StopCoroutine("collectionGet");
                StopCoroutine("collectionSave");

                StartCoroutine(collectionSave());
                break;
        }
    }

    public IEnumerator collectionGet() //탐험가상자를 처음 수집했을 때 노출 
    {
        Text alarmText = col_alarm.GetComponent<Text>();
        col_alarm.GetComponent<Text>().color = new Color(1, 1, 1, 0);

        float alphaTimer = 0f;

        alarmText.text = col_getText;
        
        while (alarmText.color.a <= 1)
        {
            alarmText.color = new Color(1, 1, 1, alphaTimer/0.5f);
            alphaTimer += Time.deltaTime;
            yield return null;
        }
        alarmText.color = new Color(1, 1, 1, 1); //0.5초에 걸쳐 알람 밝아짐 

        alphaTimer = 0.5f;
        yield return new WaitForSeconds(4f);

        while (alarmText.color.a >= 0 )
        {
            alarmText.color = new Color(1, 1, 1, alphaTimer / 0.5f);
            alphaTimer -= Time.deltaTime;
            yield return null;
        }
        alarmText.color = new Color(1, 1, 1, 0); //0.5초에 걸쳐 알람 어두워짐  
    }

    public IEnumerator collectionSave() //탐험가상자를 세이브에서 저장했을 때 노출 
    {
        Text alarmText = col_alarm.GetComponent<Text>();
        col_alarm.GetComponent<Text>().color = new Color(1, 1, 1, 0);

        float alphaTimer = 0f;

        alarmText.text = col_saveText;

        while (alarmText.color.a <= 1)
        {
            alarmText.color = new Color(1, 1, 1, alphaTimer / 0.5f);
            alphaTimer += Time.deltaTime;
            yield return null;
        }
        alarmText.color = new Color(1, 1, 1, 1); //0.5초에 걸쳐 알람 밝아짐 

        alphaTimer = 0.5f;
        yield return new WaitForSeconds(4f);

        while (alarmText.color.a >= 0)
        {
            alarmText.color = new Color(1, 1, 1, alphaTimer / 0.5f);
            alphaTimer -= Time.deltaTime;
            yield return null;
        }
        alarmText.color = new Color(1, 1, 1, 0); //0.5초에 걸쳐 알람 어두워짐  
    }

    public void clickSoundGen() //UI 클릭할 때 딸깍 소리 냄 
    {
        sound.Play();
    }

    public void cameraShake(float size, float length)
    {
        GameObject.Find("Main Camera").GetComponent<MainCamera>().cameraShake(size, length);
    }

    public void OnClickOptionMenu() //옵션 창 켜기 버튼을 누를 때 
    {
        clickSoundGen();
        optionMenu.SetActive(true);
    }

    public void optionDisable()
    {
        clickSoundGen();
        optionMenu.SetActive(false);
    }
}

