using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class stageManager : MonoBehaviour
{    
    public Button[] stageButton;
    public GameObject iconGroup; 
    public GameObject betaModeWindow;

    [SerializeField] public int selectedStageNum; //현재 선택된 스테이지 
    [SerializeField] public int selectedSavePointNum; //현재 선택된 세이브포인트 
    public int savePointCount; //이 스테이지의 전체 세이브포인트 개수 

    [SerializeField] Vector2 firstIconPos;
    public float iconGap;
    public GameObject stageIcon;
    public GameObject firstIconPosAnchor; //첫 세이브포인트 아이콘이 생성될 위치 
    public Sprite okIcon; //세이브포인트가 활성화됐을 때의 아이콘 
    public Sprite noIcon; //세이브포인트가 비활성화됐을 때의 아이콘 

    [SerializeField] List<GameObject> savePointIcon;
    [SerializeField] GameObject selectedStageButton;

    public GameObject chapterStartButton;
    bool isStartButtonShake = false;

    void Start()
    {
        UIManager.instance.FadeIn(1f);

        selectedStageNum = GameManager.instance.gameData.curStageNum; //이전에 플레이하던 stage에서 시작해야 함 ( 1부터 시작 ) 
        selectedStageButton = stageButton[selectedStageNum - 1].gameObject;
        selectedSavePointNum = 1;

        savePointCount = selectedStageButton.GetComponent<stageMoveButton>().savePointCount;

        firstIconPos = firstIconPosAnchor.transform.position;

        iconMake();
    }



    public void iconMake() //선택된 스테이지 번호에 맞게 알맞은 개수의 세이브포인트 아이콘을 생성해야 함 
    {                
        savePointIcon = new List<GameObject>(); //세이브포인트 아이콘 그룹 초기화 
        for(int index=0; index<iconGroup.transform.childCount; index++)
        {
            Destroy(iconGroup.transform.GetChild(index).gameObject); //iconGroup에 들어있던 모든 saveIconButton 제거 
        }

        selectedStageButton = stageButton[selectedStageNum - 1].gameObject;
        for (int index = 0; index < selectedStageButton.GetComponent<stageMoveButton>().savePointCount; index++)
        {
            GameObject temp = Instantiate(stageIcon); //세이브포인트 아이콘 생성 
            savePointIcon.Add(temp);

            temp.transform.position = firstIconPos + new Vector2(iconGap * index, 0);
            temp.GetComponent<savePointIconButton>().savePointNum = index + 1;
            temp.transform.SetParent(iconGroup.transform); //아이콘 그룹에 넣어서 정리 

            temp.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);

            if(GameManager.instance.gameData.savePointUnlock[GameManager.instance.saveNumCalculate(new Vector2(selectedStageNum, index + 1))] == 1) //해당 세이브포인트가 활성화됐는지 검사 
            {
                savePointIcon[index].GetComponent<Image>().sprite = okIcon;
            }
            else
            {
                savePointIcon[index].GetComponent<Image>().sprite = noIcon;
            }
        }
        
        iconCheck();
    }

    public void iconCheck() //현재 선택된 세이브포인트의 번호에 맞게 색상 조정 
    {
        for (int index = 1; index <= savePointCount; index++)
        {
            if (index == selectedSavePointNum)
            {
                savePointIcon[index - 1].GetComponent<Image>().color = new Color(1, 1, 1, 1);
            }
            else
            {
                savePointIcon[index - 1].GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 1);
            }

        }
    }

    public void iconInputCheck() //좌우 화살표 조작을 통해 선택중인 세이브포인트 번호를 바꿀 수 있도록 함 
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

    public void backToMainMenu() //메인메뉴로 돌아가는 함수 
    {
        SceneManager.LoadScene("MainMenu");
    }
    
    void Update()
    {       
        iconInputCheck();

        //베타모드 실행창이 켜져 있으면 Q 눌러서 끌 수 있음 
        if(betaModeWindow.activeSelf && Input.GetKeyDown(KeyCode.Q))
        {
            betaModeWindow.SetActive(false);
        }
    }
    public void ChapterStart() //선택한 스테이지, 세이브포인트 시작하는 버튼 
    {
        UIManager.instance.clickSoundGen();

        GameObject stageButtonTmp = stageButton[selectedStageNum-1].gameObject;
        stageMoveButton tmpScript = stageButtonTmp.GetComponent<stageMoveButton>();

        /*
        if (GameManager.instance.gameData.finalStageNum == selectedStageNum)
        {
            //선택한 스테이지의 세이브포인트 넘버가 현재 성취도보다 낮으면 이동 불가능 
            //만약 현재 스테이지가 finalStage보다 낮으면 애초에 클릭이 불가능하기에 고려 x 
            if (!GameManager.instance.gameData.savePointUnlock[selectedStageNum-1, selectedSavePointNum-1])
            {
                Debug.Log("savePoint not activated");
                return;
            }
        }
        */

        if (GameManager.instance.gameData.savePointUnlock[GameManager.instance.saveNumCalculate(new Vector2(selectedStageNum, selectedSavePointNum))] == 0)
        {
            if (!isStartButtonShake)
            {
                StartCoroutine(chapterStartShake());
            }
            return;
        }

        //gameData 바꿔 줌 
        GameManager.instance.gameData.curAchievementNum = selectedSavePointNum; 
        GameManager.instance.gameData.curStageNum = selectedStageNum;
        GameManager.instance.gameData.savePointUnlock[GameManager.instance.saveNumCalculate(new Vector2(selectedStageNum, selectedSavePointNum))] = 1;

        GameManager.instance.nextScene = tmpScript.savePointScene[selectedSavePointNum-1]; 
        GameManager.instance.nextPos = new Vector2(0, 0);
        GameManager.instance.nextGravityDir = new Vector2(0, -1);
        //nextPos와 nextGravityDIr은 어차피 다음 씬에 도착하면 savePointLoad 가 알아서 조정해 줌 

        GameManager.instance.nextState = Player.States.Walk;

        GameManager.instance.gameData.respawnScene = GameManager.instance.nextScene;
        GameManager.instance.gameData.respawnPos = GameManager.instance.nextPos;
        GameManager.instance.gameData.respawnGravityDir = GameManager.instance.nextGravityDir;

        Cursor.lockState = CursorLockMode.Locked;
        InputManager.instance.isInputBlocked = false;

        //게임데이터 저장 
        string ToJsonData = JsonUtility.ToJson(GameManager.instance.gameData);
        string filePath = Application.persistentDataPath + GameManager.instance.gameDataFileNames[GameManager.instance.curSaveFileNum];
        File.WriteAllText(filePath, ToJsonData);

        GameManager.instance.shouldSpawnSavePoint = true; //세이브포인트에서 시작해야 함 
        GameManager.instance.shouldUseOpeningElevator = false;
        SceneManager.LoadScene(GameManager.instance.nextScene);
    }

    IEnumerator chapterStartShake()
    {
        isStartButtonShake = true;

        chapterStartButton.transform.position += new Vector3(0.1f, 0, 0);
        yield return new WaitForSeconds(0.1f);
        chapterStartButton.transform.position += new Vector3(-0.1f, 0, 0);
        yield return new WaitForSeconds(0.1f);
        chapterStartButton.transform.position += new Vector3(-0.1f, 0, 0);
        yield return new WaitForSeconds(0.1f);
        chapterStartButton.transform.position += new Vector3(0.1f, 0, 0);
        yield return new WaitForSeconds(0.1f);

        isStartButtonShake = false;
    }
    
    public void betaModeActive() //개발용 베타테스트 버튼 ~> 정식출시 버전에선 삭제해야 함
    {
        Debug.Log("beta-mode activated");

        GameManager.instance.gameData.finalAchievementNum = stageButton[2].GetComponent<stageMoveButton>().savePointCount;
        GameManager.instance.gameData.finalStageNum = 3;

        for(int index=0; index< GameManager.instance.gameData.savePointUnlock.Length; index++)
        {
            GameManager.instance.gameData.savePointUnlock[index] = 1; //모든 세이브포인트 활성화 
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