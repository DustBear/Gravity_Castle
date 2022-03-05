using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DataManager : MonoBehaviour
{
    static DataManager _instance;
    static public DataManager instance
    {
        get
        {
            if (_instance == null)
            {
                _container = new GameObject();
                _container.name = "DataManager";
                _instance = _container.AddComponent(typeof(DataManager)) as DataManager;
                DontDestroyOnLoad(_container);
            }
            return _instance;
        }
    }

    static GameObject _container;
    static public GameObject container
    {
        get
        {
            return _container;
        }
    }

    public const string GameDataFileName = "GameData.json";

    GameData gameData = new GameData();

    public void CheckSavedGame(bool isNewGame)
    {
        string filePath = Application.persistentDataPath + GameDataFileName;
        if (isNewGame)
        {
            if (File.Exists(filePath))
            {
                UIManager.instance.ExistSavedGame();
            }
            else
            {
                GameManager.instance.StartGame(true);
            }
        }
        else
        {
            if (File.Exists(filePath))
            {
                LoadData();
                GameManager.instance.StartGame(false);
            }
            else
            {
                UIManager.instance.NoSavedGame();
            }
        }
    }

    public void SaveData()
    {
        gameData.curAchievementNum = GameManager.instance.curAchievementNum;
        gameData.respawnScene = GameManager.instance.respawnScene;
        gameData.respawnPos = GameManager.instance.respawnPos;
        gameData.respawnGravityDir = GameManager.instance.respawnGravityDir;
        gameData.isCliffChecked = GameManager.instance.isCliffChecked;
        gameData.storedIsShaked = GameManager.instance.storedIsShaked;

        string ToJsonData = JsonUtility.ToJson(gameData);
        string filePath = Application.persistentDataPath + GameDataFileName;
        File.WriteAllText(filePath, ToJsonData);
    }

    public void LoadData()
    {
        string filePath = Application.persistentDataPath + GameDataFileName;
        string FromJsonData = File.ReadAllText(filePath);
        gameData = JsonUtility.FromJson<GameData>(FromJsonData);

        GameManager.instance.curAchievementNum = gameData.curAchievementNum;
        GameManager.instance.nextScene = gameData.respawnScene;
        GameManager.instance.nextPos = gameData.respawnPos;
        GameManager.instance.nextGravityDir = gameData.respawnGravityDir;
        GameManager.instance.isCliffChecked = gameData.isCliffChecked;
        GameManager.instance.storedIsShaked = gameData.storedIsShaked;
    }

    void OnApplicationQuit()
    {
        SaveData();
    }
}
