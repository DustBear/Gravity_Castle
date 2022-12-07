using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class text_universal : MonoBehaviour
{
    //text ������Ʈ�� �ٿ��� ����ϴ� ���� �ؽ�Ʈ ������ 

    [SerializeField] int textIndex;

    int languageIndex_last=0;
    int languageIndex_cur=0;

    Text thisText;

    private void Awake()
    {
        thisText = GetComponent<Text>();
    }
    void Start()
    {
        thisText.text = gameTextManager.instance.systemTextManager(textIndex);
        languageIndex_cur = gameTextManager.instance.selectedLanguageNum;
    }

    void Update()
    {
        languageIndex_cur = gameTextManager.instance.selectedLanguageNum;

        //���� �� �ٲ�� ����Ǵ� �ؽ�Ʈ�� �ٲ��� �� 
        if(languageIndex_cur != languageIndex_last)
        {
            thisText.text = gameTextManager.instance.systemTextManager(textIndex);
        }
        languageIndex_last = languageIndex_cur;
    }
}
