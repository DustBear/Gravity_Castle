using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class mapOpenSensor : MonoBehaviour
{
    public GameObject mapCanvas;
    public GameObject map;
    public GameObject chapter_instruction;

    //버튼 눌러서 키보드 위 아래로 스크롤하기 
    public Button Button_up;
    public Button Button_down;
    public float mapMaxY;
    public float mapMinY;

    public float mapScrollSpeed; //맵을 위 아래로 스크롤하는 속도

    [SerializeField]bool isSensorOn;
    [SerializeField]bool shouldMapMoveUp;
    [SerializeField]bool shouldMapMoveDown;
    void Start()
    {
        mapCanvas.SetActive(false); //시작하면 맵은 끄고 시작 
        shouldMapMoveDown = false;
        shouldMapMoveUp = false;
    }

    
    void Update()
    {
        //센서 작동기능
        if(isSensorOn && Input.GetKeyDown(KeyCode.E))
        {
            mapCanvas.SetActive(true);
            InputManager.instance.isInputBlocked = true; //맵 메뉴 열려있는 동안은 플레이어 움직일 수 없음
            Cursor.lockState = CursorLockMode.None; //마우스 조작가능 
        }

        if(isSensorOn && Input.GetKeyDown(KeyCode.Q))
        {
            mapCanvas.SetActive(false);
            InputManager.instance.isInputBlocked = true; //맵 메뉴 열려있는 동안은 플레이어 움직일 수 없음
            Cursor.lockState = CursorLockMode.Locked; //마우스 조작 불가능
        }

        mapMove();
    }

    void mapMove()
    {
        Vector2 mapPos = map.GetComponent<RectTransform>().position;
        if (shouldMapMoveUp)
        {
            if (mapPos.y <= mapMinY + 540) //맵 위치가 최저 기준선보다 낮아지면 정지
            {                
                return;
            }
            map.GetComponent<RectTransform>().position = new Vector2(mapPos.x, mapPos.y - mapScrollSpeed * Time.deltaTime);
        }
        if (shouldMapMoveDown)
        {            
            if (mapPos.y >= mapMaxY + 540) //맵 위치가 최고 기준선보다 높아지면 정지
            {
                return;
            }
            
            map.GetComponent<RectTransform>().position = new Vector2(mapPos.x, mapPos.y + mapScrollSpeed * Time.deltaTime);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            isSensorOn = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            isSensorOn = false;
        }
    }

    public void mapScroll_up()
    {
        shouldMapMoveUp = true;
    }
    public void mapScroll_down()
    {
        shouldMapMoveDown = true;
    }

    public void mapScrollPause()
    {
        shouldMapMoveUp = false;
        shouldMapMoveDown = false;
    }
}
