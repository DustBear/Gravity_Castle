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
    //���������� Ż ���� ���� �Ұ����ϰ� ����. 
    //���� player �˰���� �����δ� �÷��̾ ������ �� y�� �ӵ��� �������� ������ ������ ���� 
    //���������͸� Ÿ�� �ö󰡴� ���� �����ϰ� �����ϸ� y�� �۷ι� ��ǥ �ӵ��� ����̹Ƿ� ���� ���� ���� 
    //�ܱⰣ�� �ذ��ϱ� ���� ������ ���̹Ƿ� �׳� ���������͸� Ÿ�� ������ ������ �Ұ����ϵ��� ������. 

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {        
        if (!isInputBlocked) //InputBlocked �Ǹ� ���� �Ұ���
        {
            horizontal = Input.GetAxisRaw("Horizontal");
            horizontalDown = Input.GetButtonDown("Horizontal");
            vertical = Input.GetAxisRaw("Vertical");
            verticalDown = Input.GetButtonDown("Vertical");

            //vertical�� ���� �˰���򿡼� ���. jump�� ���� �˰���򿡼� ��� 
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
        
        // ���� �����߿� esc ������ �޴� �¿��� ����
        esc = Input.GetButtonDown("Cancel");

        if (esc && SceneManager.GetActiveScene().name != "MainMenu" 
            && SceneManager.GetActiveScene().name != "openingScene"
            && SceneManager.GetActiveScene().name != "InGameMenu") //esc 를 눌러 mainMenu 씬을 열 수 있는 조건 
        {
            UIManager.instance.OnOffInGameMenu();
        }
    }
}
