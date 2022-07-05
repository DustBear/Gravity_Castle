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
        // SaveFile�� �ϳ��� �����ϸ� "�̾��ϱ�"�� �ؽ�Ʈ ����
        if (GameManager.instance.saveFileSeq.saveFileSeqList.Count != 0)
        {
            text.text = "�̾��ϱ�";
            isSaveFileExist = true;
        }
    }

    public void OnClickButton()
    {
        // SaveFile�� ������ �� ����
        if (!isSaveFileExist)
        {
            GameManager.instance.curSaveFileNum = 0;
            GameManager.instance.saveFileSeq.saveFileSeqList.Add(0);
            GameManager.instance.SaveSaveFileSeq();
            GameManager.instance.StartGame(false);
        }
        // SaveFile�� ������ ���� �ֱ� SaveFile�� Load
        else
        {
            GameManager.instance.curSaveFileNum = GameManager.instance.saveFileSeq.saveFileSeqList.Last();
            GameManager.instance.shouldStartAtSavePoint = true;
            GameManager.instance.StartGame(true);
        }
    }
}
