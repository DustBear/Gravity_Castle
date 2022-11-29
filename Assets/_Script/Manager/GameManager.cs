using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Linq;

public class GameManager : Singleton<GameManager>
{    
    public bool shouldSpawnSavePoint = true;    
    public bool shouldUseOpeningElevator = false;

    public string[] stageName = new string[7]; //각 스테이지별 이름 
    public int[] saveNumForStage = new int[7]; //각 스테이지별로 세이브포인트가 몇개 있는지 저장하는 배열 
    public int nextScene {get; set;}
    public Vector2 nextPos {get; set;}
    public Vector2 nextGravityDir {get; set;}
    public Player.States nextState {get; set;}

    public bool isStartWithFlipX; //
  
    public string[] gameDataFileNames = {"/GameData0.json", "/GameData1.json", "/GameData2.json", "/GameData3.json"};
    public int curSaveFileNum; //현재 플레이중인 세이브파일 번호 
    public int saveFileCount; //전체 세이브파일 개수 
    public GameData gameData { get; private set; }
    public SaveFileSeq saveFileSeq {get; set;}
    string saveFileSeqName = "/SaveFileSeq.json";

    public AudioSource bgmMachine;
    public AudioClip[] bgmGroup;

    //public AudioSource moodMachine;
    //public AudioClip[] moodSoundGroup;

    [SerializeField] int purposeBgmIndex; //실행시키고자 하는 bgm index
    [SerializeField] int curBgmIndex; //현재 실행중인 bgm index
    public float masterVolume_bgm; 

    void Awake() 
    {
        DontDestroyOnLoad(gameObject);
        string filePath = Application.persistentDataPath + saveFileSeqName; //현재 플레이중인 세이브파일의 저장경로 

        gameData = new GameData(); //gameData 생성하기 

        if (File.Exists(filePath)) //세이브파일이 존재하면 
        {
            Debug.Log("file exist");

            string FromJsonData = File.ReadAllText(filePath);
            saveFileSeq = JsonUtility.FromJson<SaveFileSeq>(FromJsonData); //svaeFileSeq 가져오기 

            curSaveFileNum = saveFileSeq.saveFileSeqList.Last(); //마지막으로 실행했던 세이브파일 번호

            
            string filePath_2 = Application.persistentDataPath + gameDataFileNames[curSaveFileNum];
            string FromJsonData_2 = File.ReadAllText(filePath_2);

            gameData = JsonUtility.FromJson<GameData>(FromJsonData_2); //선택한 세이브파일의 GameData 불러옴 
            
        }
        else //처음 시작하는 게임이면 
        {
            Debug.Log("file dont exist");

            saveFileSeq = new SaveFileSeq(); //SaveFileSeq 없으면 만들기 
            saveFileSeq.saveFileSeqList = new List<int>();
       
        }
        
        bgmMachine = gameObject.AddComponent<AudioSource>();
        bgmMachine.volume = masterVolume_bgm;
        //moodMachine = gameObject.AddComponent<AudioSource>();
    }

