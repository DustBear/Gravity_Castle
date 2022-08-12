using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class StartMenu : MonoBehaviour
{
    public GameObject titleMenu;

    public GameObject gameMenu;
    //public Button[] gameMenuButton = new Button[4];
    public TextMeshProUGUI[] gameMenuButtonText = new TextMeshProUGUI[4];
    public Button[] gameMenuButton = new Button[4];
    public TextMeshProUGUI settingExp;

    public GameObject quickMenu;
    public GameObject titleImage; //배경 영상 
    public GameObject fadeCover;
    public float fadeSpeed;
    public GameObject gameMenuText; //'아무 버튼이나 눌러 시작하세요' 버튼 ~> 깜빡여야 함 

    Color fadeCoverColor;

    public bool isTitleMenuOpen;
    public bool isGameMenuOpen;
    public bool isQuickMenuOpen;
    private void Start()
    {
        isTitleMenuOpen = true;
        titleMenu.SetActive(true);
        isGameMenuOpen = false;
        gameMenu.SetActive(false);
        isQuickMenuOpen = false;
        quickMenu.SetActive(false);
        InputManager.instance.isInputBlocked = false; //메인 메뉴로 돌아오면 무조건 inputBlock 풀어줌

        fadeCoverColor = fadeCover.GetComponent<Image>().color;
        fadeCoverColor = new Color(0, 0, 0, 0); //처음에는 투명
        
        for (int index = 0; index <= 3; index++)
        {
            gameMenuButtonText[index].color = new Color(1, 1, 1, 0);
        }
        for (int index = 0; index <= 3; index++)
        {
            gameMenuButton[index].GetComponent<Image>().color = new Color(1, 1, 1, 0);
        }
        StartCoroutine("blink");
    }

    private void Update()
    {
        if (Input.anyKeyDown && isTitleMenuOpen)
        {
            titleMenu.SetActive(false);
            isTitleMenuOpen = false;
            StartCoroutine("gameMenuOpen");
        }

        if (Input.GetKeyDown(KeyCode.Escape) && isQuickMenuOpen)
        {
            quickMenu.SetActive(false);
            isQuickMenuOpen = false;
        }
    }

    IEnumerator gameMenuOpen() //메뉴 열고 닫는 중간에 페이드인/아웃 효과 있어야 함 
    {
        isGameMenuOpen = true;
        gameMenu.SetActive(true);
 
        //페이드인
        while (fadeCoverColor.a <= 1) //투명도가 1이 될 때 까지 계속 투명도 높임 
        {
            fadeCoverColor = new Color(0, 0, 0, fadeCoverColor.a + fadeSpeed);
            for(int index=0; index <=3; index++)
            {
                gameMenuButtonText[index].color = new Color(1, 1, 1, gameMenuButtonText[index].color.a + fadeSpeed);
            }
            for (int index = 0; index <= 3; index++)
            {
                gameMenuButton[index].GetComponent<Image>().color = new Color(1, 1, 1, gameMenuButton[index].GetComponent<Image>().color.a + fadeSpeed);
            }
            yield return new WaitForFixedUpdate();
        }              
    }

    IEnumerator blink() //시작 버튼 깜빡이게 만듦
    {
        if (isGameMenuOpen)
        {
            yield return null;
        }
        while (true)
        {
            gameMenuText.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            gameMenuText.SetActive(false);
            yield return new WaitForSeconds(0.5f);
        }      
    }

    public void openQuickMenu()
    {
        if (isQuickMenuOpen)
        {
            quickMenu.SetActive(false);
            isQuickMenuOpen = false;
        }
        else if (!isQuickMenuOpen)
        {
            quickMenu.SetActive(true);
            isQuickMenuOpen = true;
        }
    }

    public void newStart() //스테이지1 이전에 튜토리얼이 삽입된 버전 
    {
        SceneManager.LoadScene(24); //튜토리얼은 24번째 씬 
    }

    public void OnClickTutorial()
    {
        Debug.Log("Start Tutorial!");
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
