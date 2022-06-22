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
                //door 가 비활성화되면 그 자식 오브젝트인 door sensor 와 스크립트도 같이 비활성화 
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