    public void soundNumCheck() //매 프레임마다 실행 
    {
        int curSceneNum = SceneManager.GetActiveScene().buildIndex;
        if(curSceneNum == 0)
        {
            //메인메뉴
            purposeBgmIndex = 0;
        }
        else if(curSceneNum == 1)
        {
            //인게임 메뉴 
            purposeBgmIndex = curBgmIndex;
        }
        else if(curSceneNum == 3)
        {
            //오프닝 애니메이션
            purposeBgmIndex = 10; //index 10이 오프닝 애니메이션 bgm 
        }
        else //각각의 스테이지에 진입했을 때 
        {
            int stageNum = gameData.curStageNum; //현재의 스테이지 번호에 따라 bgm 및 moodSound의 index 바뀜 

            switch (stageNum)
            {
                case 1:
                    purposeBgmIndex = 1;
                    break;
                case 2:
                    purposeBgmIndex = 2;
                    break;
                case 3:
                    purposeBgmIndex = 3;
                    break;
                case 4:
                    purposeBgmIndex = 4;
                    break;
            }
        }

        if(purposeBgmIndex != curBgmIndex) //현재 씬에서 실행해야 하는 bgm이 이미 실행되고 있으면 무시해야 함 
        {
            StopCoroutine("soundManager"); //기존에 실행중이던 sound 중지 
            StartCoroutine("soundManager");
        }
    } 
    IEnumerator soundManager()
    {
        curBgmIndex = purposeBgmIndex;

        for (int index = 20; index >= 1; index--)
        {
            if (bgmMachine.volume == 0)
            {
                break;
            }

            bgmMachine.volume = bgmMachine.volume-masterVolume_bgm/20f;
            //moodMachine.volume = bgmMachine.volume;
            yield return new WaitForSeconds(0.05f);

            //현재 볼륨이 얼마이든 서서히 줄여서 0으로 만듦        
        }
              
        bgmMachine.clip = bgmGroup[purposeBgmIndex]; //bgm clip 에 해당하는 bgm 파일 할당   
        //moodMachine.clip = moodSoundGroup[purposeBgmIndex];

        yield return new WaitForSeconds(1f); //1초간 음악 정지 
        bgmMachine.Play();
        //moodMachine.Play();
   
        for (int index = 1; index <= 20; index++)
        {
            bgmMachine.volume = masterVolume_bgm / 20f * index;
            //moodMachine.volume = bgmMachine.volume;
            yield return new WaitForSeconds(0.05f); //다시 서서히 volume 키움 
        }
    }

    IEnumerator soundOff() //GM에 의한 bgm 제어를 끄고 해당 씬에서 직접 음향을 제어해야 할 때 사용
    {
        for (int index = 20; index >= 1; index--)
        {
            if (bgmMachine.volume == 0)
            {
                break;
            }

            bgmMachine.volume = bgmMachine.volume - 0.05f;
            //moodMachine.volume = bgmMachine.volume;
            yield return new WaitForSeconds(0.05f);

            //현재 볼륨이 얼마이든 서서히 줄여서 0으로 만듦        
        }
    }


    void Start()
    {
        //게임이 시작하면 MainMenu 로드 
        SceneManager.LoadScene("MainMenu"); 
        
        //bgm 과련 코드 
        //purposeBgmIndex = 0; //main theme 에서 틀어줘야 하는 bgm 은 9 
        //curBgmIndex = 0;
        //bgmMachine.clip = bgmGroup[0];
        //moodMachine.clip = moodSoundGroup[9];
        //bgmMachine.Play();
        //moodMachine.Play();

        bgmMachine.loop = true;
        //moodMachine.loop = true;
        bgmMachine.playOnAwake = false;
        //moodMachine.playOnAwake = false;
    }

    private void Update()
    {
        soundNumCheck();

        //게임이 제대로 작동하는 지 체크하기 위한 함수 ~> 출시버전에선 삭제해야 함 
        if (Input.GetKeyDown(KeyCode.U))
        {
            Debug.Log("curAchieve: " + gameData.curAchievementNum
                + "  curStageNum: " + gameData.curStageNum
                + "\nfinalAchieve: " + gameData.finalAchievementNum
                + "  finalStage: " + gameData.finalStageNum
                );

            gameData.collectionTmp.Clear(); //임시저장한 탐험가상자 지움
            for(int index=0; index<gameData.collectionUnlock.Length; index++)
            {
                gameData.collectionUnlock[index] = false;
                // 모든 탐험가상자 수집기록 삭제 
            }
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            string outputStr = "";

            for (int index=0; index<gameData.savePointUnlock.Length; index++)
            {
                if (gameData.savePointUnlock[index]==1) 
                {
                    outputStr += "True, ";
                }
                else
                {
                    outputStr += "False, ";
                }

                if ((index+1) % 8 == 0) outputStr += "\n";
            }

            Debug.Log(outputStr);
        }
        
    }

