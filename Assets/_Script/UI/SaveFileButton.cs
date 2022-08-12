using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;

public class SaveFileButton : MonoBehaviour
{
    [SerializeField] int saveFileNum; //이 버튼의 세이브파일 번호
    TextMeshProUGUI text;
    bool isSaveFileExist;

    public GameObject betaModeWindow; //베타테스트 버전으로 바꾸시겠습니까? 창 띄움 
    public GameObject saveDeleteWindow;

    public GameObject saveDeleteButton;

    void Awake()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
    }

    void Start()
    {      
        KeyValuePair<int, int> keyVal = GameManager.instance.GetSavedData(saveFileNum);
        if (keyVal.Key != -1)
        {
            text.text = "Save" + (saveFileNum + 1) + " : 스테이지" + keyVal.Key + "_" + keyVal.Value;
            isSaveFileExist = true;
        }

        saveDeleteWindow.SetActive(false);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q) && saveDeleteWindow.activeSelf == true)
        {
            saveDeleteButton.SetActive(false);
            saveDeleteWindow.SetActive(false);
        }
    }

    public void OnClickButton()
    {
        UIManager.instance.clickSoundGen();

        GameManager.instance.curSaveFileNum = saveFileNum;
        // SaveFile이 없으면 새 게임
        if (!isSaveFileExist)
        {
            GameManager.instance.saveFileSeq.saveFileSeqList.Add(saveFileNum);
            GameManager.instance.SaveSaveFileSeq();

            GameManager.instance.StartGame(false);
        }
        // SaveFile이 있으면 Load
        else
        {
            GameManager.instance.saveFileSeq.saveFileSeqList.Remove(saveFileNum);
            GameManager.instance.saveFileSeq.saveFileSeqList.Add(saveFileNum);
            GameManager.instance.SaveSaveFileSeq();
            GameManager.instance.shouldStartAtSavePoint = true;

            GameManager.instance.StartGame(true);
        }
    }

    public void saveDelete() //세이브파일 정보를 게임을 처음 시작하는 상태로 초기화시킴 
    {
        UIManager.instance.clickSoundGen();
        Debug.Log("delete");

        UIManager.instance.clickSoundGen();
        GameManager.instance.curSaveFileNum = saveFileNum;

        GameManager.instance.gameData.curAchievementNum = 0;
        GameManager.instance.gameData.curStageNum = 0;
        GameManager.instance.gameData.finalAchievementNum = 0;
        GameManager.instance.gameData.finalStageNum = 0;

        isSaveFileExist = false;

        string ToJsonData = JsonUtility.ToJson(GameManager.instance.gameData);
        string filePath = Application.persistentDataPath + GameManager.instance.gameDataFileNames[GameManager.instance.curSaveFileNum];
        File.WriteAllText(filePath, ToJsonData);

        text.text = "Save" + (saveFileNum + 1) + " : 스테이지 0_0";
        saveDeleteWindow.SetActive(false);
    }

    public void saveDeleteWindowOpen()
    {
        UIManager.instance.clickSoundGen();

        saveDeleteWindow.SetActive(true);

        for(int index=0; index<4; index++)
        {
            saveDeleteButton.SetActive(true);
        }
    }
}
