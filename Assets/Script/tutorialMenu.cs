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
    public Button gameStartButton; //scene01 로 이동함

    [SerializeField]int curIndex; //현재 열려 있는 index
    void Start()
    {
        curIndex = 0;
        for(int index=0; index<5; index++) //맨 처음 튜토리얼 메뉴 시작하면 1번 튜토리얼 빼고 비활성화 
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
            return; //마지막 장이면 더 이상 안 넘어감
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

    void keyCheck() //마우스 이외에 키보드 방향키로도 메뉴 넘길 수 있음
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
        SceneManager.LoadScene(0); //메인 메뉴로 돌아가기 
    }

    public void startGame()
    {
        GameManager.instance.CheckSavedGame(false); //새 게임 시작 
    }
}
