using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class advancedStageDoor_sensor : MonoBehaviour
{
    [SerializeField] bool isActived; //센서가 이미 작동했는지의 여부
    public int activeThreshold; //플레이어의 achieveNum 이 이 숫자 '초과' 이면 비활성화한다.   

    public GameObject stageDoor; //이 센서가 제어할 스테이지 문
    void Start()
    {
        if (GameManager.instance.gameData.curAchievementNum > activeThreshold)
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
