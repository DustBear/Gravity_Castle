using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    // Store informations
    [HideInInspector] public bool isDie;
    public int curAchievementNum;
    [HideInInspector] public bool isChangeGravityDir;

    // If next scene
    public int nextScene;
    public Vector2 nextPos;
    public Vector2 nextGravityDir;
    public Player.RopingState nextRopingState;
    public Player.LeveringState nextLeveringState;
    public bool nextIsJumping;

    // If die
    [HideInInspector] public int respawnScene;
    [HideInInspector] public Vector2 respawnPos;
    [HideInInspector] public Vector2 respawnGravityDir;
    [HideInInspector] public Player.RopingState respawnRopingState;

    // Shaked floor of stage5
    public int shakedFloorNum;
    [HideInInspector] public bool[] curIsShaked;
    [HideInInspector] public bool[] storedIsShaked;

    // Ice of stage6
    public int iceNum;
    [HideInInspector] public bool[] curIsMelted;
    [HideInInspector] public bool[] storedIsMelted;

    // Player detector of stage6
    public int detectorNum;
    [HideInInspector] public bool[] curIsDetected;
    [HideInInspector] public bool[] storedIsDetected;

    // Player detector button of stage6
    public int buttonNum;
    [HideInInspector] public bool[] curIsGreen;
    [HideInInspector] public bool[] storedIsGreen;
    [HideInInspector] public Vector2[] curPos;
    [HideInInspector] public Vector2[] storedPos;

    // Cliff of stage8
    [HideInInspector] public bool isCliffChecked;

    protected GameManager() {}
    
    void Awake() {
        DontDestroyOnLoad(gameObject);
        curIsShaked = new bool[shakedFloorNum];
        storedIsShaked = new bool[shakedFloorNum];
        curIsMelted = new bool[iceNum];
        storedIsMelted = new bool[iceNum];
        curIsDetected = new bool[detectorNum];
        storedIsDetected = new bool[detectorNum];
        curIsGreen = new bool[buttonNum];
        storedIsGreen = new bool[buttonNum];
        curPos = new Vector2[buttonNum];
        storedPos = new Vector2[buttonNum];
    }

    void Start()
    {
        SceneManager.LoadScene(0);
    }

    public void StartGame(bool isNewGame)
    {
        if (isNewGame)
        {
            curAchievementNum = 0;
            nextScene = 1;
            nextPos = new Vector2(-161.5f, -7f);
            nextGravityDir = Vector2.down;
            isCliffChecked = false;
            for (int i = 0; i < shakedFloorNum; i++)
            {
                curIsShaked[i] = false;
                storedIsShaked[i] = false;
            }
            for (int i = 0; i < iceNum; i++)
            {
                curIsMelted[i] = false;
                storedIsMelted[i] = false;
            }
            for (int i = 0; i < detectorNum; i++)
            {
                curIsDetected[i] = false;
                storedIsDetected[i] = false;
            }
            for (int i = 0; i < buttonNum; i++)
            {
                curIsGreen[i] = false;
                storedIsGreen[i] = false;
                curPos[i] = Vector2.zero;
                storedPos[i] = Vector2.zero;
            }
        }
        isDie = false;
        isChangeGravityDir = false;
        nextRopingState = Player.RopingState.idle;
        nextLeveringState = Player.LeveringState.idle;
        nextIsJumping = false;
        respawnScene = nextScene;
        respawnPos = nextPos;
        respawnGravityDir = nextGravityDir;
        respawnRopingState = nextRopingState;

        Cursor.lockState = CursorLockMode.Locked;
        SceneManager.LoadScene(nextScene);
    }

    public void InitShakedFloorInfo()
    {
        for (int i = 0; i < shakedFloorNum; i++)
        {
            curIsShaked[i] = storedIsShaked[i];
        }
    }

    public void UpdateShakedFloorInfo()
    {
        for (int i = 0; i < shakedFloorNum; i++)
        {
            storedIsShaked[i] = curIsShaked[i];
        }
    }

    public void InitIceInfo()
    {
        for (int i = 0; i < iceNum; i++)
        {
            curIsMelted[i] = storedIsMelted[i];
        }
    }

    public void UpdateIceInfo()
    {
        for (int i = 0; i < iceNum; i++)
        {
            storedIsMelted[i] = curIsMelted[i];
        }
    }

    public void InitDetectorInfo()
    {
        for (int i = 0; i < detectorNum; i++)
        {
            curIsDetected[i] = storedIsDetected[i];
        }
    }

    public void UpdateDetectorInfo()
    {
        for (int i = 0; i < detectorNum; i++)
        {
            storedIsDetected[i] = curIsDetected[i];
        }
    }

    public void InitButtonInfo()
    {
        for (int i = 0; i < buttonNum; i++)
        {
            curIsGreen[i] = storedIsGreen[i];
        }
    }

    public void UpdateButtonInfo()
    {
        for (int i = 0; i < buttonNum; i++)
        {
            storedIsGreen[i] = curIsGreen[i];
        }
    }

    public void InitPosInfo()
    {
        for (int i = 0; i < buttonNum; i++)
        {
            curPos[i] = storedPos[i];
        }
    }

    public void UpdatePosInfo()
    {
        for (int i = 0; i < buttonNum; i++)
        {
            storedPos[i] = curPos[i];
        }
    }
}