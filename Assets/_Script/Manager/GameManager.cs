using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

// GameManager�� ���� scene���� �̵� �� �����ؾ��� data�� ����
public class GameManager : Singleton<GameManager>
{
    // 1) ��� Stage ����
    [HideInInspector] public bool shouldStartAtSavePoint {get; set;}
    public int nextScene {get; set;}
    public Vector2 nextPos {get; set;}
    public Vector2 nextGravityDir {get; set;}
    public Player.States nextState {get; set;}

    public bool isStartWithFlipX; //�÷��̾ ���� ������ �� ��� ���� �ٶ���� �ϴ��� ����

    // 2) Stage5�� ������ �������� floor 
    [SerializeField] int shakedFloorNum = 36;
    [HideInInspector] public bool[] curIsShaked {get; set;}

    // 3) Stage6�� ice 
    [SerializeField] int iceNum = 58;
    [HideInInspector] public bool[] curIsMelted {get; set;}

    // 4) Stage6�� �÷��̾� detect ����
    [SerializeField] int detectorNum = 6;
    [HideInInspector] public bool[] curIsDetected {get; set;}

    // 5) Stage6�� button ����
    [SerializeField] int buttonNum = 11;
    [HideInInspector] public bool[] curIsGreen {get; set;}
    [HideInInspector] public Vector2[] curPos {get; set;}

    // 6) Stage8�� ���� Ȯ�� ����
    [HideInInspector] public bool isCliffChecked = false;

    const string GameDataFileName = "GameData.json";
    public GameData gameData {get; private set;}

    [SerializeField] Vector2 firstStartPos; // ���� ���� ��ġ
    [SerializeField] Vector2 firstRespawnPos; // ���� �������ڸ��� ������ ���⼭ ��Ȱ
    
    void Awake() {
        DontDestroyOnLoad(gameObject); //�� �Ѿ�� �ı�x

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
        //���� �����ϸ� mainMenu â ����
        SceneManager.LoadScene("MainMenu"); 
        Debug.Log("GM start");
    }

    public void StartGame(bool isLoadGame)
    {
        InitData(isLoadGame); //�ʱ�ȭ 

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

    public void CheckSavedGame(bool isLoadGame) //���� �� ������ �� �Լ��� ����
    {
        string filePath = Application.persistentDataPath + GameDataFileName;
        // New Game�� ���
        if (!isLoadGame)
        {
            if (File.Exists(filePath))
            {
                UIManager.instance.ExistSavedGame(); //���� ���������� �����մϴ� �޽��� ��� 
            }
            else
            {
                StartGame(isLoadGame); //���� ������ �ٷ� �� ���� ����
            }
        }
        // Load Game�� ���
        else
        {
            if (File.Exists(filePath))
            {
                shouldStartAtSavePoint = true; //��ο� ������ ������ ��� true
                StartGame(isLoadGame);
            }
            else
            {
                UIManager.instance.NoSavedGame(); //���� �ҷ����� �ߴµ� ���� ��ο� �ƹ��͵� ���� ��쿡 ���޽��� ��� 
            }
        }
    }

    // Case 1) ���θ޴����� New Game�� ������ �� ȣ���
    // Case 2) ���θ޴����� Load Game�� ������ �� ȣ��� 
    // Case 3) ���� ������ ������� �� ȣ���

    // Case 1�� savePoint�� �ƴ϶� �ʱ� ������ҿ��� ����. Case2,3�� savePoint���� ����
    public void InitData(bool isLoadGame) //�÷��̿� �ʿ��� ������ �ʱ�ȭ or �ҷ����� �Լ� 
    {
        // Case 1
        if (!shouldStartAtSavePoint) //��������1���� ����
        {
            // ���� ���� �� �ʿ��� ������ �ʱ�ȭ
            nextScene = 1;
            nextPos = firstStartPos;
            nextGravityDir = Vector2.down; //�����ϸ� �Ʒ��� ������ �߷� ����
            nextState = Player.States.Walk; //�� ó�� ������ �� �÷��̾��� ���´� walk

            // �⺻������ false�� �ʱ�ȭ �Ǿ� �ֱ� ������ �ּ� ó��
            // nextIsJumpBlocked = false;
            // for (int i = 0; i < shakedFloorNum; i++) curIsShaked[i] = false;
            // for (int i = 0; i < iceNum; i++) curIsMelted[i] = false;
            // for (int i = 0; i < detectorNum; i++) curIsDetected[i] = false;
            // for (int i = 0; i < buttonNum; i++) curIsGreen[i] = false;
            for (int i = 0; i < buttonNum; i++) curPos[i] = Vector2.zero;

            // ������ �� �ʿ��� ������ �ʱ�ȭ
            // ���� �������ڸ��� �����ٰ� �ٽ� �������� �� load�� �����ϵ���
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

            //������ ���� 
            string ToJsonData = JsonUtility.ToJson(gameData);
            string filePath = Application.persistentDataPath + GameDataFileName;
            File.WriteAllText(filePath, ToJsonData);       
        }

        // Case 2, 3
        else
        {
            // Case 2�� ���, Json ������ �����͵��� ��� GameData class�� �ҷ���
            if (isLoadGame) //�ҷ��� ���� �����Ͱ� ������ �ҷ���
            {
                string filePath = Application.persistentDataPath + GameDataFileName;
                string FromJsonData = File.ReadAllText(filePath);
                gameData = JsonUtility.FromJson<GameData>(FromJsonData);
            }
            // GameData class�� �����͵��� GameManager �����Ϳ� ����
            isCliffChecked = gameData.isCliffChecked;
            gameData.storedIsShaked.CopyTo(curIsShaked, 0);
            gameData.storedIsMelted.CopyTo(curIsMelted, 0);
            gameData.storedIsDetected.CopyTo(curIsDetected, 0);
            gameData.storedIsGreen.CopyTo(curIsGreen, 0);
            gameData.storedPos.CopyTo(curPos, 0);
        }
    }

    // Save Point�� �������� �� ȣ���
    public void SaveData(int achievementNum, int stageNum, Vector2 playerPos) 
        //���� ���̺�� ���̺�����Ʈ���� �̷����. SavePoint�� ���� �� ���� Json ���Ͽ� �����Ͱ� �����
    {        
        gameData.respawnScene = SceneManager.GetActiveScene().buildIndex; //���� ������ �ٽ� ��Ȱ�ؾ� ��
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
        
        // gameData class�� �����͵��� ��� Json ���Ͽ� ����
        string ToJsonData = JsonUtility.ToJson(gameData);
        string filePath = Application.persistentDataPath + GameDataFileName;
        File.WriteAllText(filePath, ToJsonData);
    }
}