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
        door = transform.parent.gameObject;  //doorSensor 는 door의 child 
        interText.SetActive(false);
        isPlayerOn = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && isPlayerOn) //플레이어가 센서 내에 들어와있는 상태에서 E를 누르면
        {
            if (GameManager.instance.gameData.curAchievementNum == door.GetComponent<Door>().achievementNum - 1)
            {
                door.GetComponent<Door>().shouldOpen = true;
                interText.SetActive(false);
                gameObject.SetActive(false); //문이 열리고 나면 sensor는 더이상 작동할 필요 없음
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
