using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class systemOption : MonoBehaviour
{
    public GameObject systemGroup;
    public GameObject audioGroup;

    //언어설정 
    [SerializeField] int selectedLanguageIndex; //현재 선택된 언어 번호 
    [SerializeField] int languageCount = 5; //전체 선택가능한 언어 수 

    public string[] languageArray; //지원하는 언어의 목록 ~> languageCount와 수가 같아야 함 
    public Text languageName;

    void Start()
    {
        selectedLanguageIndex = GameManager.instance.optionSettingData.languageSetting; //기본 언어 설정 가져오기 
        gameTextManager.instance.selectedLanguageNum = selectedLanguageIndex; //언어 설정 바꿈 
        languageName.text = languageArray[selectedLanguageIndex];

        systemGroup.SetActive(false);
        //시작할 때는 창 끄기 
    }

    void Update()
    {
        
    }

    public void onClickSelf()
    {
        UIManager.instance.clickSoundGen();

        //켜져 있으면 끄고 꺼져 있으면 켬 
        systemGroup.SetActive(!systemGroup.activeSelf);

        //시스템옵션 창 이외의 다른 옵션은 모두 끔 
        audioGroup.SetActive(false);
    }

    public void rightArrowClick()
    {
        UIManager.instance.clickSoundGen();

        if (selectedLanguageIndex < languageCount)
        {
            selectedLanguageIndex++;
            gameTextManager.instance.selectedLanguageNum = selectedLanguageIndex; //언어 설정 바꿈 
            languageName.text = languageArray[selectedLanguageIndex];

            GameManager.instance.optionSettingData.languageSetting = selectedLanguageIndex; //세팅 데이터 바꿈 

            //바꾼 언어 세팅 데이터 저장 
            string ToJsonData = JsonUtility.ToJson(GameManager.instance.optionSettingData);
            string filePath = Application.persistentDataPath + GameManager.instance.optionFileName;
            File.WriteAllText(filePath, ToJsonData);
        }
    }
    public void leftArrowClick()
    {
        UIManager.instance.clickSoundGen();

        if (selectedLanguageIndex > 0)
        {
            selectedLanguageIndex--;
            gameTextManager.instance.selectedLanguageNum = selectedLanguageIndex; //언어 설정 바꿈 
            languageName.text = languageArray[selectedLanguageIndex];

            GameManager.instance.optionSettingData.languageSetting = selectedLanguageIndex; //세팅 데이터 바꿈 

            //바꾼 언어 세팅 데이터 저장 
            string ToJsonData = JsonUtility.ToJson(GameManager.instance.optionSettingData);
            string filePath = Application.persistentDataPath + GameManager.instance.optionFileName;
            File.WriteAllText(filePath, ToJsonData);
        }
    }

    public void OnDisable()
    {
        //창을 닫으면 시스템 옵션창도 꺼짐 
        systemGroup.SetActive(false);
    }
}
