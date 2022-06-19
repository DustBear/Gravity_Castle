using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

// GameManager는 다음 scene으로 이동 시 유지해야할 data를 보관
public class GameManager : Singleton<GameManager>
{
    // 1) 모든 Stage 공통
    [HideInInspector] public bool shouldStartAtSavePoint {get; set;}
    public int nextScene {get; set;}
    public Vector2 nextPos {get; set;}
    public Vector2 nextGravityDir {get; set;}
    public Player.States nextState {get; set;}

    public bool isStartWithFlipX; //플레이어가 씬을 시작할 때 어느 쪽을 바라봐야 하는지 결정

    // 2) Stage5의 밟으면 떨어지는 floor 
    [SerializeField] int shakedFloorNum = 36;
    [HideInInspector] public bool[] curIsShaked {get; set;}

    // 3) Stage6의 ice 
    [SerializeField] int iceNum = 58;
    [HideInInspector] public bool[] curIsMelted {get; set;}

    // 4) Stage6의 플레이어 detect 여부
    [SerializeField] int detectorNum = 6;
    [HideInInspector] public bool[] curIsDetected {get; set;}

    // 5) Stage6의 button 색깔
    [SerializeField] int buttonNum = 11;
    [HideInInspector] public bool[] curIsGreen {get; set;}
    [HideInInspector] public Vector2[] curPos {get; set;}

    // 6) Stage8의 절벽 확인 여부
    [HideInInspector] public bool isCliffChecked = false;

    const string GameDataFileName = "GameData.json";
    public GameData gameData {get; private set;}

    [SerializeField] Vector2 firstStartPos; // 게임 시작 위치
    [SerializeField] Vector2 firstRespawnPos; // 게임 시작하자마자 나가면 여기서 부활
    
    void Awake() {
        DontDestroyOnLoad(gameObject); //씬 넘어가도 파괴x

        curIsShaked = new bool[shakedFloorNum];
        curIsMelted = new bool[iceNum];
        curIsDetected = new bool[detectorNum];
        curIsGreen = new bool[buttonNum];
        curPos = new Vector2[buttonNum];

        gameData = new GameData();
        gameData.storedIsShaked = new bool[shakedFloorNum];
        gameData.storedIsMelted = new bool[iceNum];
        gameData.storedIsDetected = new bool[detectorNum];
        gameData.storedIsGreen = new bool[buttonNum];
        gameData.storedPos = new Vector2[buttonNum];
    }

    void Start()
    {
        //게임 시작하면 mainMenu 창 열기
        SceneManager.LoadScene("MainMenu"); 
        Debug.Log("GM start");
    }

    public void StartGame(bool isLoadGame)
    {
        InitData(isLoadGame); //초기화 

        InputManager.instance.isInputBlocked = false; //입력제한 풀기 
        InputManager.instance.isJumpBlocked = false; //점프제한 풀기 
        Cursor.lockState = CursorLockMode.Locked; //기본적으로 마우스 잠그기 
        if (!shouldStartAtSavePoint) 
        {
            SceneManager.LoadScene(nextScene); //처음 시작하는 것이므로 이 시점에 nextScene=1
        }
        else SceneManager.LoadScene(gameData.respawnScene); //처음 시작하는 것이 아니라면 리스폰 장소에서 시작 

        isStartWithFlipX = false;
    }

    public void CheckSavedGame(bool isLoadGame) //게임 씬 시작은 이 함수로 수행
    {
        string filePath = Application.persistentDataPath + GameDataFileName;
        // New Game일 경우
        if (!isLoadGame)
        {
            if (File.Exists(filePath))
            {
                UIManager.instance.ExistSavedGame(); //기존 게임파일이 존재합니다 메시지 출력 
            }
            else
            {
                StartGame(isLoadGame); //파일 없으면 바로 새 게임 시작
            }
        }
        // Load Game일 경우
        else
        {
            if (File.Exists(filePath))
            {
                shouldStartAtSavePoint = true; //경로에 파일이 존재할 경우 true
                StartGame(isLoadGame);
            }
            else
            {
                UIManager.instance.NoSavedGame(); //게임 불러오기 했는데 파일 경로에 아무것도 없을 경우에 경고메시지 출력 
            }
        }
    }

    // Case 1) 메인메뉴에서 New Game을 눌렀을 시 호출됨
    // Case 2) 메인메뉴에서 Load Game을 눌렀을 시 호출됨 
    // Case 3) 게임 내에서 사망했을 시 호출됨

