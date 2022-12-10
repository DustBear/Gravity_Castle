using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class text_universal : MonoBehaviour
{
    //text ������Ʈ�� �ٿ��� ����ϴ� ���� �ؽ�Ʈ ������ 

    [SerializeField] int textIndex;

    int languageIndex_last=0;
    int languageIndex_cur=0;

    //text �� textMeshPro �� �� ����� �� �ֵ��� ����� �� ���� 
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

        //���� �� �ٲ�� ����Ǵ� �ؽ�Ʈ�� �ٲ��� �� 
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
