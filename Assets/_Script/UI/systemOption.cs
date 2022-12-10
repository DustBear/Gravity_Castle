using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class systemOption : MonoBehaviour
{
    public GameObject systemGroup;
    public GameObject audioGroup;

    //���� 
    [SerializeField] int selectedLanguageIndex; //���� ���õ� ��� ��ȣ 
    [SerializeField] int languageCount = 5; //��ü ���ð����� ��� �� 

    public string[] languageArray; //�����ϴ� ����� ��� ~> languageCount�� ���� ���ƾ� �� 
    public Text languageName;

    void Start()
    {
        selectedLanguageIndex = GameManager.instance.optionSettingData.languageSetting; //�⺻ ��� ���� �������� 
        gameTextManager.instance.selectedLanguageNum = selectedLanguageIndex; //��� ���� �ٲ� 
        languageName.text = languageArray[selectedLanguageIndex];

        systemGroup.SetActive(false);
        //������ ���� â ���� 
    }

    void Update()
    {
        
    }

    public void onClickSelf()
    {
        UIManager.instance.clickSoundGen();

        //���� ������ ���� ���� ������ �� 
        systemGroup.SetActive(!systemGroup.activeSelf);

        //�ý��ۿɼ� â �̿��� �ٸ� �ɼ��� ��� �� 
        audioGroup.SetActive(false);
    }

    public void rightArrowClick()
    {
        UIManager.instance.clickSoundGen();

        if (selectedLanguageIndex < languageCount)
        {
            selectedLanguageIndex++;
            gameTextManager.instance.selectedLanguageNum = selectedLanguageIndex; //��� ���� �ٲ� 
            languageName.text = languageArray[selectedLanguageIndex];

            GameManager.instance.optionSettingData.languageSetting = selectedLanguageIndex; //���� ������ �ٲ� 

            //�ٲ� ��� ���� ������ ���� 
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
            gameTextManager.instance.selectedLanguageNum = selectedLanguageIndex; //��� ���� �ٲ� 
            languageName.text = languageArray[selectedLanguageIndex];

            GameManager.instance.optionSettingData.languageSetting = selectedLanguageIndex; //���� ������ �ٲ� 

            //�ٲ� ��� ���� ������ ���� 
            string ToJsonData = JsonUtility.ToJson(GameManager.instance.optionSettingData);
            string filePath = Application.persistentDataPath + GameManager.instance.optionFileName;
            File.WriteAllText(filePath, ToJsonData);
        }
    }

    public void OnDisable()
    {
        //â�� ������ �ý��� �ɼ�â�� ���� 
        systemGroup.SetActive(false);
    }
}
