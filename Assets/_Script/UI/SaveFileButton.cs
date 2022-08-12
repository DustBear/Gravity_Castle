using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;

public class SaveFileButton : MonoBehaviour
{
    [SerializeField] int saveFileNum; //�� ��ư�� ���̺����� ��ȣ
    TextMeshProUGUI text;
    bool isSaveFileExist;

    public GameObject betaModeWindow; //��Ÿ�׽�Ʈ �������� �ٲٽðڽ��ϱ�? â ��� 
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
            text.text = "Save" + (saveFileNum + 1) + " : ��������" + keyVal.Key + "_" + keyVal.Value;
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
        // SaveFile�� ������ �� ����
        if (!isSaveFileExist)
        {
            GameManager.instance.saveFileSeq.saveFileSeqList.Add(saveFileNum);
            GameManager.instance.SaveSaveFileSeq();

            GameManager.instance.StartGame(false);
        }
        // SaveFile�� ������ Load
        else
        {
            GameManager.instance.saveFileSeq.saveFileSeqList.Remove(saveFileNum);
            GameManager.instance.saveFileSeq.saveFileSeqList.Add(saveFileNum);
            GameManager.instance.SaveSaveFileSeq();
            GameManager.instance.shouldStartAtSavePoint = true;

            GameManager.instance.StartGame(true);
        }
    }

    public void saveDelete() //���̺����� ������ ������ ó�� �����ϴ� ���·� �ʱ�ȭ��Ŵ 
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

        text.text = "Save" + (saveFileNum + 1) + " : �������� 0_0";
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
