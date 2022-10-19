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

        if (esc && SceneManager.GetActiveScene().name != "MainMenu" 
            && SceneManager.GetActiveScene().name != "openingScene"
            && SceneManager.GetActiveScene().name != "InGameMenu") //esc 를 눌러 mainMenu 씬을 열 수 있는 조건 
        {
            UIManager.instance.OnOffInGameMenu();
        }
    }
}
