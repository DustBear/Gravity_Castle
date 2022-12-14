using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

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
        if(Cursor.lockState != CursorLockMode.None)
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void onClick()
    {
        if (isSceneLoaded)
        {
            return;
        }

        isSceneLoaded = true;
       
        //엔딩 씬에서 게임 종료할 경우 이전 세이브포인트로 돌아가야 함 
        GameManager.instance.gameData.respawnScene = 6;
        GameManager.instance.gameData.SpawnSavePoint_bool = true;
        GameManager.instance.gameData.UseOpeningElevetor_bool = false;

        //새로 만든 GM 데이터를 알맞은 번호의 gameData에 저장 
        string ToJsonData = JsonUtility.ToJson(GameManager.instance.gameData);
        string filePath = Application.persistentDataPath + GameManager.instance.gameDataFileNames[GameManager.instance.curSaveFileNum];
        File.WriteAllText(filePath, ToJsonData);

        GameManager.instance.nextScene = 6;

        UIManager.instance.FadeOut(1.5f);
        Invoke("loadMainMenu", 2f);
    }

    void loadMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
