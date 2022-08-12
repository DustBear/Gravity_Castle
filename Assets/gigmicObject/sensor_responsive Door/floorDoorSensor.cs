using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class floorDoorSensor : MonoBehaviour
{
    //���� ���� �÷��̾� or Box�� �ö󰡸� ������ �ν��ؼ� ���� ������
    public GameObject door_1;
    public GameObject door_2;
    //door_1�� x�� y�� +������ Ŀ���鼭 ������ ��, door_2�� �ݴ� 

    public bool isDoorOpenVerticle; //true�̸� ���η� ������ �� false�̸� ���η� ������ ��
    public float doorLength; //door_1, door_2 �� ���� 
    public float doorOpenDelay; //���� ������ �����ų� �����µ� �ɸ��� �ð�

    bool isObjectOn;
    bool isPlayerOn;

    Vector3 iniPos_door1;
    Vector3 iniPos_door2;
    //�� ó�� �����ִ� ���¿����� door1/door2 �� ��ġ 

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
        if(isPlayerOn || isObjectOn) //�÷��̾ �ڽ� �� �ϳ��� ���� ���� �ö� ������ �� ��
        {
            StopCoroutine(doorClose());
            StartCoroutine(doorOpen());
        }
        else //�� �ۿ��� �� ����
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
                float centerYpos = (iniPos_door1.y + iniPos_door2.y) / 2; //�� ���� �߽���ġ ~> ���⸦ ��Ī���� �� ���� ������ 

                door_1.transform.position = new Vector3(door_1.transform.position.x, door_1.transform.position.y + (doorLength / 100), 0);
                door_2.transform.position = new Vector3(door_2.transform.position.x, 2*centerYpos - door_1.transform.position.y, 0);
                
                if(door_1.transform.position.y >= iniPos_door1.y + doorLength)
                {
                    door_1.transform.position = new Vector3(door_1.transform.position.x, iniPos_door1.y + doorLength, 0); //�����ϸ� �� ���� 
                    break;
                } //������ door2�� door1�� ��Ī�̹Ƿ� ���� ���� Ʈ���� ���� �ʿ� X 

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
                float centerYpos = (iniPos_door1.y + iniPos_door2.y) / 2; //�� ���� �߽���ġ ~> ���⸦ ��Ī���� �� ���� ������ 

                door_1.transform.position = new Vector3(door_1.transform.position.x, door_1.transform.position.y - (doorLength / 100), 0);
                door_2.transform.position = new Vector3(door_2.transform.position.x, 2 * centerYpos - door_1.transform.position.y, 0);
                
                if (door_1.transform.position.y <= iniPos_door1.y)
                {
                    door_1.transform.position = new Vector3(door_1.transform.position.x, iniPos_door1.y, 0); //�����ϸ� �� ���� 
                    break;
                } //������ door2�� door1�� ��Ī�̹Ƿ� ���� ���� Ʈ���� ���� �ʿ� X 

                yield return new WaitForSeconds(doorOpenDelay / 100);
            }
        }
    }
}
