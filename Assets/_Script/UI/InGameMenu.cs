using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.UI;

public class InGameMenu : MonoBehaviour
{
    public int collectionCount; //현재 씬 내의 수집요소가 몇 개나 있는지 
    public int firstCol_num; //현재 씬에서 첫 번째 수집요소의 번호는 몇 번인지 

    public GameObject collectionIcon;
    public GameObject collection_firstAnchor; //수집과 저장을 끝낸 탐험가상자 중 이 씬 내에 있는 것의 1번 정렬위치 
    public GameObject collection_tmp_Anchor; //수집만 하고 저장은 안 한 탐험가상자의 1번 정렬위치
    public GameObject compassIcon;

    [SerializeField] GameObject collGroup; //생성된 수집요소 아이콘은 이 아래에 집어넣어 정리함 
    [SerializeField] float iconGap;

    [SerializeField] Sprite activeIcon; //이미 수집한 수집요소의 아이콘
    [SerializeField] Sprite tmpSavedIcon; //수집은 했지만 저장하지 않은 아이콘 
    [SerializeField] Sprite deactiveIcon; //아직 수집하지 않은 수집요소의 아이콘 

    [SerializeField] Sprite[] compass_sprite;

    Vector2 col_initIconPos;
    Vector2 col_tmpIconPos;

    public Text stageNameIns; //현재 스테이지의 이름 
    public Text depthInstruction; //현재 심도(curachievement Num * 50f)

    [SerializeField] Button mainMenuButton;
    [SerializeField] Button gameMenuButton;


    private void Awake()
    {
        col_initIconPos = collection_firstAnchor.transform.position;
        col_tmpIconPos = collection_tmp_Anchor.transform.position;
    }

    private void OnEnable()
    {
        collectionIconMake();
        compassIconControl();

        //현재 스테이지 이름과 심도를 창을 열 때마다 보여줌 
        stageNameIns.text = GameManager.instance.stageName[GameManager.instance.gameData.curStageNum - 1];

        int depthNum = GameManager.instance.saveNumCalculate(new Vector2(GameManager.instance.gameData.curStageNum, GameManager.instance.gameData.curAchievementNum));
        depthInstruction.text = "심도\n " + depthNum * 50f + "m";
    }

    public void OnClickExit() //메인메뉴로 나가기 버튼 누를 때 
    {
        mainMenuButton.interactable = false;
        gameMenuButton.interactable = false;

        //나가기 버튼 중복 클릭 못하게 락 검 

        UIManager.instance.clickSoundGen();

        GameManager.instance.gameData.collectionTmp.Clear();
        GameManager.instance.gameData.SpawnSavePoint_bool = true;

        Time.timeScale = 1f;

        if (GameManager.instance.gameData.curAchievementNum == 0)
        {
            //opening elevator에서 내리고 첫 번째 세이브를 활성화하기 전에 죽으면 그냥 save1 까지는 활성화시킨 것으로 치고 넘어감  

            GameManager.instance.gameData.curAchievementNum = 1;
            if (GameManager.instance.gameData.finalStageNum == GameManager.instance.gameData.curStageNum)
            {
                if (GameManager.instance.gameData.finalAchievementNum == 0)
                {
                    GameManager.instance.gameData.finalAchievementNum = 1; 
                    //만약 현재 최고 진행도 이상이라면 final data 역시 갱신해줘야 함 
                }
            }
            GameManager.instance.gameData.savePointUnlock[GameManager.instance.saveNumCalculate(new Vector2(GameManager.instance.gameData.curStageNum, 1))] = 1; //세이브포인트 1 활성화 
            GameManager.instance.gameData.respawnScene = SceneManager.GetActiveScene().buildIndex; // 

            GameManager.instance.gameData.SpawnSavePoint_bool = true;
            GameManager.instance.gameData.UseOpeningElevetor_bool = false;

            //세이브파일에 데이터 저장 
            string ToJsonData = JsonUtility.ToJson(GameManager.instance.gameData);
            string filePath = Application.persistentDataPath + GameManager.instance.gameDataFileNames[GameManager.instance.curSaveFileNum];
            File.WriteAllText(filePath, ToJsonData);

            GameManager.instance.nextScene = GameManager.instance.gameData.respawnScene;
            GameManager.instance.shouldUseOpeningElevator = false;
        }

        UIManager.instance.FadeOut(0.7f);
        collectionIconDel();
        Invoke("loadMainMenu", 1f);
    }

