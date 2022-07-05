using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class NewGameButton : MonoBehaviour
{
    TextMeshProUGUI text;
    bool isSaveFileExist;

    void Awake()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
    }


    void Start()
    {
        // SaveFile이 하나라도 존재하면 "이어하기"로 텍스트 변경
        if (GameManager.instance.saveFileSeq.saveFileSeqList.Count != 0)
        {
            text.text = "이어하기";
            isSaveFileExist = true;
        }
    }

    public void OnClickButton()
    {
        // SaveFile이 없으면 새 게임
        if (!isSaveFileExist)
        {
            GameManager.instance.curSaveFileNum = 0;
            GameManager.instance.saveFileSeq.saveFileSeqList.Add(0);
            GameManager.instance.SaveSaveFileSeq();
            GameManager.instance.StartGame(false);
        }
        // SaveFile이 있으면 가장 최근 SaveFile을 Load
        else
        {
            GameManager.instance.curSaveFileNum = GameManager.instance.saveFileSeq.saveFileSeqList.Last();
            GameManager.instance.shouldStartAtSavePoint = true;
            GameManager.instance.StartGame(true);
        }
    }
}
