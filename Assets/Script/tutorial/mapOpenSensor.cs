using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class mapOpenSensor : MonoBehaviour
{
    public GameObject mapCanvas;
    public GameObject map;
    public GameObject chapter_instruction;

    //��ư ������ Ű���� �� �Ʒ��� ��ũ���ϱ� 
    public Button Button_up;
    public Button Button_down;
    public float mapMaxY;
    public float mapMinY;

    public float mapScrollSpeed; //���� �� �Ʒ��� ��ũ���ϴ� �ӵ�

    [SerializeField]bool isSensorOn;
    [SerializeField]bool shouldMapMoveUp;
    [SerializeField]bool shouldMapMoveDown;
    void Start()
    {
        mapCanvas.SetActive(false); //�����ϸ� ���� ���� ���� 
        shouldMapMoveDown = false;
        shouldMapMoveUp = false;
    }

    
    void Update()
    {
        //���� �۵����
        if(isSensorOn && Input.GetKeyDown(KeyCode.E))
        {
            mapCanvas.SetActive(true);
            InputManager.instance.isInputBlocked = true; //�� �޴� �����ִ� ������ �÷��̾� ������ �� ����
            Cursor.lockState = CursorLockMode.None; //���콺 ���۰��� 
        }

        if(isSensorOn && Input.GetKeyDown(KeyCode.Q))
        {
            mapCanvas.SetActive(false);
            InputManager.instance.isInputBlocked = true; //�� �޴� �����ִ� ������ �÷��̾� ������ �� ����
            Cursor.lockState = CursorLockMode.Locked; //���콺 ���� �Ұ���
        }

        mapMove();
    }

    void mapMove()
    {
        Vector2 mapPos = map.GetComponent<RectTransform>().position;
        if (shouldMapMoveUp)
        {
            if (mapPos.y <= mapMinY + 540) //�� ��ġ�� ���� ���ؼ����� �������� ����
            {                
                return;
            }
            map.GetComponent<RectTransform>().position = new Vector2(mapPos.x, mapPos.y - mapScrollSpeed * Time.deltaTime);
        }
        if (shouldMapMoveDown)
        {            
            if (mapPos.y >= mapMaxY + 540) //�� ��ġ�� �ְ� ���ؼ����� �������� ����
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
