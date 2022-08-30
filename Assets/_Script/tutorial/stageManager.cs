using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class stageManager : MonoBehaviour
{    
    public Button[] stageButton;
    public GameObject iconGroup; //생성된 아이콘은 이 밑에 넣어서 정리 
    public GameObject betaModeWindow;

    public int selectedStageNum; //현재 선택된 스테이지: 1부터 시작 
    public int selectedSavePointNum; //현재 선택된 세이브포인트: 1부터 시작 
    public int savePointCount; //표시해야 하는 세이브포인트의 개수

    Vector2 firstIconPos;
    public float iconGap;
    public GameObject stageIcon;
    public GameObject firstIconPosAnchor;
    public Sprite okIcon; //현재 진행도로 빠른이동 가능한 세이브포인트 아이콘
    public Sprite noIcon; //현재 진행도상 빠른이동 불가능한 세이브포인트 아이콘
    public float maxIconCount; //모든 스테이지의 세이브포인트 개수 중 가장 높은 값 이용
    [SerializeField] List<GameObject> savePointIcon;

    //public GameObject betaModeWindow;

    void Start()
    {       
        selectedStageNum = GameManager.instance.gameData.curStageNum; //마지막에 플레이하던 스테이지가 기본으로 선택됨 
        selectedSavePointNum = GameManager.instance.gameData.curAchievementNum; //마지막에 플레이하던 세이브포인트가 기본으로 선택됨 

        firstIconPos = firstIconPosAnchor.transform.position;

        iconInitiate();
        iconMake();
       
        //스테이지 세이브포인트 아이콘 초기화 및 생성은 맵을 열 때마다 실행 
    }

    void iconInitiate()
    {
        savePointIcon = new List<GameObject>(); //세이브포인트 배열 초기화 

        for (int index=0; index<maxIconCount; index++)
        {
            savePointIcon.Add(Instantiate(stageIcon, new Vector3(firstIconPos.x + iconGap * index, firstIconPos.y, 0), Quaternion.identity, iconGroup.transform));
            savePointIcon[index].GetComponent<savePointIconButton>().savePointNum = index+1; //세이브포인트 번호는 1부터 시작 

            //최대 아이콘 개수만큼 아이콘 생성한 뒤 iconGroup 그룹에 집어넣음          
        }
    }

    public void iconMake() //스테이지 번호가 바뀔 때 해당 스테이지의 세이브포인트 개수에 맞게 스테이지 아이콘 활성화~>씬 처음 시작할 때, stageNum 바뀔 때 마다 호출 
    {
        Debug.Log(selectedStageNum + ", " + selectedSavePointNum);

        for (int index = 0; index < maxIconCount; index++)
        {
            if (index < savePointCount) //maxIconCount 개의 아이콘 중 앞에서부터 savePointCount개의 아이콘만 활성화하고 나머지는 비활성화
            {              
                //일단 전부 활성화 
                savePointIcon[index].SetActive(true);
                savePointIcon[index].GetComponent<Image>().sprite = okIcon;

                if (!GameManager.instance.gameData.savePointUnlock[selectedStageNum - 1, selectedSavePointNum-1]) //해당 세이브포인트를 아직 활성화하지 않았으면 
                {
                    savePointIcon[index].GetComponent<Image>().sprite = noIcon;
                }
            }
            else
            {
                savePointIcon[index].SetActive(false); //전부 사용하고 남은 아이콘은 비활성화 
            }
        }
        iconCheck();
    }

    public void iconInputCheck() //키보드 입력에 따라 현재 선택해야 하는 세이브포인트 번호 지정
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (selectedSavePointNum < savePointCount)
            {
                selectedSavePointNum++;
                iconCheck();
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (selectedSavePointNum > 1)
            {
                selectedSavePointNum--;
                iconCheck();
            }
        }
    }

    public void iconCheck() //현재 커서가 표시되어 있는 세이브포인트에 맞게 아이콘 색깔 표시~>스테이지 번호 바뀔 때, 동일 스테이지에서 세이브포인트 번호 바뀔 때 호출 
    {
        for(int index=1; index<=savePointCount; index++)
        {
            if(index == selectedSavePointNum)
            {
                savePointIcon[index-1].GetComponent<Image>().color = new Color(1, 1, 1, 1);
            }
            else
            {
                savePointIcon[index-1].GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 1);
            }
            
        }
    }

    public void backToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
    
    void Update()
    {       
        iconInputCheck(); //키보드 좌우 화살표 눌러서 현재 선택된 세이브포인트 번호 이동 
        if(betaModeWindow.activeSelf && Input.GetKeyDown(KeyCode.Q))
        {
            betaModeWindow.SetActive(false);
        }
    }
    public void ChapterStart() //챕터 시작하기 버튼을 누르면 실행되는 함수 
    {
        UIManager.instance.clickSoundGen();

        GameObject stageButtonTmp = stageButton[selectedStageNum-1].gameObject;
        stageMoveButton tmpScript = stageButtonTmp.GetComponent<stageMoveButton>();

        if (GameManager.instance.gameData.finalStageNum == selectedStageNum)
        {
            //만약 이동하고자 하는 세이브포인트 숫자가 성취도보다 높으면 이동x 
            //성취도보다 높은 스테이지는 애초에 클릭이 불가능하므로 굳이 고려할 필요x
            if (!GameManager.instance.gameData.savePointUnlock[selectedStageNum-1, selectedSavePointNum-1])
            {
                Debug.Log("savePoint not activated");
                return;
            }
        }

        Debug.Log("savePointBackUp: " + (selectedSavePointNum));

        GameManager.instance.gameData.curAchievementNum = selectedSavePointNum; 
        GameManager.instance.gameData.curStageNum = selectedStageNum;

        GameManager.instance.nextScene = tmpScript.savePointScene[selectedSavePointNum-1]; //선택된 씬으로 이동
        GameManager.instance.nextPos = new Vector2(0, 0);
        GameManager.instance.nextGravityDir = new Vector2(0, -1);
        //어차피 해당 씬으로 이동한 뒤 다시 재조정해 줄 테니 지금 신경쓸 필요x


        GameManager.instance.shouldSpawnSavePoint = true;
        GameManager.instance.nextState = Player.States.Walk;

        //gameData 초기화
        GameManager.instance.gameData.respawnScene = GameManager.instance.nextScene;
        GameManager.instance.gameData.respawnPos = GameManager.instance.nextPos;
        GameManager.instance.gameData.respawnGravityDir = GameManager.instance.nextGravityDir;

        Cursor.lockState = CursorLockMode.Locked;
        InputManager.instance.isInputBlocked = false;

        string ToJsonData = JsonUtility.ToJson(GameManager.instance.gameData);
        string filePath = Application.persistentDataPath + GameManager.instance.gameDataFileNames[GameManager.instance.curSaveFileNum];
        File.WriteAllText(filePath, ToJsonData);

        GameManager.instance.shouldSpawnSavePoint = true; //빠른이동은 무조건 세이브포인트로만 이동
        SceneManager.LoadScene(GameManager.instance.nextScene);
    }

    
    public void betaModeActive() //진행도를 최대치로 높여주는 시스템
    {
        Debug.Log("beta-mode activated");

        GameManager.instance.gameData.finalAchievementNum = stageButton[2].GetComponent<stageMoveButton>().savePointCount;
        GameManager.instance.gameData.finalStageNum = 3;

        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < 50; j++)
            {
                GameManager.instance.gameData.savePointUnlock[i, j] = true; //모든 세이브포인트 활성화 
            }
        }
        string ToJsonData = JsonUtility.ToJson(GameManager.instance.gameData);
        string filePath = Application.persistentDataPath + GameManager.instance.gameDataFileNames[GameManager.instance.curSaveFileNum];
        File.WriteAllText(filePath, ToJsonData);

        betaModeWindow.SetActive(false);
    }

    public void betaModeWindowOpen()
    {
        betaModeWindow.SetActive(true);
    }
   
}