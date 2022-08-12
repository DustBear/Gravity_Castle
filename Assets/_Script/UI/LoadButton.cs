using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LoadButton : MonoBehaviour
{
    [SerializeField] GameObject loadMenu;
    Button button;
    TextMeshProUGUI text;
    bool isSaveFileExist;

    void Awake()
    {
        button = GetComponent<Button>();
        text = GetComponentInChildren<TextMeshProUGUI>();
    }

    void Start()
    {
        // SaveFile�� �ϳ��� �����ϸ� Load��ư Ȱ��ȭ
        for (int saveFileNum = 0; saveFileNum < GameManager.instance.saveFileCount; saveFileNum++)
        {
            if (GameManager.instance.GetSavedData(saveFileNum).Key != -1)
            {
                button.interactable = true;
                text.color = new Color(1f, 1f, 1f, 1f);
                isSaveFileExist = true;
                return;
            }
        }
    }

    public void OnClickButton()
    {
        UIManager.instance.clickSoundGen();

        if (isSaveFileExist)
        {
            loadMenu.SetActive(!loadMenu.activeSelf);
        }
    }
}
