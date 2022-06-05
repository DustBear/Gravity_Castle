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
        if (!isInputBlocked) //InputBlocked 되면 조작 불가능
        {
            horizontal = Input.GetAxisRaw("Horizontal");
            horizontalDown = Input.GetButtonDown("Horizontal");
            vertical = Input.GetAxisRaw("Vertical");
            verticalDown = Input.GetButtonDown("Vertical");
            jump = Input.GetKey(KeyCode.Space);
            jumpUp = Input.GetKeyUp(KeyCode.Space);
        }
        else
        {
            horizontal = 0f;
            horizontalDown = false;
            vertical = 0f;
            verticalDown = false;
            jump = false;
            jumpUp = false;
        }
        
        // 게임 진행중에 esc 누르면 메뉴 온오프 가능
        esc = Input.GetButtonDown("Cancel");
        if (esc && SceneManager.GetActiveScene().name != "MainMenu") //메인메뉴에선 ESC 누를 수 없음 
        {
            UIManager.instance.OnOffInGameMenu();
        }
    }
}
