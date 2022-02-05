using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : Singleton<InputManager>
{
    protected InputManager() {}

    [HideInInspector] public float horizontal;
    [HideInInspector] public bool horizontalDown;
    [HideInInspector] public float vertical;
    [HideInInspector] public bool verticalDown;
    [HideInInspector] public bool jump;
    [HideInInspector] public bool esc;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        horizontalDown = Input.GetButtonDown("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
        verticalDown = Input.GetButtonDown("Vertical");
        jump = Input.GetKeyDown(KeyCode.Space);
        esc = Input.GetButtonDown("Cancel");

        if (esc && GameManager.instance.curAchievementNum >= 0)
        {
            UIManager.instance.OnOffInGameMenu();
        }
    }
}
