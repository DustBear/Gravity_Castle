using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class StartMenu : MonoBehaviour
{
    public GameObject titleMenu;

    public GameObject gameMenu; //�����ϱ�, �̾��ϱ�, �ҷ����� ��ư �ִ� â 
    public GameObject loadMenu; //���̺����� ��ư 1~4 �ߴ� �� 
    
    public TextMeshProUGUI settingExp;

    public GameObject titleImage; //��� ���� 
    public GameObject fadeCover;
    public GameObject gameMenuText; //'�ƹ� ��ư�̳� ���� �����ϼ���' ��ư ~> �������� �� 

    Color fadeCoverColor;

    public bool isTitleMenuOpen;
    public bool isGameMenuOpen;
    private void Start()
    {
        isTitleMenuOpen = true;
        titleMenu.SetActive(true); //�����ϸ� titleMenu ���� ������ 

        //gameMenu, loadMenu �� 
        isGameMenuOpen = false;
        gameMenu.SetActive(false);
      
        loadMenu.SetActive(false);


        InputManager.instance.isInputBlocked = false; //���� �޴��� ���ƿ��� ������ inputBlock Ǯ����
        
        StartCoroutine("blink");
    }

    private void Update()
    {
        if (Input.anyKeyDown && isTitleMenuOpen) //Ÿ��Ʋȭ���� ���� �ִ� ���¿��� �ƹ� Ű�� ������ 
        {
            //Ÿ��Ʋȭ�� ���� 
            titleMenu.SetActive(false);
            isTitleMenuOpen = false;

            StartCoroutine("gameMenuOpen"); //���Ӹ޴� Ű�� 
        }  
    }

    IEnumerator gameMenuOpen() //�޴� ���� �ݴ� �߰��� ���̵���/�ƿ� ȿ�� �־�� �� 
    {
        fadeCover.GetComponent<Image>().color = new Color(0, 0, 0, 1); //ó������ ���� ȭ�鿡�� ����

        //gameMenu Ű�� 
        isGameMenuOpen = true;
        gameMenu.SetActive(true);

        //���̵���
        float fadeTime = 2f; //���̵����� �Ϸ�Ǵ� �� �ɸ��� �ð� 

        for(int index=100; index>=1; index--)
        {
            fadeCover.GetComponent<Image>().color = new Color(0, 0, 0, 0.01f*index);
            yield return new WaitForSeconds(0.01f * fadeTime);
        }

        fadeCover.GetComponent<Image>().color = new Color(0, 0, 0, 0);
    }

    IEnumerator blink() //���� ��ư �����̰� ����
    {       
        while (true)
        {
            gameMenuText.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            gameMenuText.SetActive(false);
            yield return new WaitForSeconds(0.5f);

            if (!isTitleMenuOpen)
            {
                yield break;
            }
        }      
    }

 
    public void OnClickSetting()
    {
        UIManager.instance.clickSoundGen();

        settingExp.gameObject.SetActive(true);
        Invoke("settingMessageOff", 3f); //3�� �� ���ø޽��� ����

        //Debug.Log("Start Setting!");
    }
    void settingMessageOff()
    {
        settingExp.gameObject.SetActive(false);
    }

    // ���� ���� �� Editor �󿡼��� ��������, ���� ���� �󿡼��� �������� ����
    public void OnClickExit()
    {
        UIManager.instance.clickSoundGen();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