    //세이브포인트 불러오기 버튼에 현재 진행도 표시하는 함수 
    public KeyValuePair<int, int> GetSavedData(int saveFileNum)
    {
        string filePath = Application.persistentDataPath + gameDataFileNames[saveFileNum];
        if (File.Exists(filePath))
        {
            string FromJsonData = File.ReadAllText(filePath);
            GameData curGameData = JsonUtility.FromJson<GameData>(FromJsonData);
            return new KeyValuePair<int, int>(curGameData.finalStageNum, curGameData.finalAchievementNum);
        }
        return new KeyValuePair<int, int>(-1, -1); //세이브가 없으면 (-1, -1) 반환 
    }  


    public void SaveData(int savePointNum, int stageNum, Vector2 playerPos) //세이브포인트에서 실행하는 함수 
    {
        gameData.respawnScene = SceneManager.GetActiveScene().buildIndex; //현재 세이브포인트가 있는 씬에서 리스폰해야 함 
        gameData.respawnPos = playerPos;

        gameData.curAchievementNum = savePointNum;
        gameData.curStageNum = stageNum;

        if(gameData.finalStageNum < stageNum) //만약 스테이지를 갱신하는 세이브포인트를 활성화시켰으면 
        {
            gameData.finalStageNum = stageNum; //finalStage 갱신 
            gameData.finalAchievementNum = savePointNum; //단순히 1이 되면x ~> stage4 에서 stage5 savePoint 2 로 한번에 넘어갈 수도 있기 때문 
        }else if(gameData.finalStageNum == stageNum) //동일 스테이지 내에서 최종 achNum 만 갱신하는 세이브포인트를 활성화시켰으면 
        {
            if(gameData.finalAchievementNum < savePointNum)
            {
                gameData.finalAchievementNum = savePointNum;
            }
        }
        
        gameData.savePointUnlock[saveNumCalculate(new Vector2(stageNum, savePointNum))] = 1; //세이브포인트 활성화 여부 저장 
        gameData.respawnGravityDir = Physics2D.gravity.normalized;
        
        if(gameData.collectionTmp.Count != 0) //만약 세이브해야 할 수집요소가 있다면
        {
            for(int index=0; index < gameData.collectionTmp.Count; index++)
            {
                int colNum = gameData.collectionTmp[index];
                gameData.collectionUnlock[colNum] = true; //해당 수집요소 수집 완료 
            }
        }

        gameData.collectionTmp.Clear(); // 리스트는 정리 

        //GameData 에 데이터 저장 
        string ToJsonData = JsonUtility.ToJson(gameData);
        string filePath = Application.persistentDataPath + gameDataFileNames[curSaveFileNum];
        File.WriteAllText(filePath, ToJsonData);
    }

    public void SaveSaveFileSeq() //마지막으로 실행한 세이브 기억
    {
        string ToJsonData = JsonUtility.ToJson(saveFileSeq);
        string filePath = Application.persistentDataPath + saveFileSeqName; 
        File.WriteAllText(filePath, ToJsonData);
    }
    
    //(stageNum, saveNum) 이 들어오면 int (전체 게임에서 몇 번째 세이브인지) 로 환산해 반환하는 함수 
    // 입력값은 (1,1) 부터 시작하고 출력값은 index 기준으로 0부터 시작한다 
    public int saveNumCalculate(Vector2 saveData) //x가 stageNum, y가 savePointNum 
    {
        if (saveData.x == 1)
        {
            return ((int)saveData.y - 1);
        }

        int saveIndex = 0;
        for (int index = 0; index < (int)saveData.x - 1; index++) //ex) stage 3 이라면 stage1, stage2의 전체 세이브포인트 개수를 더하고,
        {
            saveIndex += saveNumForStage[index];
        }

        saveIndex += (int)saveData.y - 1; //stage3에서 내가 몇번째 세이브인지도 더해야 함 
        return saveIndex;
    }

    public int collectionNumCalculate(Vector2 collectionData)
    {       
        int collectionIndex = ((int)collectionData.x - 1) * 30 + (int)collectionData.y - 1;
        //각 스테이지에는 30개의 수집요소가 있다고 가정 
      
        return collectionIndex;
    }
}
