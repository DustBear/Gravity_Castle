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
    public GameObject collection_firstAnchor;

    [SerializeField] GameObject collGroup; //생성된 수집요소 아이콘은 이 아래에 집어넣어 정리함 
    [SerializeField] float iconGap;
    [SerializeField] Sprite activeIcon; //이미 수집한 수집요소의 아이콘
    [SerializeField] Sprite deactiveIcon; //아직 수집하지 않은 수집요소의 아이콘 

    Vector2 col_initIconPos;

    private void Awake()
    {
        col_initIconPos = collection_firstAnchor.transform.position;
    }

    private void OnEnable()
    {
        collectionIconMake();
    }

    public void OnClickExit() //메인메뉴로 나가기 버튼 누를 때 
    {
        UIManager.instance.clickSoundGen();
        collectionIconDel();
        GameManager.instance.gameData.collectionTmp.Clear();

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

            //세이브파일에 데이터 저장 
            string ToJsonData = JsonUtility.ToJson(GameManager.instance.gameData);
            string filePath = Application.persistentDataPath + GameManager.instance.gameDataFileNames[GameManager.instance.curSaveFileNum];
            File.WriteAllText(filePath, ToJsonData);

            GameManager.instance.nextScene = GameManager.instance.gameData.respawnScene;
            GameManager.instance.shouldUseOpeningElevator = false;
        }

        UIManager.instance.FadeOut(1f);
        Invoke("loadMainMenu", 1.5f);
    }

    public void OnClickMainStage() //인게임 메뉴로 나가기 버튼 누를 때 
    {
        UIManager.instance.clickSoundGen();
        collectionIconDel();
        GameManager.instance.gameData.collectionTmp.Clear();

        Time.timeScale = 1f;

        if(GameManager.instance.gameData.curAchievementNum == 0)
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

            //세이브파일에 데이터 저장 
            string ToJsonData = JsonUtility.ToJson(GameManager.instance.gameData);
            string filePath = Application.persistentDataPath + GameManager.instance.gameDataFileNames[GameManager.instance.curSaveFileNum];
            File.WriteAllText(filePath, ToJsonData);

            GameManager.instance.nextScene = GameManager.instance.gameData.respawnScene;
            GameManager.instance.shouldUseOpeningElevator = false;
        }

        UIManager.instance.FadeOut(1f);
        Invoke("loadInGameMenu", 1.5f);
    }

    void loadInGameMenu()
    {
        SceneManager.LoadScene("InGameMenu");
        Cursor.lockState = CursorLockMode.None;

        gameObject.SetActive(false);
    }

    void loadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");

        gameObject.SetActive(false);
    }

    public void collectionIconMake()
    {
        for(int index=0; index<collectionCount; index++)
        {
            GameObject tmpIcon = Instantiate(collectionIcon);
            tmpIcon.transform.SetParent(collGroup.transform); //생성된 수집요소 아이콘은 ingameMenu 아래에 넣어 저장 
            tmpIcon.transform.position = col_initIconPos + new Vector2(iconGap * index, 0);

            int tmpIconNum = firstCol_num + index; //방금 만든 수집요소의 번호 

            if (GameManager.instance.gameData.collectionUnlock[tmpIconNum])
            {
                tmpIcon.GetComponent<Image>().sprite = activeIcon;
            }
            else
            {
                tmpIcon.GetComponent<Image>().sprite = deactiveIcon;
            }
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
}
