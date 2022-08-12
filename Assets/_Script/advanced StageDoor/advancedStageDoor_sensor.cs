using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class advancedStageDoor_sensor : MonoBehaviour
{
    [SerializeField] bool isActived; //센서가 이미 작동했는지의 여부
    public GameObject stageDoor; //이 센서가 제어할 스테이지 문
    void Start()
    {
        if (stageDoor.GetComponent<advancedStageDoor>().disposable) //disposable 설정된 씬을 시작할 때 항상 비활성화된 문 ~> 늘 센서 비활성화해야 함 
        {
            isActived = false;
        }
        else if (GameManager.instance.gameData.curAchievementNum >= stageDoor.GetComponent<advancedStageDoor>().DoorActiveTrheshold)
        {           
            isActived = true;
        }
        else
        {
            isActived = false;
        }
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isActived)
        {
            return;
        }

        if(collision.tag ==  "Player")
        {
            isActived = true;
            stageDoor.GetComponent<advancedStageDoor>().doorMove();
        }
    }
}
