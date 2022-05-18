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
    [HideInInspector] public bool esc {get; private set;}

    [HideInInspector] public bool isInputBlocked {get; set;}

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (!isInputBlocked)
        {
            horizontal = Input.GetAxisRaw("Horizontal");
            horizontalDown = Input.GetButtonDown("Horizontal");
            vertical = Input.GetAxisRaw("Vertical");
            verticalDown = Input.GetButtonDown("Vertical");
            jump = Input.GetKeyDown(KeyCode.Space);
        }
        else
        {
            horizontal = 0f;
            horizontalDown = false;
            vertical = 0f;
            verticalDown = false;
            jump = false;
        }
        
        // 게임 진행중에 esc 누르면 메뉴 온오프 가능
        esc = Input.GetButtonDown("Cancel");
        if (esc && SceneManager.GetActiveScene().name != "MainMenu")
        {
            UIManager.instance.OnOffInGameMenu();
        }
    }
}
