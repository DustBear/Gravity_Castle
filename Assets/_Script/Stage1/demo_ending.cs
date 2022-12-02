using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class demo_ending : MonoBehaviour
{
    bool isSceneLoaded = false;

    void Start()
    {
        InputManager.instance.isInputBlocked = true;
        Cursor.lockState = CursorLockMode.None;
    }

    
    void Update()
    {
        
    }

    public void onClick()
    {
        if (isSceneLoaded)
        {
            return;
        }

        isSceneLoaded = true;

        UIManager.instance.FadeOut(1.5f);
        Invoke("loadMainMenu", 2f);
    }

    void loadMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
