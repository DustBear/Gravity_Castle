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
        if (Input.GetKeyDown(KeyCode.E) && isPlayerOn)
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
            //interText = collision.transform.GetChild(0).GetChild(0).gameObject; //플레이어 ~> 캔버스 ~> 텍스트 로 추적함 
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
