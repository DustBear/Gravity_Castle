using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class gameTextManager : Singleton<gameTextManager>
{
    public int languageNum; //�����ϴ� ��ü ����� ���� 

    public int selectedLanguageNum;
    //���õ� ��� ��ȣ 

    // 0: �ѱ��� 
    // 1: ���� 
    // 2: �߱���(��ü)
    // 3: �߱���(��ü)
    // 4: �Ϻ��� 
    //... 

    //���� ��� ������ �°� �߰� ���� 

    public TextAsset stageName_textFile; //�� ���������� �̸��� ������ �ؽ�Ʈ����
    public TextAsset system_textFile; //UI � ���Ǵ� �� ������ �ؽ�Ʈ���� 
    public TextAsset tutorial_textFile; //Ʃ�丮�� ���� ���Ǵ� �ؽ�Ʈ�� ������ ���� 

    private void Awake()
    {
        selectedLanguageNum = GameManager.instance.optionSettingData.languageSetting;
    }
    public string stageNameManager(int stageNum) //���õ� �������� ��ȣ(1~) �� ��� ��ȣ(0~) �� �´� �ؽ�Ʈ ���� 
    {
        string[] textData = stageName_textFile.text.Split(new string[] { ",", "\n" }, StringSplitOptions.None);
        int requiredIndex = (stageNum - 1) * languageNum + selectedLanguageNum;

        return textData[requiredIndex];
    }

    public string systemTextManager(int textIndex)
    {
        string[] textData = system_textFile.text.Split(new string[] { ",", "\n" }, StringSplitOptions.None);
        int requiredIndex = (textIndex - 1) * languageNum + selectedLanguageNum;

        string aimWords = textData[requiredIndex];
        string output = "";

        for (int index = 0; index < aimWords.Length; index++)
        {
            if (aimWords[index] == '/')
            {
                output += "\n"; //���� ��ȣ 
            }
            else if (aimWords[index] == '#')
            {
                output += ","; //�޸� ��ȣ 
            }
            else
            {
                output += aimWords[index];
            }
        }

        return output;
    }

    public string tutorialTextManager(int textIndex)
    {
        string[] textData = tutorial_textFile.text.Split(new string[] { ",", "\n" }, StringSplitOptions.None);
        int requiredIndex = (textIndex - 1) * languageNum + selectedLanguageNum;

        string aimWords = textData[requiredIndex];
        string output = "";
               
        for(int index=0; index<aimWords.Length; index++)
        {           
            if(aimWords[index] == '/')
            {
                output += "\n"; //���� ��ȣ 
            }
            else if(aimWords[index] == '#')
            {
                output += ","; //�޸� ��ȣ 
            }
            else
            {
                output += aimWords[index];
            }
        }

        return output;
    }

    void Start()
    {
    }

    void Update()
    {
        
    }
}