    public void OnClickMainStage() //인게임 메뉴로 나가기 버튼 누를 때 
    {
        mainMenuButton.interactable = false;
        gameMenuButton.interactable = false;

        UIManager.instance.clickSoundGen();

        GameManager.instance.gameData.collectionTmp.Clear();
        GameManager.instance.gameData.SpawnSavePoint_bool = true;

        Time.timeScale = 1f;
        
        if (GameManager.instance.gameData.curAchievementNum == 0)
        {
            //opening Elevator 작동중이고 아직 첫 세이브포인트를 활성화시키지 않았을 때 

            GameManager.instance.gameData.curAchievementNum = 1; 
            if (GameManager.instance.gameData.finalStageNum == GameManager.instance.gameData.curStageNum)
            {
                if (GameManager.instance.gameData.finalAchievementNum == 0)
                {
                    GameManager.instance.gameData.finalAchievementNum = 1; //진행도 갱신한 거면 데이터 반영해 줌
                }
            }
            GameManager.instance.gameData.savePointUnlock[GameManager.instance.saveNumCalculate(new Vector2(GameManager.instance.gameData.curStageNum, 1))] = 1; 
            GameManager.instance.gameData.respawnScene = SceneManager.GetActiveScene().buildIndex;

            GameManager.instance.gameData.SpawnSavePoint_bool = true;
            GameManager.instance.gameData.UseOpeningElevetor_bool = false;

            //세이브파일에 데이터 저장 
            string ToJsonData = JsonUtility.ToJson(GameManager.instance.gameData);
            string filePath = Application.persistentDataPath + GameManager.instance.gameDataFileNames[GameManager.instance.curSaveFileNum];
            File.WriteAllText(filePath, ToJsonData);

            GameManager.instance.nextScene = GameManager.instance.gameData.respawnScene;
            GameManager.instance.shouldUseOpeningElevator = false;
        }

        UIManager.instance.FadeOut(0.7f);
        collectionIconDel();
        Invoke("loadInGameMenu", 1f);
    }

    void loadInGameMenu()
    {
        SceneManager.LoadScene("InGameMenu");
        Cursor.lockState = CursorLockMode.None;

        gameMenuButton.interactable = true;
        mainMenuButton.interactable = true;

        gameObject.SetActive(false);
    }

    void loadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
        Cursor.lockState = CursorLockMode.None;

        gameMenuButton.interactable = true;
        mainMenuButton.interactable = true;

        gameObject.SetActive(false);
    }

    public void collectionIconMake()
    {
        for(int index=0; index<collectionCount; index++)
        {
            GameObject tmpIcon = Instantiate(collectionIcon);
            tmpIcon.transform.SetParent(collGroup.transform); //생성된 수집요소 아이콘은 ingameMenu 아래에 넣어 저장            

            int tmpIconNum = firstCol_num + index; //방금 만든 수집요소의 번호 
            tmpIcon.transform.position = col_initIconPos + new Vector2(iconGap * index, 0);
            tmpIcon.GetComponent<collectionIconButton>().collectionNum = tmpIconNum;

            //현재 수집, 저장 끝낸 탐험가 상자를 정렬하여 표시 
            if (GameManager.instance.gameData.collectionUnlock[tmpIconNum])
            {
                //만약 이미 수집과 저장을 끝낸 탐험가 상자라면               
                tmpIcon.GetComponent<Image>().sprite = activeIcon;
            }
            else
            {
                //아직 세이브하지 않았다면
                tmpIcon.GetComponent<Image>().sprite = deactiveIcon;
            }
        }

        for(int index=0; index<GameManager.instance.gameData.collectionTmp.Count; index++)
        {
            GameObject tmpIcon = Instantiate(collectionIcon);
            tmpIcon.transform.SetParent(collGroup.transform); //생성된 수집요소 아이콘은 ingameMenu 아래에 넣어 저장            
            tmpIcon.transform.position = col_tmpIconPos + new Vector2(iconGap * index, 0); //정렬 

            tmpIcon.GetComponent<collectionIconButton>().collectionNum = GameManager.instance.gameData.collectionTmp[index];
            tmpIcon.GetComponent<Image>().sprite = tmpSavedIcon;
        }
    }

    public void collectionIconDel()
    {
        Transform[] childList = collGroup.GetComponentsInChildren<Transform>();

        if(childList != null)
        {
            for(int i=1; i < childList.Length; i++)
            {
                if(childList[i] != transform)
                {
                    Destroy(childList[i].gameObject);
                }
            }
        }
    }

    void compassIconControl()
    {
        Image spr = compassIcon.GetComponent<Image>();
        if(Physics2D.gravity.normalized == new Vector2(0, -1)) //땅이 아래쪽에 있음 
        {
            spr.sprite = compass_sprite[2];
        }
        else if(Physics2D.gravity.normalized == new Vector2(0, 1)) //땅이 위쪽에 있음
        {
            spr.sprite = compass_sprite[0];
        }
        else if (Physics2D.gravity.normalized == new Vector2(1, 0)) //땅이 오른쪽에 있음 
        {
            spr.sprite = compass_sprite[1];
        }
        else //땅이 왼쪽에 있음 
        {
            spr.sprite = compass_sprite[3];
        }
    }
}
