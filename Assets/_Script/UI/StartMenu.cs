using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class StartMenu : MonoBehaviour
{
    public GameObject titleMenu;

    public GameObject gameMenu;
    //public Button[] gameMenuButton = new Button[4];
    public TextMeshProUGUI[] gameMenuButtonText = new TextMeshProUGUI[4];
    public Button[] gameMenuButton = new Button[4];
    public TextMeshProUGUI settingExp;

    public GameObject quickMenu;
    public GameObject titleImage; //��� ���� 
    public GameObject fadeCover;
    public float fadeSpeed;
    public GameObject gameMenuText; //'�ƹ� ��ư�̳� ���� �����ϼ���' ��ư ~> �������� �� 

    Color fadeCoverColor;

    public bool isTitleMenuOpen;
    public bool isGameMenuOpen;
    public bool isQuickMenuOpen;
    private void Start()
    {
        isTitleMenuOpen = true;
        titleMenu.SetActive(true);
        isGameMenuOpen = false;
        gameMenu.SetActive(false);
        isQuickMenuOpen = false;
        quickMenu.SetActive(false);
        InputManager.instance.isInputBlocked = false; //���� �޴��� ���ƿ��� ������ inputBlock Ǯ����

        fadeCoverColor = fadeCover.GetComponent<Image>().color;
        fadeCoverColor = new Color(0, 0, 0, 0); //ó������ ����
        
        for (int index = 0; index <= 3; index++)
        {
            gameMenuButtonText[index].color = new Color(1, 1, 1, 0);
        }
        for (int index = 0; index <= 3; index++)
        {
            gameMenuButton[index].GetComponent<Image>().color = new Color(1, 1, 1, 0);
        }
        StartCoroutine("blink");
    }

    private void Update()
    {
        if (Input.anyKeyDown && isTitleMenuOpen)
        {
            titleMenu.SetActive(false);
            isTitleMenuOpen = false;
            StartCoroutine("gameMenuOpen");
        }

        if (Input.GetKeyDown(KeyCode.Escape) && isQuickMenuOpen)
        {
            quickMenu.SetActive(false);
            isQuickMenuOpen = false;
        }
    }

    IEnumerator gameMenuOpen() //�޴� ���� �ݴ� �߰��� ���̵���/�ƿ� ȿ�� �־�� �� 
    {
        isGameMenuOpen = true;
        gameMenu.SetActive(true);
 
        //���̵���
        while (fadeCoverColor.a <= 1) //������ 1�� �� �� ���� ��� ���� ���� 
        {
            fadeCoverColor = new Color(0, 0, 0, fadeCoverColor.a + fadeSpeed);
            for(int index=0; index <=3; index++)
            {
                gameMenuButtonText[index].color = new Color(1, 1, 1, gameMenuButtonText[index].color.a + fadeSpeed);
            }
            for (int index = 0; index <= 3; index++)
            {
                gameMenuButton[index].GetComponent<Image>().color = new Color(1, 1, 1, gameMenuButton[index].GetComponent<Image>().color.a + fadeSpeed);
            }
            yield return new WaitForFixedUpdate();
        }              
    }

    IEnumerator blink() //���� ��ư �����̰� ����
    {
        if (isGameMenuOpen)
        {
            yield return null;
        }
        while (true)
        {
            gameMenuText.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            gameMenuText.SetActive(false);
            yield return new WaitForSeconds(0.5f);
        }      
    }

    public void openQuickMenu()
    {
        if (isQuickMenuOpen)
        {
            quickMenu.SetActive(false);
            isQuickMenuOpen = false;
        }
        else if (!isQuickMenuOpen)
        {
            quickMenu.SetActive(true);
            isQuickMenuOpen = true;
        }
    }

    public void newStart() //��������1 ������ Ʃ�丮���� ���Ե� ���� 
    {
        SceneManager.LoadScene(24); //Ʃ�丮���� 24��° �� 
    }

    public void OnClickTutorial()
    {
        Debug.Log("Start Tutorial!");
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
