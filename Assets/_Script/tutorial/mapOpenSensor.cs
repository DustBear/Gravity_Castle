using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class mapOpenSensor : MonoBehaviour
{
    public GameObject mapCanvas;
    public GameObject map;
    public GameObject chapter_instruction;
    public Button[] stageButton;
    public GameObject iconGroup; //생성된 아이콘은 이 밑에 넣어서 정리 

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

    public int selectedStageNum; //현재 선택된 스테이지
    public int selectedSavePointNum; //현재 선택된 세이브포인트 
    public int savePointCount; //표시해야 하는 세이브포인트의 개수
    [SerializeField] Vector2 firstIconPos;
    public float iconGap;
    public GameObject stageIcon;
    public GameObject firstIconPosAnchor;
    public float maxIconCount; //모든 스테이지의 세이브포인트 개수 중 가장 높은 값 이용
    [SerializeField] List<GameObject> savePointIcon;

    public int stage0SavePointCount; 
    void Start()
    {
        mapCanvas.SetActive(false); //시작하면 맵은 끄고 시작 
        shouldMapMoveDown = false;
        shouldMapMoveUp = false;
        selectedStageNum = 0; //맨 처음 선택된 스테이지는 0
        selectedSavePointNum = 0; //맨 처음 선택된 세이브포인트는 0

        firstIconPos = firstIconPosAnchor.transform.position;      
    }

    void iconInitiate()
    {
        savePointIcon = new List<GameObject>(); //세이브포인트 배열 초기화 

        for (int index=0; index<maxIconCount; index++)
        {
            savePointIcon.Add(Instantiate(stageIcon, new Vector3(firstIconPos.x + iconGap * index, firstIconPos.y, 0), Quaternion.identity, iconGroup.transform));
            savePointIcon[index].GetComponent<savePointIconButton>().savePointNum = index; //세이브포인트 번호는 1부터 시작 
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
            if (selectedSavePointNum < savePointCount - 1)
            {
                selectedSavePointNum++;
                iconCheck();
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (selectedSavePointNum >= 1)
            {
                selectedSavePointNum--;
                iconCheck();
            }
        }
    }

    public void iconCheck() //현재 커서가 표시되어 있는 세이브포인트에 맞게 아이콘 표시~>스테이지 번호 바뀔 때, 동일 스테이지에서 세이브포인트 번호 바뀔 때 호출 
    {
        for(int index=0; index<savePointCount; index++)
        {
            if(index == selectedSavePointNum)
            {
                savePointIcon[index].GetComponent<Image>().color = new Color(1, 1, 1, 1);
            }
            else
            {
                savePointIcon[index].GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 1);
            }
            
        }
    }
    
    void Update()
    {
        //센서 작동기능
        if(isSensorOn && Input.GetKeyDown(KeyCode.E))
        {
            mapCanvas.SetActive(true);
            InputManager.instance.isInputBlocked = true; //맵 메뉴 열려있는 동안은 플레이어 움직일 수 없음

            //스테이지 세이브포인트 아이콘 초기화 및 생성은 맵을 열 때마다 실행 
            
            iconInitiate();
            iconMake();
            savePointCount = stage0SavePointCount;
        }

        if(isSensorOn && Input.GetKeyDown(KeyCode.Q))
        {
            mapCanvas.SetActive(false);
            InputManager.instance.isInputBlocked = false; 
            Cursor.lockState = CursorLockMode.Locked; //마우스 조작 불가능
        }

        mapMove();
        InputCheck();
        iconInputCheck();

        if (mapCanvas.activeSelf)
        {
            Cursor.lockState = CursorLockMode.None;
        }
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

        bool isMouseOnMap = (mapMinXpos < mouseXpos) && (mouseXpos < mapMaxXpos);

        if (Input.GetKey(KeyCode.UpArrow) || (isMouseOnMap && (Input.mouseScrollDelta.y > 0)))
        {
            shouldMapMoveUp = true; //위 화살표를 누르거나 휠을 위쪽으로 돌리면 맵 올려줌
        }else if (Input.GetKey(KeyCode.DownArrow) || (isMouseOnMap && (Input.mouseScrollDelta.y < 0)))
        {
            shouldMapMoveDown = true; //아래 화살표를 누르거나 휠을 아래쪽으로 돌리면 맵 내려줌 
        }else
        {
            shouldMapMoveUp = false;
            shouldMapMoveDown = false;
        }
    }

    public void StageStart() //test()와 동일한 역할 함. GameData 갱신 기능 없음
    {
        GameObject stageButtonTmp = stageButton[selectedStageNum].gameObject;
        stageMoveButton tmpScript = stageButtonTmp.GetComponent<stageMoveButton>();

        GameManager.instance.gameData.curAchievementNum = selectedSavePointNum+1; //선택된 세이브포인트 숫자가 현재 achievement Num 의 번호(achievement Num 은 1부터 시작)
        GameManager.instance.gameData.curStageNum = selectedStageNum;
        GameManager.instance.nextScene = tmpScript.savePointScene[selectedSavePointNum]; //선택된 씬으로 이동
        GameManager.instance.nextPos = tmpScript.savePointPos[selectedSavePointNum]; //선택된 savePoint 위치로 이동 
        GameManager.instance.nextGravityDir = tmpScript.savePointGravityDir[selectedSavePointNum]; //선택된 savePoint 중력방향 적용
        GameManager.instance.isCliffChecked = false;

        for (int i = 0; i < 35; i++)
        {
            GameManager.instance.curIsShaked[i] = false;
            GameManager.instance.gameData.storedIsShaked[i] = false;
        }

        GameManager.instance.shouldStartAtSavePoint = true;
        GameManager.instance.nextState = Player.States.Walk;
        GameManager.instance.gameData.respawnScene = GameManager.instance.nextScene;
        GameManager.instance.gameData.respawnPos = GameManager.instance.nextPos;
        GameManager.instance.gameData.respawnGravityDir = GameManager.instance.nextGravityDir;

        Cursor.lockState = CursorLockMode.Locked;
        InputManager.instance.isInputBlocked = false;
        SceneManager.LoadScene(GameManager.instance.nextScene);
    }   
}