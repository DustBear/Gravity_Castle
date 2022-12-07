using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        selectedLanguageIndex = 0; //�⺻ ������ �ѱ��� 
        languageName.text = languageArray[selectedLanguageIndex];

        systemGroup.SetActive(false);
        //������ ���� â ���� 
    }

    void Update()
    {
        
    }

    public void onClickSelf()
    {
        //���� ������ ���� ���� ������ �� 
        systemGroup.SetActive(!systemGroup.activeSelf);

        //�ý��ۿɼ� â �̿��� �ٸ� �ɼ��� ��� �� 
        audioGroup.SetActive(false);
    }

    public void rightArrowClick()
    {
        if(selectedLanguageIndex < languageCount)
        {
            selectedLanguageIndex++;
            gameTextManager.instance.selectedLanguageNum = selectedLanguageIndex; //��� ���� �ٲ� 
            languageName.text = languageArray[selectedLanguageIndex];
        }
    }
    public void leftArrowClick()
    {
        if (selectedLanguageIndex > 0)
        {
            selectedLanguageIndex--;
            gameTextManager.instance.selectedLanguageNum = selectedLanguageIndex; //��� ���� �ٲ� 
            languageName.text = languageArray[selectedLanguageIndex];
        }
    }

    public void OnDisable()
    {
        //â�� ������ �ý��� �ɼ�â�� ���� 
        systemGroup.SetActive(false);
    }
}
