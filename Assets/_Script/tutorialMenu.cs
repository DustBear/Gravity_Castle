using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class tutorialMenu : MonoBehaviour
{
    public GameObject[] ch = new GameObject[5];
    public Button rightButton;
    public Button leftButton;
    public Button gameStartButton; //scene01 �� �̵���

    [SerializeField]int curIndex; //���� ���� �ִ� index
    void Start()
    {
        curIndex = 0;
        for(int index=0; index<5; index++) //�� ó�� Ʃ�丮�� �޴� �����ϸ� 1�� Ʃ�丮�� ���� ��Ȱ��ȭ 
        {
            if (index == 0)
            {
                ch[index].SetActive(true);
            }
            else
            {
                ch[index].SetActive(false);
            }
        }
    }

    
    void Update()
    {
        keyCheck();
    }

    public void rightClick()
    {
        if(curIndex == 4)
        {
            return; //������ ���̸� �� �̻� �� �Ѿ
        }
        else
        {
            curIndex++;
        }
        checkIndex();
    }

    public void leftClick()
    {
        if(curIndex == 0)
        {
            return;
        }
        else
        {
            curIndex--;
        }
        checkIndex();
    }

    void checkIndex()
    {
        for(int i=0; i<5; i++)
        {
            if(i == curIndex)
            {
                ch[i].SetActive(true);
            }
            else
            {
                ch[i].SetActive(false);
            }
        }
    }

    void keyCheck() //���콺 �̿ܿ� Ű���� ����Ű�ε� �޴� �ѱ� �� ����
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            rightClick();
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            leftClick();
        }
    }

    public void backToMainMenu()
    {
        SceneManager.LoadScene(0); //���� �޴��� ���ư��� 
    }

    public void startGame()
    {
        GameManager.instance.CheckSavedGame(false); //�� ���� ���� 
    }
}
