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

    public Sprite[] chapterImage;

    //각 스테이지별 세이브포인트 이미지 
    public Sprite[] savePointImage_1;
    public Sprite[] savePointImage_2;
    public Sprite[] savePointImage_3;
    public Sprite[] savePointImage_4;
    public Sprite[] savePointImage_5;
    public Sprite savePointLock;

    public Image chapterInstruction;
    public Text chapterName;
    public Image saveInstruction;

    void Start()
    {
        UIManager.instance.FadeIn(1.5f);

        selectedStageNum = GameManager.instance.gameData.curStageNum; //이전에 플레이하던 stage에서 시작해야 함 ( 1부터 시작 ) 
        selectedStageButton = stageButton[selectedStageNum - 1].gameObject;
        selectedSavePointNum = GameManager.instance.gameData.curAchievementNum;


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

            temp.transform.position = firstIconPos + new Vector2(iconGap * (index % 15) , -index/15); //아이콘 생성할 때 10줄 단위로 엔터 침
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
        for (int index = 1; index <= selectedStageButton.GetComponent<stageMoveButton>().savePointCount; index++)
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

    public void stageButtonCheck()
    {
        for(int index=0; index<stageButton.Length; index++)
        {
            if(index == selectedStageNum - 1)
            {
                stageButton[index].GetComponent<Image>().color = new Color(1, 1, 1, 1);
            }
            else
            {
                stageButton[index].GetComponent<Image>().color = new Color(1, 1, 1, 0.6f);
            }
        }
    }

    public void iconInputCheck() //좌우 화살표 조작을 통해 선택중인 세이브포인트 번호, 스테이지 번호를 바꿀 수 있도록 함 
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            UIManager.instance.clickSoundGen();
            if (selectedSavePointNum < selectedStageButton.GetComponent<stageMoveButton>().savePointCount)
            {
                selectedSavePointNum++;
                iconCheck();
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            UIManager.instance.clickSoundGen();
            if (selectedSavePointNum > 1)
            {
                selectedSavePointNum--;
                iconCheck();
            }
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            int curSelectedStageNum = selectedStageNum; //현재의 스테이지 번호 임시저장
            if(selectedStageNum < 8)
            {
                while (true) //만약 다음 스테이지가 비활성화상태이면 그 다음 스테이지로 넘어가야 함 
                {
                    if(selectedStageNum == 8) //마지막 스테이지에 도달했는데 이것도 비활성화 상태일 때
                    {
                        selectedStageNum = curSelectedStageNum;
                        break;
                    }

                    selectedStageNum++;
                    if (stageButton[selectedStageNum - 1].GetComponent<stageMoveButton>().isActive)
                    {
                        UIManager.instance.clickSoundGen();
                        selectedSavePointNum = 1;
                        break;
                    }
                }               

                iconMake();
                iconCheck();
            }
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            int curSelectedStageNum = selectedStageNum; //현재의 스테이지 번호 임시저장
            if (selectedStageNum > 1)
            {
                while (true) //만약 다음 스테이지가 비활성화상태이면 그 다음 스테이지로 넘어가야 함 
                {
                    if (selectedStageNum == 1) //맨 처음 스테이지에 도달했는데 이것도 비활성화 상태일 때
                    {
                        selectedStageNum = curSelectedStageNum;
                        break;
                    }

                    selectedStageNum--;
                    if (stageButton[selectedStageNum - 1].GetComponent<stageMoveButton>().isActive)
                    {
                        UIManager.instance.clickSoundGen();
                        selectedSavePointNum = 1;
                        break;
                    }
                }
                
                iconMake();
                iconCheck();
            }
        }
    }

    public void backToMainMenu() //메인메뉴로 돌아가는 함수 
    {
        UIManager.instance.FadeOut(1f);
        Invoke("backToMainMenuInvoke", 1.5f);
    }

    void backToMainMenuInvoke()
    {
        SceneManager.LoadScene("MainMenu");
    }
    
    void Update()
    {       
        iconInputCheck();
        instructionCheck();
        stageButtonCheck();

        //베타모드 실행창이 켜져 있으면 Q 눌러서 끌 수 있음 
        if(betaModeWindow.activeSelf && Input.GetKeyDown(KeyCode.Q))
        {
            betaModeWindow.SetActive(false);
        }
    }

    void instructionCheck()
    {
        if(chapterInstruction.sprite != chapterImage[selectedStageNum - 1])
        {
            chapterInstruction.sprite = chapterImage[selectedStageNum - 1];
        }

        if(chapterName.text != gameTextManager.instance.stageNameManager(selectedStageNum))
        {
            chapterName.text = gameTextManager.instance.stageNameManager(selectedStageNum);
        }

        Sprite[] tmpArray;
        switch (selectedStageNum)
        {
            case 1:
                tmpArray = savePointImage_1;
                break;
            case 2:
                tmpArray = savePointImage_2;
                break;
            case 3:
                tmpArray = savePointImage_3;
                break;
            case 4:
                tmpArray = savePointImage_4;
                break;
            case 5:
                tmpArray = savePointImage_5;
                break;
            default:
                tmpArray = null;
                break;
        }

        if (tmpArray != null)
        {
            if (GameManager.instance.gameData.savePointUnlock[GameManager.instance.saveNumCalculate(new Vector2(selectedStageNum, selectedSavePointNum))] == 1)
            {
                //해당 세이브파일이 이미 언락된 상태일 때 
                saveInstruction.sprite = tmpArray[selectedSavePointNum - 1];
            }
            else
            {
                saveInstruction.sprite = savePointLock;
            }
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

        GameManager.instance.gameData.respawnScene = tmpScript.savePointScene[selectedSavePointNum-1]; 
        GameManager.instance.gameData.respawnPos = new Vector2(0, 0);
        GameManager.instance.gameData.respawnGravityDir = new Vector2(0, -1);
        //nextPos와 nextGravityDIr은 어차피 다음 씬에 도착하면 savePointLoad 가 알아서 조정해 줌 

        //챕터 시작 버튼 누를 경우 세이브포인트에서 시작해야 함
        GameManager.instance.gameData.SpawnSavePoint_bool = true;
        GameManager.instance.gameData.UseOpeningElevetor_bool = false;

        //게임데이터 저장 
        string ToJsonData = JsonUtility.ToJson(GameManager.instance.gameData);
        string filePath = Application.persistentDataPath + GameManager.instance.gameDataFileNames[GameManager.instance.curSaveFileNum];
        File.WriteAllText(filePath, ToJsonData);

        //당장 GM이 써야 하는 데이터는 초기화 해 줌 
        GameManager.instance.nextScene = GameManager.instance.gameData.respawnScene;
        GameManager.instance.nextPos = GameManager.instance.gameData.respawnPos;
        GameManager.instance.nextGravityDir = GameManager.instance.gameData.respawnGravityDir;
        GameManager.instance.nextState = Player.States.Walk;
        
        Cursor.lockState = CursorLockMode.Locked;
        InputManager.instance.isInputBlocked = false;

        UIManager.instance.FadeOut(1f);
        Invoke("chapterStart_invoke", 1.5f);
    }

    void chapterStart_invoke()
    {
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

        GameManager.instance.gameData.finalAchievementNum = 30;
        GameManager.instance.gameData.finalStageNum = 7;

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