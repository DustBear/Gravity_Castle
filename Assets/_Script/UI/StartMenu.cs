using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class StartMenu : MonoBehaviour
{
    public GameObject titleMenu;

    public GameObject gameMenu; //시작하기, 이어하기, 불러오기 버튼 있는 창 
    public GameObject loadMenu; //세이브파일 버튼 1~4 뜨는 것 
    
    public TextMeshProUGUI settingExp;

    public GameObject titleImage; //배경 영상 
    public GameObject gameMenuText; //'아무 버튼이나 눌러 시작하세요' 버튼 ~> 깜빡여야 함 

    public bool isTitleMenuOpen;
    public bool isGameMenuOpen;


    private void Start()
    {
        StartCoroutine(startFade());

        isTitleMenuOpen = true;
        titleMenu.SetActive(true); //시작하면 titleMenu 에서 시작함 

        //gameMenu, loadMenu 끔
        isGameMenuOpen = false;
        gameMenu.SetActive(false);
        loadMenu.SetActive(false);

        InputManager.instance.isInputBlocked = false; //메인 메뉴로 돌아오면 무조건 inputBlock 풀어줌
        
        StartCoroutine("blink");
    }

    IEnumerator startFade()
    {
        UIManager.instance.fade.color = new Color(0, 0, 0, 1);
        yield return new WaitForSeconds(0.3f);
        UIManager.instance.FadeIn(1.5f);
    }

    private void Update()
    {
        if (Input.anyKeyDown && isTitleMenuOpen) //타이틀화면이 열려 있는 상태에서 아무 키나 누르면 
        {
            StartCoroutine("gameMenuOpen"); //게임메뉴 키기 
        }  
    }

    IEnumerator gameMenuOpen() //메뉴 열고 닫는 중간에 페이드인/아웃 효과 있어야 함 
    {
        isTitleMenuOpen = false;
        UIManager.instance.FadeOut(1.5f);

        yield return new WaitForSeconds(2f); //화면이 완전히 어두워지면 

        //타이틀화면 끄고 
        titleMenu.SetActive(false);

        //gameMenu 키고 
        isGameMenuOpen = true;
        gameMenu.SetActive(true);

        UIManager.instance.FadeIn(1.5f);
    }

    IEnumerator blink() //시작 버튼 깜빡이게 만듦
    {
        float cosTimer = 0;
        float blinkPeriod = 0.6f;
        while (true)
        {
            gameMenuText.GetComponent<Text>().color = new Color(1, 1, 1, Mathf.Cos(cosTimer/blinkPeriod)/2 + 0.5f);
            cosTimer += 0.01f;
            yield return new WaitForSeconds(0.01f);

            if (!isTitleMenuOpen)
            {
                yield break;
            }
        }      
    }

    public void optionMenuOpen() 
    {
        UIManager.instance.clickSoundGen();

        //옵션 창 켬 
        UIManager.instance.optionMenu.SetActive(true);
    }

    void settingMessageOff()
    {
        settingExp.gameObject.SetActive(false);
    }

    // 게임 종료 시 Editor 상에서의 종료인지, 빌드 파일 상에서의 종료인지 구분
    public void OnClickExit()
    {
        UIManager.instance.clickSoundGen();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
