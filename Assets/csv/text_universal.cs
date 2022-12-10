using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class text_universal : MonoBehaviour
{
    //text 컴포넌트에 붙여서 사용하는 범용 텍스트 관리자 

    [SerializeField] int textIndex;

    int languageIndex_last=0;
    int languageIndex_cur=0;

    //text 와 textMeshPro 둘 다 사용할 수 있도록 경우의 수 지정 
    Text thisText; //textType = 1
    TextMeshProUGUI thisTextPro; //textType = 2

    int textType;

    private void Awake()
    {
        if(GetComponent<Text>() != null)
        {
            thisText = GetComponent<Text>();
            textType = 1;
        }
        else if(GetComponent<TextMeshPro>() != null)
        {
            thisTextPro = GetComponent<TextMeshProUGUI>();
            textType = 2;
        }
    }
    void Start()
    {
        if(textType == 1)
        {
            thisText.text = gameTextManager.instance.systemTextManager(textIndex);
        }
        else
        {
            thisTextPro.text = gameTextManager.instance.systemTextManager(textIndex);
        }

        languageIndex_cur = gameTextManager.instance.selectedLanguageNum;
    }

    void Update()
    {
        languageIndex_cur = gameTextManager.instance.selectedLanguageNum;

        //만약 언어가 바뀌면 노출되는 텍스트도 바뀌어야 함 
        if(languageIndex_cur != languageIndex_last)
        {
            if(textType == 1)
            {
                thisText.text = gameTextManager.instance.systemTextManager(textIndex);
            }
            else
            {
                thisTextPro.text = gameTextManager.instance.systemTextManager(textIndex);
            }
        }
        languageIndex_last = languageIndex_cur;
    }
}
