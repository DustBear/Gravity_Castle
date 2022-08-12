using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class mapOpenSensor : MonoBehaviour
{
    public GameObject mapCanvas;
    public GameObject map;
    public GameObject chapter_instruction;
    public Button[] stageButton;
    public GameObject iconGroup; //생성된 아이콘은 이 밑에 넣어서 정리 

    public Sprite okIcon; //현재 진행도로 빠른이동 가능한 세이브포인트 아이콘
    public Sprite noIcon; //현재 진행도상 빠른이동 불가능한 세이브포인트 아이콘

    //버튼 눌러서 키보드 위 아래로 스크롤하기 
    public Button Button_up;
    public Button Button_down;
    public float mapMaxY;
    public float mapMinY;

    public float mapScrollSpeed; //맵을 위 아래로 스크롤하는 속도

    [SerializeField]bool isSensorOn;
    [SerializeField]bool shouldMapMoveUp;
    [SerializeField]bool shouldMapMoveDown;
    [SerializeField] bool isScrollButtonDown; //스크롤 버튼 누르고 있을 땐 키보드 및 마우스 휠 입력 무시 

    public int selectedStageNum; //현재 선택된 스테이지: 1부터 시작 
    public int selectedSavePointNum; //현재 선택된 세이브포인트: 1부터 시작 
    public int savePointCount; //표시해야 하는 세이브포인트의 개수
    [SerializeField] Vector2 firstIconPos;
    public float iconGap;
    public GameObject stageIcon;
    public GameObject firstIconPosAnchor;
    public float maxIconCount; //모든 스테이지의 세이브포인트 개수 중 가장 높은 값 이용
    [SerializeField] List<GameObject> savePointIcon;

    public GameObject betaModeWindow;

    public int stage0SavePointCount; 
    void Start()
    {
        mapCanvas.SetActive(false); //시작하면 맵은 끄고 시작 
        shouldMapMoveDown = false;
        shouldMapMoveUp = false;
        selectedStageNum = 1; //맨 처음 선택된 스테이지는 1
        selectedSavePointNum = 1; //맨 처음 선택된 세이브포인트는 1

        firstIconPos = firstIconPosAnchor.transform.position;
        betaModeWindow.SetActive(false);
    }

    void iconInitiate()
    {
        savePointIcon = new List<GameObject>(); //세이브포인트 배열 초기화 

        for (int index=0; index<maxIconCount; index++)
        {
            savePointIcon.Add(Instantiate(stageIcon, new Vector3(firstIconPos.x + iconGap * index, firstIconPos.y, 0), Quaternion.identity, iconGroup.transform));
            savePointIcon[index].GetComponent<savePointIconButton>().savePointNum = index+1; //세이브포인트 번호는 1부터 시작 

            //최대 아이콘 개수만큼 아이콘 생성한 뒤 savePointIcon 그룹에 집어넣음          
        }
    }

    public void iconMake() //스테이지 번호가 바뀔 때 해당 스테이지의 세이브포인트 개수에 맞게 스테이지 아이콘 활성화~>stageNum 바뀔 때 마다 호출 
    {               
        for (int index = 0; index < maxIconCount; index++)
        {
            if (index < savePointCount) //maxIconCount 개의 아이콘 중 앞에서부터 savePointCount개의 아이콘만 활성화하고 나머지는 비활성화
            {              
                savePointIcon[index].SetActive(true);
                savePointIcon[index].GetComponent<Image>().sprite = okIcon;

                if (GameManager.instance.gameData.finalStageNum == selectedStageNum)
                {
                    if(!GameManager.instance.gameData.savePointUnlock[selectedStageNum-1,selectedSavePointNum]) //해당 세이브포인트를 아직 활성화하지 않았으면 
                    {
                        savePointIcon[index].GetComponent<Image>().sprite = noIcon;
                    }                   
                }
            }
            else
            {
                savePointIcon[index].SetActive(false);
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
    
    void Update()
    {
        //센서 작동기능
        if(isSensorOn && Input.GetKeyDown(KeyCode.E))
        {
            mapCanvas.SetActive(true);

            if (GameManager.instance.gameData.mapStageNum < GameManager.instance.gameData.finalStageNum)
            {
                GameManager.instance.gameData.mapStageNum = GameManager.instance.gameData.finalStageNum;
                Debug.Log("지도 갱신");
            }
            
            InputManager.instance.isInputBlocked = true; //맵 메뉴 열려있는 동안은 플레이어 움직일 수 없음

            //스테이지 세이브포인트 아이콘 초기화 및 생성은 맵을 열 때마다 실행 
            
            iconInitiate();
            iconMake();
            savePointCount = stage0SavePointCount;
        }

        if(isSensorOn && Input.GetKeyDown(KeyCode.Q))
        {
            if (betaModeWindow.activeSelf == true)
            {
                betaModeWindow.SetActive(false);
            }
            else
            {
                mapCanvas.SetActive(false);
                InputManager.instance.isInputBlocked = false;
                Cursor.lockState = CursorLockMode.Locked; //마우스 조작 불가능
            }          
        }

        mapMove();
        InputCheck();
        iconInputCheck();

        if (mapCanvas.activeSelf)
        {
            Cursor.lockState = CursorLockMode.None;
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
            if (selectedSavePointNum > GameManager.instance.gameData.finalAchievementNum)
            {
                Debug.Log("not enough achievement");
                return;
            }
        }

        Debug.Log("savePointBackUp: " + (selectedSavePointNum));

        GameManager.instance.gameData.curAchievementNum = selectedSavePointNum; 
        GameManager.instance.gameData.curStageNum = selectedStageNum;

        /*
        if (GameManager.instance.gameData.finalStageNum < selectedStageNum)
        {
            GameManager.instance.gameData.finalStageNum = selectedStageNum; //방금 세이브 한 스테이지가 finalStage보다 앞서 있으면 finalStage 갱신
            GameManager.instance.gameData.finalAchievementNum = selectedSavePointNum; //스테이지가 갱신되었으면 achievement Num은 무조건 갱신해야 함 
        }
        else if (GameManager.instance.gameData.finalStageNum == selectedStageNum)
        {
            if (GameManager.instance.gameData.finalAchievementNum < selectedSavePointNum)
            {
                GameManager.instance.gameData.finalAchievementNum = selectedSavePointNum;
                //동일 스테이지에서 더 큰 achNum으로 이동할 때 갱신해 줌 ex) (1,10) ~> (1,11)
                //만약 스테이지가 더 작다면 finalAch보다 큰 achNum이 나와도 무시 ex) (2,10) ~> (1,13)으로 이동할 때는 finalAchNum 갱신x 
            }
        }
        */

        GameManager.instance.nextScene = tmpScript.savePointScene[selectedSavePointNum-1]; //선택된 씬으로 이동
        GameManager.instance.nextPos = new Vector2(0, 0);
        GameManager.instance.nextGravityDir = new Vector2(0, -1);
        //어차피 해당 씬으로 이동한 뒤 다시 재조정해 줄 테니 지금 신경쓸 필요x


        GameManager.instance.shouldStartAtSavePoint = true;
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

        SceneManager.LoadScene(GameManager.instance.nextScene);
    }

    public void betaModeActive() //진행도를 최대치로 높여주는 시스템
    {
        Debug.Log("beta-mode activated");

        GameManager.instance.gameData.finalAchievementNum = 13;
        GameManager.instance.gameData.finalStageNum = 4;

        for (int i = 0; i < 8; i++)
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

    void mapMove()
    {
        Vector2 mapPos = map.GetComponent<RectTransform>().position;
        if (shouldMapMoveUp)
        {
            if (mapPos.y <= mapMinY + 540) //맵 위치가 최저 기준선보다 낮아지면 정지
            {                
                return;
            }
            map.GetComponent<RectTransform>().position = new Vector2(mapPos.x, mapPos.y - mapScrollSpeed * Time.deltaTime);
        }
        if (shouldMapMoveDown)
        {            
            if (mapPos.y >= mapMaxY + 540) //맵 위치가 최고 기준선보다 높아지면 정지
            {
                return;
            }
            
            map.GetComponent<RectTransform>().position = new Vector2(mapPos.x, mapPos.y + mapScrollSpeed * Time.deltaTime);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            isSensorOn = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            isSensorOn = false;
        }
    }

    //화면상 버튼 눌러서 작동 
    public void mapScroll_up() 
    {
        isScrollButtonDown = true;
        shouldMapMoveUp = true;
    }
    public void mapScroll_down()
    {
        isScrollButtonDown = true;
        shouldMapMoveDown = true;
    }
    public void mapScrollPause()
    {
        isScrollButtonDown = false;
        shouldMapMoveUp = false;
        shouldMapMoveDown = false;
    }

    
    public void InputCheck()
    {
        if (isScrollButtonDown)
        {
            return;
        }

        //마우스 커서가 지도 위에 있는지 감지
        float mouseXpos = Input.mousePosition.x;
        float mapMinXpos = map.GetComponent<RectTransform>().position.x - map.GetComponent<RectTransform>().rect.width / 2;
        float mapMaxXpos = map.GetComponent<RectTransform>().position.x + map.GetComponent<RectTransform>().rect.width / 2;

        //bool isMouseOnMap = (mapMinXpos < mouseXpos) && (mouseXpos < mapMaxXpos);

        if (Input.GetKey(KeyCode.UpArrow))
        {
            shouldMapMoveUp = true; //위 화살표를 누르거나 휠을 위쪽으로 돌리면 맵 올려줌
        }else if (Input.GetKey(KeyCode.DownArrow))
        {
            shouldMapMoveDown = true; //아래 화살표를 누르거나 휠을 아래쪽으로 돌리면 맵 내려줌 
        }else
        {
            shouldMapMoveUp = false;
            shouldMapMoveDown = false;
        }
    }
    

    
}