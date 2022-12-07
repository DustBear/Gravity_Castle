using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class text_universal : MonoBehaviour
{
    //text 컴포넌트에 붙여서 사용하는 범용 텍스트 관리자 

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

        //만약 언어가 바뀌면 노출되는 텍스트도 바뀌어야 함 
        if(languageIndex_cur != languageIndex_last)
        {
            thisText.text = gameTextManager.instance.systemTextManager(textIndex);
        }
        languageIndex_last = languageIndex_cur;
    }
}
