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

    // shaked floor
    [SerializeField] int shakedFloorNum;
    [HideInInspector] public bool[] curIsShaked;
    [HideInInspector] public bool[] storedIsShaked;

    [HideInInspector] public bool isCliffChecked;

    protected GameManager() {}
    
    void Awake() {
        DontDestroyOnLoad(gameObject);
        curIsShaked = new bool[shakedFloorNum];
        storedIsShaked = new bool[shakedFloorNum];
    }

    void Start()
    {
        SceneManager.LoadScene(0);
    }

    public void Init()
    {
        isDie = false;;
        curAchievementNum = 0;
        isChangeGravityDir = false;
        nextScene = 0;
        nextPos = new Vector2(-161.5f, -7f);
        nextGravityDir = Vector2.down;
        nextRopingState = Player.RopingState.idle;
        nextLeveringState = Player.LeveringState.idle;
        nextIsJumping = false;
        respawnScene = 1;
        respawnPos = nextPos;
        respawnGravityDir = nextGravityDir;
        respawnRopingState = nextRopingState;
        isCliffChecked = false;
        for (int i = 0; i < shakedFloorNum; i++)
        {
            curIsShaked[i] = false;
            storedIsShaked[i] = false;
        }
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
}