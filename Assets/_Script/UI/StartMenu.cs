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
    public GameObject fadeCover;
    public GameObject gameMenuText; //'아무 버튼이나 눌러 시작하세요' 버튼 ~> 깜빡여야 함 

    Color fadeCoverColor;

    public bool isTitleMenuOpen;
    public bool isGameMenuOpen;
    private void Start()
    {
        isTitleMenuOpen = true;
        titleMenu.SetActive(true); //시작하면 titleMenu 에서 시작함 

        //gameMenu, loadMenu 끔 
        isGameMenuOpen = false;
        gameMenu.SetActive(false);
      
        loadMenu.SetActive(false);


        InputManager.instance.isInputBlocked = false; //메인 메뉴로 돌아오면 무조건 inputBlock 풀어줌
        
        StartCoroutine("blink");
    }

    private void Update()
    {
        if (Input.anyKeyDown && isTitleMenuOpen) //타이틀화면이 열려 있는 상태에서 아무 키나 누르면 
        {
            //타이틀화면 끄고 
            titleMenu.SetActive(false);
            isTitleMenuOpen = false;

            StartCoroutine("gameMenuOpen"); //게임메뉴 키기 
        }  
    }

    IEnumerator gameMenuOpen() //메뉴 열고 닫는 중간에 페이드인/아웃 효과 있어야 함 
    {
        fadeCover.GetComponent<Image>().color = new Color(0, 0, 0, 1); //처음에는 검은 화면에서 시작

        //gameMenu 키고 
        isGameMenuOpen = true;
        gameMenu.SetActive(true);

        //페이드인
        float fadeTime = 2f; //페이드인이 완료되는 데 걸리는 시간 

        for(int index=100; index>=1; index--)
        {
            fadeCover.GetComponent<Image>().color = new Color(0, 0, 0, 0.01f*index);
            yield return new WaitForSeconds(0.01f * fadeTime);
        }

        fadeCover.GetComponent<Image>().color = new Color(0, 0, 0, 0);
    }

    IEnumerator blink() //시작 버튼 깜빡이게 만듦
    {       
        while (true)
        {
            gameMenuText.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            gameMenuText.SetActive(false);
            yield return new WaitForSeconds(0.5f);

            if (!isTitleMenuOpen)
            {
                yield break;
            }
        }      
    }

 
    public void OnClickSetting()
    {
        UIManager.instance.clickSoundGen();

        settingExp.gameObject.SetActive(true);
        Invoke("settingMessageOff", 3f); //3초 후 세팅메시지 꺼짐

        //Debug.Log("Start Setting!");
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
