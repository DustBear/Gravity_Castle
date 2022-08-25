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

    /*
    public GameObject fadeCircle;
    public Vector3 playerPos;   
    public float fadeCircleDelay;
    */
    AudioSource sound;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        sound = GetComponent<AudioSource>();

        fadeCoroutine = _FadeIn(1.5f);

        //게임 시작하면 UI 메뉴는 전부 끄고 시작하기 
        inGameMenu.SetActive(false);
        existSavedGame.SetActive(false);
        noSavedGame.SetActive(false);

        //fadeCircle.SetActive(false);
    }

    private void Update()
    {
        
    }
    /*
    public void circleFade(bool bigger)
    {
        fadeCircle.SetActive(true);
        StartCoroutine(circleFadeCor(bigger));
    }
    IEnumerator circleFadeCor(bool bigger)
    {       
        if (bigger) //circle이 커져야 하는 경우 ~> fadeOut 의 기능(어두워짐)
        {           
            fadeCircle.SetActive(true);
            fadeCircle.GetComponent<RectTransform>().localScale = new Vector3(0,0,0);

            Vector2 circleAimPos = Camera.main.WorldToScreenPoint(playerPos);
            fadeCircle.GetComponent<RectTransform>().position = new Vector3(circleAimPos.x, circleAimPos.y, 5f);

            for (int index = 1; index <= 200; index++)
            {
                fadeCircle.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1) * (index / 200);
                yield return new WaitForSeconds(fadeCircleDelay / 200);
            }
        }
        else
        {
            fadeCircle.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);

            Vector2 circleAimPos = Camera.main.WorldToScreenPoint(playerPos);
            Debug.Log(circleAimPos);
            fadeCircle.GetComponent<RectTransform>().position = new Vector3(circleAimPos.x, circleAimPos.y, 5f);

            for (int index = 200; index >= 1; index--)
            {
                Debug.Log(fadeCircle.GetComponent<RectTransform>().localScale);
                fadeCircle.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1) * (index / 200);
                yield return new WaitForSeconds(fadeCircleDelay / 200);
            }

            fadeCircle.SetActive(false);
        }      
    }
    */

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
        var wait = new WaitForSeconds(delayTime/200);
        Color color = fade.color;
        color.a = 1f;
        fade.color = color;

        yield return new WaitForSeconds(0.3f);
        while (color.a > 0f)
        {
            color.a -= 0.005f;
            fade.color = color;
            yield return wait;
        }
    }

    IEnumerator _FadeOut(float delayTime) //화면 어두워짐 
    {        
        var wait = new WaitForSeconds(delayTime/200);
        Color color = fade.color;
        while (fade.color.a < 1f)
        {           
            color.a += 0.005f;
            fade.color = color;
            yield return wait;
        }

        yield return new WaitForSeconds(1f);
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

    public void clickSoundGen() //UI 클릭할 때 딸깍 소리 냄 
    {
        sound.Play();
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
