using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InputManager : Singleton<InputManager>
{
    [HideInInspector] public float horizontal {get; private set;}
    [HideInInspector] public bool horizontalDown {get; private set;}
    [HideInInspector] public float vertical {get; private set;}
    [HideInInspector] public bool verticalDown {get; private set;}
    [HideInInspector] public bool jumpDown { get; private set; }
    [HideInInspector] public bool jump {get; private set;}
    [HideInInspector] public bool jumpUp {get; private set;}
    [HideInInspector] public bool esc {get; private set;}

    [HideInInspector] public bool isInputBlocked {get; set;}


    public bool isPlayerDying= false;
    //플레이어가 죽는 동작 중 메인메뉴로 나가버리면 에러 발생
    void Awake()
    {
        DontDestroyOnLoad(gameObject);       
    }

    void Update()
    {        
        if (!isInputBlocked) //InputBlocked 상태에서는 플레이어조작 금지 
        {
            horizontal = Input.GetAxisRaw("Horizontal");
            horizontalDown = Input.GetButtonDown("Horizontal");
            vertical = Input.GetAxisRaw("Vertical");
            verticalDown = Input.GetButtonDown("Vertical");

            jumpDown = Input.GetKeyDown(KeyCode.Space);
            jump = Input.GetKey(KeyCode.Space);
            jumpUp = Input.GetKeyUp(KeyCode.Space);
        }
        else
        {
            horizontal = 0f;
            horizontalDown = false;
            vertical = 0f;
            verticalDown = false;
            jumpDown = false;
            jump = false;
            jumpUp = false;
        }
        
        // 메인메뉴, 오프닝씬, 인게임메뉴 이외의 창에서 esc 누르면 인게임 메뉴 열어줌 
        esc = Input.GetButtonDown("Cancel");

        //데모버전에서는 scene8에서 게임이 끝나고 엔딩크레딧이 올라옴
        //따라서 scene8에서는 esc 메뉴를 열 수 없음 
        //이후 출시버전에서는 삭제해야 함 

        if (esc && SceneManager.GetActiveScene().name != "MainMenu" 
            && SceneManager.GetActiveScene().name != "openingScene"
            && SceneManager.GetActiveScene().name != "InGameMenu"
            //&& SceneManager.GetActiveScene().buildIndex != 8
            && !UIManager.instance.optionMenu.activeSelf //설정창이 열려 있는 상태에서 뒤에 있는 메뉴창을 닫을 수 없음 
            && !isPlayerDying) //esc 를 눌러 mainMenu 씬을 열 수 있는 조건 
        {
            UIManager.instance.OnOffInGameMenu();
        }

        //&& SceneManager.GetActiveScene().buildIndex != 8 집어넣으면 마지막 씬에서 esc 사용 못하게 됨 
    }
}
