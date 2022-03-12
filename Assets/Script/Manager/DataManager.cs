using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DataManager : MonoBehaviour
{
    public const string GameDataFileName = "GameData.json";
    GameData gameData;
    int shakedFloorNum;
    int iceNum;
    int detectorNum;
    int buttonNum;

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

    public void Awake()
    {
        shakedFloorNum = GameManager.instance.shakedFloorNum;
        iceNum = GameManager.instance.iceNum;
        detectorNum = GameManager.instance.detectorNum;
        buttonNum = GameManager.instance.buttonNum;
        gameData = new GameData();
        gameData.storedIsShaked = new bool[GameManager.instance.shakedFloorNum];
        gameData.storedIsMelted = new bool[GameManager.instance.iceNum];
        gameData.storedIsDetected = new bool[GameManager.instance.detectorNum];
        gameData.storedIsGreen = new bool[GameManager.instance.buttonNum];
        gameData.storedPos = new Vector2[GameManager.instance.buttonNum];
    }

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
        for (int i = 0; i < shakedFloorNum; i++)
        {
            gameData.storedIsShaked[i] = GameManager.instance.storedIsShaked[i];
        }
        for (int i = 0; i < iceNum; i++)
        {
            gameData.storedIsMelted[i] = GameManager.instance.storedIsMelted[i];
        }
        for (int i = 0; i < detectorNum; i++)
        {
            gameData.storedIsDetected[i] = GameManager.instance.storedIsDetected[i];
        }
        for (int i = 0; i < buttonNum; i++)
        {
            gameData.storedIsGreen[i] = GameManager.instance.storedIsGreen[i];
            gameData.storedPos[i] = GameManager.instance.storedPos[i];
        }
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
        for (int i = 0; i < shakedFloorNum; i++)
        {
            GameManager.instance.storedIsShaked[i] = gameData.storedIsShaked[i];
        }
        for (int i = 0; i < iceNum; i++)
        {
            GameManager.instance.storedIsMelted[i] = gameData.storedIsMelted[i];
        }
        for (int i = 0; i < detectorNum; i++)
        {
            GameManager.instance.storedIsDetected[i] = gameData.storedIsDetected[i];
        }
        for (int i = 0; i < buttonNum; i++)
        {
            GameManager.instance.storedIsGreen[i] = gameData.storedIsGreen[i];
            GameManager.instance.storedPos[i] = gameData.storedPos[i];
        }
    }
}
