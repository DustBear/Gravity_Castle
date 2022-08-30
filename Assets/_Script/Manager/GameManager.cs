using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

// GameManager는 다음 scene으로 이동 시 유지해야할 data를 보관
public class GameManager : Singleton<GameManager>
{    
    public bool shouldSpawnSavePoint = true;
    //빠른메뉴나 시작키, 죽고 나서 부활할 때는 세이브포인트에서 씬 시작. 단순히 changeScene으로 씬과 씬을 이동할 때는 세이브포인트 이용x 
    
    public bool shouldUseOpeningElevator = false;

    public int nextScene {get; set;}
    public Vector2 nextPos {get; set;}
    public Vector2 nextGravityDir {get; set;}
    public Player.States nextState {get; set;}

    public bool isStartWithFlipX; //플레이어가 씬을 시작할 때 어느 쪽을 바라봐야 하는지 결정
  
    public string[] gameDataFileNames = {"/GameData0.json", "/GameData1.json", "/GameData2.json", "/GameData3.json"};
    public int curSaveFileNum; // 현재 실행중인 게임의 세이브파일 번호
    public int saveFileCount; // 전체 세이브파일 수
    public GameData gameData {get; private set;}
    public SaveFileSeq saveFileSeq {get; set;}
    string saveFileSeqName = "/SaveFileSeq.json";

    [SerializeField] Vector2 firstStartPos; // 게임 시작 위치
    [SerializeField] int firstScene;

    public AudioSource bgmMachine; //각 스테이지에 맞는 bgm 송출
    public AudioClip[] bgmGroup;

    public AudioSource moodMachine; //각 스테이지에 맞는 무드 사운드 송출 
    public AudioClip[] moodSoundGroup;

    [SerializeField] int purposeBgmIndex; //재생되어야 하는 bgm 사운드 번호
    [SerializeField] int curBgmIndex; //현재 재생되고 있는 bgm 사운드의 번호(0~10)
    
    void Awake() 
    {
        DontDestroyOnLoad(gameObject); //씬 넘어가도 파괴x

        string filePath = Application.persistentDataPath + saveFileSeqName; 
        if (File.Exists(filePath)) //파일 존재할 때 
        {
            string FromJsonData = File.ReadAllText(filePath);
            saveFileSeq = JsonUtility.FromJson<SaveFileSeq>(FromJsonData); //svaeFileSeq 를 그대로 받아옴 
        }
        else //파일 존재하지 않을 때 
        {
            saveFileSeq = new SaveFileSeq(); //새로 하나 만들어야 함 
            saveFileSeq.saveFileSeqList = new List<int>();
        }

        gameData = new GameData();

        bgmMachine = gameObject.AddComponent<AudioSource>();
        moodMachine = gameObject.AddComponent<AudioSource>();
    }

