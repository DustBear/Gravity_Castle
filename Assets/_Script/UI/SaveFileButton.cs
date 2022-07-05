using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SaveFileButton : MonoBehaviour
{
    [SerializeField] int saveFileNum;
    TextMeshProUGUI text;
    bool isSaveFileExist;

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
    }

    public void OnClickButton()
    {
        
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
}
