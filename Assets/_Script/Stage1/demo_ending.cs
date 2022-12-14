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
       
        //���� ������ ���� ������ ��� ���� ���̺�����Ʈ�� ���ư��� �� 
        GameManager.instance.gameData.respawnScene = 6;
        GameManager.instance.gameData.SpawnSavePoint_bool = true;
        GameManager.instance.gameData.UseOpeningElevetor_bool = false;

        //���� ���� GM �����͸� �˸��� ��ȣ�� gameData�� ���� 
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