    public void soundNumCheck() //현재 씬에서 몇 번 bgm이 출력되어야 할 지 체크
    {
        int sceneNum = SceneManager.GetActiveScene().buildIndex; //현재 씬 번호

        //현재 위치한 scene의 번호에 따라 목표 bgm index가 변화함
        if (sceneNum == 0) purposeBgmIndex = 9; //메인메뉴 bgm은 index 9 
        else if (sceneNum == 1) purposeBgmIndex = 10; //오프닝 씬 bgm은 index 10 
        else if (2 <= sceneNum && sceneNum < 4) purposeBgmIndex = 0; //stage0
        else if ((4 <= sceneNum && sceneNum < 7) || (sceneNum == 26) || (sceneNum == 27) || (sceneNum == 30)) purposeBgmIndex = 1; //stage1
        else if ((7 <= sceneNum && sceneNum < 10) || (sceneNum == 28)) purposeBgmIndex = 2; //stage2
        else if ((10 <= sceneNum && sceneNum < 12) || (sceneNum == 29)) purposeBgmIndex = 3; //stage3
        else if (12 <= sceneNum && sceneNum < 14) purposeBgmIndex = 4; //stage4

        if(purposeBgmIndex != curBgmIndex) //현재 재생중인 bgm 인덱스와 목표 인덱스가 다름 ~> 씬이 바뀌었다는 뜻이므로 인덱스 바꿔줘야 함
        {
            StopCoroutine("soundManager"); //아직 코루틴이 다 끝나지 않은 상태에서 다음 음향 코루틴 시작하면 이전 코루틴은 지워야 함
            StartCoroutine("soundManager");
        }
    } 
    IEnumerator soundManager() //튜토리얼:0 부터 시작 ~> 스테이지1 bgm은 index=1 / 게임메뉴의 bgm index=9(10번째)
    {
        curBgmIndex = purposeBgmIndex;

        for (int index = 10; index >= 1; index--)
        {
            if (bgmMachine.volume == 0)
            {
                break;
            }

            bgmMachine.volume = bgmMachine.volume-0.1f;
            moodMachine.volume = bgmMachine.volume;
            yield return new WaitForSeconds(0.1f);

            //일정 속도로 volume을 줄여서 음향 크기 0으로 만듦. 1에서 시작했으면 1초가 걸리고 0.5에서 시작했으면 0.5초가 걸림            
        }
              
        bgmMachine.clip = bgmGroup[purposeBgmIndex]; //bgm 사운드를 목푯값에 맞게 교체     
        moodMachine.clip = moodSoundGroup[purposeBgmIndex];

        yield return new WaitForSeconds(1f); //잠시 기다렸다가        
        bgmMachine.Play();
        moodMachine.Play();
   
        for (int index = 1; index <= 20; index++)
        {
            bgmMachine.volume = 0.05f * index;
            moodMachine.volume = bgmMachine.volume;
            yield return new WaitForSeconds(0.1f); //2초에 걸쳐 음향 크기 1으로 만듦
        }

    }

    void Start()
    {
        //게임 시작하면 mainMenu 창 열기
        SceneManager.LoadScene("MainMenu"); 
        
        //bgm 관련 코드 
        purposeBgmIndex = 9; //main theme 이 초깃값이 되어야 함
        curBgmIndex = 9;
        bgmMachine.clip = bgmGroup[9];
        moodMachine.clip = moodSoundGroup[9];
        bgmMachine.Play();
        moodMachine.Play();

        bgmMachine.loop = true;
        moodMachine.loop = true;
        bgmMachine.playOnAwake = false;
        moodMachine.playOnAwake = false;
    }

    private void Update()
    {
        soundNumCheck();

        //제대로 작동하는지 체크하기 위한 코드 ~> 출시 빌드에선 삭제
        if (Input.GetKeyDown(KeyCode.U))
        {
            Debug.Log("curAchieve: " + gameData.curAchievementNum
                + "  curStageNum: " + gameData.curStageNum
                + "\nfinalAchieve: " + gameData.finalAchievementNum
                + "  finalStage: " + gameData.finalStageNum
                );
        }
    }

    /*
    public void StartGame(bool isStartNew) //true 값 주면 새로 시작하는 게임 false값 주면 기존 세이브에서 시작하는 게임 
    {
        if (!isStartNew)
        {

        }


        InitData(isLoadGame); //초기화 
        shouldSpawnSavePoint = true;

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
    */

    //세이브파일 앞에 어디까지 진행했는지 문자 띄워줌 
    public KeyValuePair<int, int> GetSavedData(int saveFileNum)
    {
        string filePath = Application.persistentDataPath + gameDataFileNames[saveFileNum];
        if (File.Exists(filePath))
        {
            string FromJsonData = File.ReadAllText(filePath);
            GameData curGameData = JsonUtility.FromJson<GameData>(FromJsonData);
            return new KeyValuePair<int, int>(curGameData.finalStageNum, curGameData.finalAchievementNum);
        }
        return new KeyValuePair<int, int>(-1, -1); //경로에 세이브파일이 없으면 ~> '새 게임' 텍스트 표시해야 함 
    }  

