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
    [HideInInspector] public bool isJumpBlocked { get; set; } 
    //엘리베이터 탈 때는 점프 불가능하게 만듦. 
    //현재 player 알고리즘 상으로는 플레이어가 착지할 때 y축 속도가 음수여야 착지한 것으로 인지 
    //엘리베이터를 타고 올라가는 도중 점프하고 착지하면 y축 글로벌 좌표 속도가 양수이므로 착지 인지 못함 
    //단기간에 해결하기 힘든 문제로 보이므로 그냥 엘리베이터를 타는 동안은 점프가 불가능하도록 만들자. 

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

            //vertical은 로프 알고리즘에서 사용. jump는 점프 알고리즘에서 사용 
            if (!isJumpBlocked)
            {
                jumpDown = Input.GetKeyDown(KeyCode.Space);
                jump = Input.GetKey(KeyCode.Space);
                jumpUp = Input.GetKeyUp(KeyCode.Space);
            }
            else
            {
                jumpDown = false;
                jump = false;
                jumpUp = false;
            }       
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
        
        // 게임 진행중에 esc 누르면 메뉴 온오프 가능
        esc = Input.GetButtonDown("Cancel");
        if (esc && SceneManager.GetActiveScene().name != "MainMenu") //메인메뉴에선 ESC 누를 수 없음 
        {
            UIManager.instance.OnOffInGameMenu();
        }
    }
}
