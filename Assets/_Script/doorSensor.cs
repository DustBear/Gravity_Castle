using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class doorSensor : MonoBehaviour
{
    GameObject player;
    public GameObject interText;
    GameObject door;
    bool isPlayerOn;
    void Start()
    {
        player = GameObject.Find("Player");
        door = transform.parent.gameObject;  //doorSensor �� door�� child 
        interText.SetActive(false);
        isPlayerOn = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && isPlayerOn) //�÷��̾ ���� ���� �����ִ� ���¿��� E�� ������
        {
            if (GameManager.instance.gameData.curAchievementNum == door.GetComponent<Door>().achievementNum - 1)
            {
                door.GetComponent<Door>().shouldOpen = true;
                interText.SetActive(false);
                gameObject.SetActive(false); //���� ������ ���� sensor�� ���̻� �۵��� �ʿ� ����
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {           
            interText.SetActive(true);
            isPlayerOn = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            interText.SetActive(false);
            isPlayerOn = false;
        }
    }
}