    // Case 1은 savePoint가 아니라 초기 스폰장소에서 스폰. Case2,3은 savePoint에서 스폰
    public void InitData(bool isLoadGame) //플레이에 필요한 데이터 초기화 or 불러오는 함수 
    {
        // Case 1
        if (!shouldStartAtSavePoint) //스테이지1부터 시작
        {
            // 게임 시작 시 필요한 데이터 초기화
            nextScene = 1;
            nextPos = firstStartPos;
            nextGravityDir = Vector2.down; //시작하면 아래쪽 보도록 중력 적용
            nextState = Player.States.Walk; //맨 처음 시작할 때 플레이어의 상태는 walk

            // 기본적으로 false로 초기화 되어 있기 때문에 주석 처리
            // nextIsJumpBlocked = false;
            // for (int i = 0; i < shakedFloorNum; i++) curIsShaked[i] = false;
            // for (int i = 0; i < iceNum; i++) curIsMelted[i] = false;
            // for (int i = 0; i < detectorNum; i++) curIsDetected[i] = false;
            // for (int i = 0; i < buttonNum; i++) curIsGreen[i] = false;
            for (int i = 0; i < buttonNum; i++) curPos[i] = Vector2.zero;

            // 리스폰 시 필요한 데이터 초기화
            // 게임 시작하자마자 나갔다가 다시 접속했을 시 load가 가능하도록
            gameData.curAchievementNum = 0;
            gameData.curStageNum = 1;
            gameData.respawnScene = 1;
            gameData.respawnPos = firstRespawnPos;
            gameData.respawnGravityDir = Vector2.down;
            gameData.isCliffChecked = false;
            curIsShaked.CopyTo(gameData.storedIsShaked, 0);
            curIsMelted.CopyTo(gameData.storedIsMelted, 0);
            curIsDetected.CopyTo(gameData.storedIsDetected, 0);
            curIsGreen.CopyTo(gameData.storedIsGreen, 0);
            curPos.CopyTo(gameData.storedPos, 0);     

            //데이터 저장 
            string ToJsonData = JsonUtility.ToJson(gameData);
            string filePath = Application.persistentDataPath + GameDataFileName;
            File.WriteAllText(filePath, ToJsonData);       
        }

        // Case 2, 3
        else
        {
            // Case 2인 경우, Json 파일의 데이터들을 모두 GameData class로 불러옴
            if (isLoadGame) //불러올 게임 데이터가 있으면 불러옴
            {
                string filePath = Application.persistentDataPath + GameDataFileName;
                string FromJsonData = File.ReadAllText(filePath);
                gameData = JsonUtility.FromJson<GameData>(FromJsonData);
            }
            // GameData class의 데이터들을 GameManager 데이터에 저장
            isCliffChecked = gameData.isCliffChecked;
            gameData.storedIsShaked.CopyTo(curIsShaked, 0);
            gameData.storedIsMelted.CopyTo(curIsMelted, 0);
            gameData.storedIsDetected.CopyTo(curIsDetected, 0);
            gameData.storedIsGreen.CopyTo(curIsGreen, 0);
            gameData.storedPos.CopyTo(curPos, 0);
        }
    }

    // Save Point에 도달했을 시 호출됨
    public void SaveData(int achievementNum, int stageNum, Vector2 playerPos) 
        //게임 세이브는 세이브포인트에서 이루어짐. SavePoint에 닿을 때 마다 Json 파일에 데이터가 저장됨
    {        
        gameData.respawnScene = SceneManager.GetActiveScene().buildIndex; //현재 씬에서 다시 부활해야 함
        gameData.respawnPos = playerPos;

        gameData.curAchievementNum = achievementNum;
        gameData.curStageNum = stageNum;
        gameData.respawnGravityDir = Physics2D.gravity.normalized;
        gameData.isCliffChecked = isCliffChecked;
        curIsShaked.CopyTo(gameData.storedIsShaked, 0);
        curIsMelted.CopyTo(gameData.storedIsMelted, 0);
        curIsDetected.CopyTo(gameData.storedIsDetected, 0);
        curIsGreen.CopyTo(gameData.storedIsGreen, 0);
        curPos.CopyTo(gameData.storedPos, 0);       
        
        // gameData class의 데이터들을 모두 Json 파일에 저장
        string ToJsonData = JsonUtility.ToJson(gameData);
        string filePath = Application.persistentDataPath + GameDataFileName;
        File.WriteAllText(filePath, ToJsonData);
    }
}