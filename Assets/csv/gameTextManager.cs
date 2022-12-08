using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class gameTextManager : Singleton<gameTextManager>
{
    public int languageNum; //지원하는 전체 언어의 개수 

    public int selectedLanguageNum;
    //선택된 언어 번호 

    // 0: 한국어 
    // 1: 영어 
    // 2: 중국어(간체)
    // 3: 중국어(번체)
    // 4: 일본어 
    //... 

    //이후 언어 개수에 맞게 추가 가능 

    public TextAsset stageName_textFile; //각 스테이지의 이름을 저장한 텍스트파일
    public TextAsset system_textFile; //UI 등에 사용되는 용어를 저장한 텍스트파일 

    private void Awake()
    {
        selectedLanguageNum = GameManager.instance.optionSettingData.languageSetting;
    }
    public string stageNameManager(int stageNum) //선택된 스테이지 번호(1~) 와 언어 번호(0~) 에 맞는 텍스트 송출 
    {
        string[] textData = stageName_textFile.text.Split(new string[] { ",", "\n" }, StringSplitOptions.None);
        int requiredIndex = (stageNum - 1) * languageNum + selectedLanguageNum;

        return textData[requiredIndex];
    }

    public string systemTextManager(int textIndex)
    {
        string[] textData = system_textFile.text.Split(new string[] { ",", "\n" }, StringSplitOptions.None);
        int requiredIndex = (textIndex - 1) * languageNum + selectedLanguageNum;

        return textData[requiredIndex];
    }

    void Start()
    {
    }

    void Update()
    {
        
    }
}
