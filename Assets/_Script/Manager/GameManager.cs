using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class GameManager : Singleton<GameManager>
{    
    public bool shouldSpawnSavePoint = true;    
    public bool shouldUseOpeningElevator = false;

    public int nextScene {get; set;}
    public Vector2 nextPos {get; set;}
    public Vector2 nextGravityDir {get; set;}
    public Player.States nextState {get; set;}

    public bool isStartWithFlipX; //
  
    public string[] gameDataFileNames = {"/GameData0.json", "/GameData1.json", "/GameData2.json", "/GameData3.json"};
    public int curSaveFileNum; //현재 플레이중인 세이브파일 번호 
    public int saveFileCount; //전체 세이브파일 개수 
    public GameData gameData {get; private set;}
    public SaveFileSeq saveFileSeq {get; set;}
    string saveFileSeqName = "/SaveFileSeq.json";

    [SerializeField] Vector2 firstStartPos; // ���� ���� ��ġ
    [SerializeField] int firstScene;

    public AudioSource bgmMachine; //�� ���������� �´� bgm ����
    public AudioClip[] bgmGroup;

    public AudioSource moodMachine; //�� ���������� �´� ���� ���� ���� 
    public AudioClip[] moodSoundGroup;

    [SerializeField] int purposeBgmIndex; //����Ǿ�� �ϴ� bgm ���� ��ȣ
    [SerializeField] int curBgmIndex; //���� ����ǰ� �ִ� bgm ������ ��ȣ(0~10)

    void Awake() 
    {
        DontDestroyOnLoad(gameObject);
        string filePath = Application.persistentDataPath + saveFileSeqName; 
        if (File.Exists(filePath)) //세이브파일이 존재하면 
        {
            string FromJsonData = File.ReadAllText(filePath);
            saveFileSeq = JsonUtility.FromJson<SaveFileSeq>(FromJsonData); //svaeFileSeq �� �״�� �޾ƿ� 
        }
        else //���� �������� ���� �� 
        {
            saveFileSeq = new SaveFileSeq(); //���� �ϳ� ������ �� 
            saveFileSeq.saveFileSeqList = new List<int>();
        }

        gameData = new GameData();

        bgmMachine = gameObject.AddComponent<AudioSource>();
        moodMachine = gameObject.AddComponent<AudioSource>();
    }

    public void soundNumCheck() //���� ������ �� �� bgm�� ��µǾ�� �� �� üũ
    {
        int sceneNum = SceneManager.GetActiveScene().buildIndex; //���� �� ��ȣ

        //���� ��ġ�� scene�� ��ȣ�� ���� ��ǥ bgm index�� ��ȭ��
        if (sceneNum == 0) purposeBgmIndex = 9; //���θ޴� bgm�� index 9 
        else if (sceneNum == 1) purposeBgmIndex = 10; //������ �� bgm�� index 10 
        else if (2 <= sceneNum && sceneNum < 4) purposeBgmIndex = 0; //stage0
        else if ((4 <= sceneNum && sceneNum < 7) || (sceneNum == 26) || (sceneNum == 27) || (sceneNum == 30)) purposeBgmIndex = 1; //stage1
        else if ((7 <= sceneNum && sceneNum < 10) || (sceneNum == 28)) purposeBgmIndex = 2; //stage2
        else if ((10 <= sceneNum && sceneNum < 12) || (sceneNum == 29)) purposeBgmIndex = 3; //stage3
        else if (12 <= sceneNum && sceneNum < 14) purposeBgmIndex = 4; //stage4

        if(purposeBgmIndex != curBgmIndex) //���� ������� bgm �ε����� ��ǥ �ε����� �ٸ� ~> ���� �ٲ���ٴ� ���̹Ƿ� �ε��� �ٲ���� ��
        {
            StopCoroutine("soundManager"); //���� �ڷ�ƾ�� �� ������ ���� ���¿��� ���� ���� �ڷ�ƾ �����ϸ� ���� �ڷ�ƾ�� ������ ��
            StartCoroutine("soundManager");
        }
    } 
    IEnumerator soundManager() //Ʃ�丮��:0 ���� ���� ~> ��������1 bgm�� index=1 / ���Ӹ޴��� bgm index=9(10��°)
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

            //���� �ӵ��� volume�� �ٿ��� ���� ũ�� 0���� ����. 1���� ���������� 1�ʰ� �ɸ��� 0.5���� ���������� 0.5�ʰ� �ɸ�            
        }
              
        bgmMachine.clip = bgmGroup[purposeBgmIndex]; //bgm ���带 ��ǩ���� �°� ��ü     
        moodMachine.clip = moodSoundGroup[purposeBgmIndex];

        yield return new WaitForSeconds(1f); //��� ��ٷȴٰ�        
        bgmMachine.Play();
        moodMachine.Play();
   
        for (int index = 1; index <= 20; index++)
        {
            bgmMachine.volume = 0.05f * index;
            moodMachine.volume = bgmMachine.volume;
            yield return new WaitForSeconds(0.1f); //2�ʿ� ���� ���� ũ�� 1���� ����
        }

    }

    void Start()
    {
        //���� �����ϸ� mainMenu â ����
        SceneManager.LoadScene("MainMenu"); 
        
        //bgm ���� �ڵ� 
        purposeBgmIndex = 9; //main theme �� �ʱ갪�� �Ǿ�� ��
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

        //����� �۵��ϴ��� üũ�ϱ� ���� �ڵ� ~> ��� ��忡�� ����
        
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
    public void StartGame(bool isStartNew) //true �� �ָ� ���� �����ϴ� ���� false�� �ָ� ���� ���̺꿡�� �����ϴ� ���� 
    {
        if (!isStartNew)
        {

        }


        InitData(isLoadGame); //�ʱ�ȭ 
        shouldSpawnSavePoint = true;

        InputManager.instance.isInputBlocked = false; //�Է����� Ǯ�� 
        InputManager.instance.isJumpBlocked = false; //�������� Ǯ�� 
        Cursor.lockState = CursorLockMode.Locked; //�⺻������ ���콺 ��ױ� 

        if (!shouldStartAtSavePoint) 
        {
            SceneManager.LoadScene(nextScene); //ó�� �����ϴ� ���̹Ƿ� �� ������ nextScene=1
        }
        else SceneManager.LoadScene(gameData.respawnScene); //ó�� �����ϴ� ���� �ƴ϶�� ������ ��ҿ��� ���� 

        isStartWithFlipX = false;
    }
    */

    //���̺����� �տ� ������ �����ߴ��� ���� ����� 
    public KeyValuePair<int, int> GetSavedData(int saveFileNum)
    {
        string filePath = Application.persistentDataPath + gameDataFileNames[saveFileNum];
        if (File.Exists(filePath))
        {
            string FromJsonData = File.ReadAllText(filePath);
            GameData curGameData = JsonUtility.FromJson<GameData>(FromJsonData);
            return new KeyValuePair<int, int>(curGameData.finalStageNum, curGameData.finalAchievementNum);
        }
        return new KeyValuePair<int, int>(-1, -1); //��ο� ���̺������� ������ ~> '�� ����' �ؽ�Ʈ ǥ���ؾ� �� 
    }  

    /*
    // Case 1) ���θ޴����� New Game�� ������ �� ȣ���
    // Case 2) ���θ޴����� Load Game�� ������ �� ȣ��� 
    // Case 3) ���� ������ ������� �� ȣ���

    // Case 1�� savePoint�� �ƴ϶� �ʱ� ������ҿ��� ����. Case2,3�� savePoint���� ����
    public void InitData(bool isLoadGame) //�÷��̿� �ʿ��� ������ �ʱ�ȭ or �ҷ����� �Լ� 
    {
        // Case 1
        if (!shouldStartAtSavePoint) //��������0���� ����
        {
            // ���� ���� �� �ʿ��� ������ �ʱ�ȭ
            nextScene = firstScene; //castleEnterance �� 
            nextPos = firstStartPos; //�迡�� ������ ��ġ 
            nextGravityDir = Vector2.down; //�����ϸ� �Ʒ��� ������ �߷� ����
            nextState = Player.States.Walk; //�� ó�� ������ �� �÷��̾��� ���´� walk

            // ������ �� �ʿ��� ������ �ʱ�ȭ
            // ���� �������ڸ��� �����ٰ� �ٽ� �������� �� load�� �����ϵ���
            gameData.curAchievementNum = 0; //���� �ƹ��� ���̺�����Ʈ�� Ȱ��ȭ���� �ʾҴٸ� 0 
            gameData.curStageNum = 0; //castleEnterance�� ����������ȣ�� 0 
            gameData.respawnScene = nextScene;
            gameData.respawnPos = nextPos;
            gameData.respawnGravityDir = nextGravityDir;
            
            for(int i=0; i<8; i++)
            {
                for(int j=0; j<50; j++)
                {
                    gameData.savePointUnlock[i, j] = false; //��� ���̺�����Ʈ ��Ȱ��ȭ 
                }
            }
           
            //������ ���� 
            string ToJsonData = JsonUtility.ToJson(gameData);
            string filePath = Application.persistentDataPath + gameDataFileNames[curSaveFileNum];
            File.WriteAllText(filePath, ToJsonData);       
        }

        // Case 2
        else
        {
            // Case 2�� ���, Json ������ �����͵��� ��� GameData class�� �ҷ���
            if (isLoadGame) //�ҷ��� ���� �����Ͱ� ������ �ҷ���
            {
                string filePath = Application.persistentDataPath + gameDataFileNames[curSaveFileNum];
                string FromJsonData = File.ReadAllText(filePath);
                gameData = JsonUtility.FromJson<GameData>(FromJsonData);
            }
            // GameData class�� �����͵��� GameManager �����Ϳ� ����   

            // Case 3�� ��� ������ ��� �����ʹ� �ױ� ���� �����ϹǷ� ���� �ʱ�ȭ�� �ʿ� ���� 
            
        }
    }
    */

    // Save Point�� �������� ��, ���� �� ��, key ȹ���� �� ��� 
    public void SaveData(int savePointNum, int stageNum, Vector2 playerPos) 
        // ���̺�����Ʈ���� �̷������ ������ ����. SavePoint�� �۵��� �� ���� Json ���Ͽ� �����Ͱ� �����
    {
        gameData.respawnScene = SceneManager.GetActiveScene().buildIndex; //���� ������ �ٽ� ��Ȱ�ؾ� ��
        gameData.respawnPos = playerPos;

        gameData.curAchievementNum = savePointNum;
        gameData.curStageNum = stageNum;

        if(gameData.finalStageNum < stageNum)
        {
            gameData.finalStageNum = stageNum; //��� ���̺� �� ���������� finalStage���� �ռ� ������ finalStage ����
            gameData.finalAchievementNum = savePointNum; //���������� ���ŵǾ����� achievement Num�� ������ �����ؾ� �� 
        }else if(gameData.finalStageNum == stageNum)
        {
            if(gameData.finalAchievementNum < savePointNum)
            {
                gameData.finalAchievementNum = savePointNum;
                //���� ������������ �� ū achNum���� �̵��� �� ������ �� ex) (1,10) ~> (1,11)
                //���� ���������� �� �۴ٸ� finalAch���� ū achNum�� ���͵� ���� ex) (2,10) ~> (1,13)���� �̵��� ���� finalAchNum ����x 
            }
        }

        gameData.savePointUnlock[stageNum - 1, savePointNum - 1] = true; //�ش� ���̺�����Ʈ ����ߴٴ� ���� ����
        gameData.respawnGravityDir = Physics2D.gravity.normalized;
        
        // gameData class�� �����͵��� ��� Json ���Ͽ� ����
        string ToJsonData = JsonUtility.ToJson(gameData);
        string filePath = Application.persistentDataPath + gameDataFileNames[curSaveFileNum];
        File.WriteAllText(filePath, ToJsonData);
    }

    public void SaveSaveFileSeq()
    {
        string ToJsonData = JsonUtility.ToJson(saveFileSeq); //saveFileSeq �� json ���Ϸ� ���� 
        string filePath = Application.persistentDataPath + saveFileSeqName; //���ϰ�� ����
        File.WriteAllText(filePath, ToJsonData);
    }
    
}