    /*
    // Case 1) 메인메뉴에서 New Game을 눌렀을 시 호출됨
    // Case 2) 메인메뉴에서 Load Game을 눌렀을 시 호출됨 
    // Case 3) 게임 내에서 사망했을 시 호출됨

    // Case 1은 savePoint가 아니라 초기 스폰장소에서 스폰. Case2,3은 savePoint에서 스폰
    public void InitData(bool isLoadGame) //플레이에 필요한 데이터 초기화 or 불러오는 함수 
    {
        // Case 1
        if (!shouldStartAtSavePoint) //스테이지0부터 시작
        {
            // 게임 시작 시 필요한 데이터 초기화
            nextScene = firstScene; //castleEnterance 씬 
            nextPos = firstStartPos; //배에서 내리는 위치 
            nextGravityDir = Vector2.down; //시작하면 아래쪽 보도록 중력 적용
            nextState = Player.States.Walk; //맨 처음 시작할 때 플레이어의 상태는 walk

            // 리스폰 시 필요한 데이터 초기화
            // 게임 시작하자마자 나갔다가 다시 접속했을 시 load가 가능하도록
            gameData.curAchievementNum = 0; //아직 아무런 세이브포인트도 활성화하지 않았다면 0 
            gameData.curStageNum = 0; //castleEnterance의 스테이지번호는 0 
            gameData.respawnScene = nextScene;
            gameData.respawnPos = nextPos;
            gameData.respawnGravityDir = nextGravityDir;
            
            for(int i=0; i<8; i++)
            {
                for(int j=0; j<50; j++)
                {
                    gameData.savePointUnlock[i, j] = false; //모든 세이브포인트 비활성화 
                }
            }
           
            //데이터 저장 
            string ToJsonData = JsonUtility.ToJson(gameData);
            string filePath = Application.persistentDataPath + gameDataFileNames[curSaveFileNum];
            File.WriteAllText(filePath, ToJsonData);       
        }

        // Case 2
        else
        {
            // Case 2인 경우, Json 파일의 데이터들을 모두 GameData class로 불러옴
            if (isLoadGame) //불러올 게임 데이터가 있으면 불러옴
            {
                string filePath = Application.persistentDataPath + gameDataFileNames[curSaveFileNum];
                string FromJsonData = File.ReadAllText(filePath);
                gameData = JsonUtility.FromJson<GameData>(FromJsonData);
            }
            // GameData class의 데이터들을 GameManager 데이터에 저장   

            // Case 3의 경우 어차피 모든 데이터는 죽기 전과 동일하므로 새로 초기화할 필요 없음 
            
        }
    }
    */

    // Save Point에 도달했을 때, 문을 열 때, key 획득할 때 사용 
    public void SaveData(int savePointNum, int stageNum, Vector2 playerPos) 
        // 세이브포인트에서 이루어지는 데이터 저장. SavePoint가 작동할 때 마다 Json 파일에 데이터가 저장됨
    {
        gameData.respawnScene = SceneManager.GetActiveScene().buildIndex; //현재 씬에서 다시 부활해야 함
        gameData.respawnPos = playerPos;

        gameData.curAchievementNum = savePointNum;
        gameData.curStageNum = stageNum;

        if(gameData.finalStageNum < stageNum)
        {
            gameData.finalStageNum = stageNum; //방금 세이브 한 스테이지가 finalStage보다 앞서 있으면 finalStage 갱신
            gameData.finalAchievementNum = savePointNum; //스테이지가 갱신되었으면 achievement Num은 무조건 갱신해야 함 
        }else if(gameData.finalStageNum == stageNum)
        {
            if(gameData.finalAchievementNum < savePointNum)
            {
                gameData.finalAchievementNum = savePointNum;
                //동일 스테이지에서 더 큰 achNum으로 이동할 때 갱신해 줌 ex) (1,10) ~> (1,11)
                //만약 스테이지가 더 작다면 finalAch보다 큰 achNum이 나와도 무시 ex) (2,10) ~> (1,13)으로 이동할 때는 finalAchNum 갱신x 
            }
        }

        gameData.savePointUnlock[stageNum - 1, savePointNum - 1] = true; //해당 세이브포인트 언락했다는 정보 저장
        gameData.respawnGravityDir = Physics2D.gravity.normalized;
        
        // gameData class의 데이터들을 모두 Json 파일에 저장
        string ToJsonData = JsonUtility.ToJson(gameData);
        string filePath = Application.persistentDataPath + gameDataFileNames[curSaveFileNum];
        File.WriteAllText(filePath, ToJsonData);
    }

    public void SaveSaveFileSeq()
    {
        string ToJsonData = JsonUtility.ToJson(saveFileSeq); //saveFileSeq 를 json 파일로 저장 
        string filePath = Application.persistentDataPath + saveFileSeqName; //파일경로 저장
        File.WriteAllText(filePath, ToJsonData);
    }
    
}
