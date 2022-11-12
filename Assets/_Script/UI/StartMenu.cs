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
    //public GameObject fadeCover;
    public GameObject gameMenuText; //'�ƹ� ��ư�̳� ���� �����ϼ���' ��ư ~> �������� �� 

    Color fadeCoverColor;

    public bool isTitleMenuOpen;
    public bool isGameMenuOpen;
    private void Start()
    {
        StartCoroutine(startFade());

        isTitleMenuOpen = true;
        titleMenu.SetActive(true); //�����ϸ� titleMenu ���� ������ 

        //gameMenu, loadMenu �� 
        isGameMenuOpen = false;
        gameMenu.SetActive(false);
      
        loadMenu.SetActive(false);


        InputManager.instance.isInputBlocked = false; //���� �޴��� ���ƿ��� ������ inputBlock Ǯ����
        
        StartCoroutine("blink");
    }

    IEnumerator startFade()
    {
        UIManager.instance.fade.color = new Color(0, 0, 0, 1);
        yield return new WaitForSeconds(0.3f);
        UIManager.instance.FadeIn(1.5f);
    }

    private void Update()
    {
        if (Input.anyKeyDown && isTitleMenuOpen) //Ÿ��Ʋȭ���� ���� �ִ� ���¿��� �ƹ� Ű�� ������ 
        {
            StartCoroutine("gameMenuOpen"); //���Ӹ޴� Ű�� 
        }  
    }

    IEnumerator gameMenuOpen() //�޴� ���� �ݴ� �߰��� ���̵���/�ƿ� ȿ�� �־�� �� 
    {
        UIManager.instance.FadeOut(1.5f);

        yield return new WaitForSeconds(2f); //ȭ���� ������ ��ο����� 

        //Ÿ��Ʋȭ�� ���� 
        titleMenu.SetActive(false);
        isTitleMenuOpen = false;

        //gameMenu Ű�� 
        isGameMenuOpen = true;
        gameMenu.SetActive(true);

        UIManager.instance.FadeIn(1.5f);
    }

    IEnumerator blink() //���� ��ư �����̰� ����
    {
        float cosTimer = 0;
        float blinkPeriod = 0.6f;
        while (true)
        {
            gameMenuText.GetComponent<Text>().color = new Color(1, 1, 1, Mathf.Cos(cosTimer/blinkPeriod)/2 + 0.5f);
            cosTimer += 0.01f;
            yield return new WaitForSeconds(0.01f);

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
