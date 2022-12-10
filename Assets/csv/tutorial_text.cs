using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class tutorial_text : MonoBehaviour
{
    [SerializeField] int textIndex;

    int languageIndex_last = 0;
    int languageIndex_cur = 0;

    TextMeshProUGUI thisText;
    private void Awake()
    {
        thisText = GetComponent<TextMeshProUGUI>();
    }
    void Start()
    {
        thisText.text = gameTextManager.instance.tutorialTextManager(textIndex);
        languageIndex_cur = gameTextManager.instance.selectedLanguageNum;
    }


    void Update()
    {
        languageIndex_cur = gameTextManager.instance.selectedLanguageNum;

        //���� �� �ٲ�� ����Ǵ� �ؽ�Ʈ�� �ٲ��� �� 
        if (languageIndex_cur != languageIndex_last)
        {
            thisText.text = gameTextManager.instance.tutorialTextManager(textIndex);
        }
        languageIndex_last = languageIndex_cur;
    }
}
