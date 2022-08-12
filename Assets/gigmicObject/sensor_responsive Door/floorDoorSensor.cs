using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class floorDoorSensor : MonoBehaviour
{
    //센서 위에 플레이어 or Box가 올라가면 센서가 인식해서 문을 열어줌
    public GameObject door_1;
    public GameObject door_2;
    //door_1이 x든 y든 +값으로 커지면서 열리는 문, door_2는 반대 

    public bool isDoorOpenVerticle; //true이면 세로로 열리는 문 false이면 가로로 열리는 문
    public float doorLength; //door_1, door_2 문 길이 
    public float doorOpenDelay; //문이 완전히 닫히거나 열리는데 걸리는 시간

    bool isObjectOn;
    bool isPlayerOn;

    Vector3 iniPos_door1;
    Vector3 iniPos_door2;
    //맨 처음 닫혀있는 상태에서의 door1/door2 문 위치 

    void Start()
    {
        iniPos_door1 = door_1.transform.position;
        iniPos_door2 = door_2.transform.position;
    }

    
    void Update()
    {
        doorCheck();   
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            isPlayerOn = true;
        }
        else if(collision.tag == "Platform")
        {
            isObjectOn = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            isPlayerOn = false;
        }
        else if (collision.tag == "Platform")
        {
            isObjectOn = false;
        }
    }

    void doorCheck()
    {
        if(isPlayerOn || isObjectOn) //플레이어나 박스 중 하나라도 센서 위에 올라가 있으면 문 엶
        {
            StopCoroutine(doorClose());
            StartCoroutine(doorOpen());
        }
        else //그 밖에는 문 닫음
        {
            StopCoroutine(doorOpen());
            StartCoroutine(doorClose());
        }
    }

    IEnumerator doorOpen()
    {
        if (isDoorOpenVerticle)
        {
            while (true)
            {
                float centerYpos = (iniPos_door1.y + iniPos_door2.y) / 2; //두 문의 중심위치 ~> 여기를 대칭으로 두 문이 움직임 

                door_1.transform.position = new Vector3(door_1.transform.position.x, door_1.transform.position.y + (doorLength / 100), 0);
                door_2.transform.position = new Vector3(door_2.transform.position.x, 2*centerYpos - door_1.transform.position.y, 0);
                
                if(door_1.transform.position.y >= iniPos_door1.y + doorLength)
                {
                    door_1.transform.position = new Vector3(door_1.transform.position.x, iniPos_door1.y + doorLength, 0); //도착하면 문 고정 
                    break;
                } //어차피 door2는 door1의 대칭이므로 굳이 따로 트리거 만들 필요 X 

                yield return new WaitForSeconds(doorOpenDelay / 100);
            }
        }
    }

    IEnumerator doorClose()
    {
        if (isDoorOpenVerticle)
        {
            while (true)
            {
                float centerYpos = (iniPos_door1.y + iniPos_door2.y) / 2; //두 문의 중심위치 ~> 여기를 대칭으로 두 문이 움직임 

                door_1.transform.position = new Vector3(door_1.transform.position.x, door_1.transform.position.y - (doorLength / 100), 0);
                door_2.transform.position = new Vector3(door_2.transform.position.x, 2 * centerYpos - door_1.transform.position.y, 0);
                
                if (door_1.transform.position.y <= iniPos_door1.y)
                {
                    door_1.transform.position = new Vector3(door_1.transform.position.x, iniPos_door1.y, 0); //도착하면 문 고정 
                    break;
                } //어차피 door2는 door1의 대칭이므로 굳이 따로 트리거 만들 필요 X 

                yield return new WaitForSeconds(doorOpenDelay / 100);
            }
        }
    }
}
