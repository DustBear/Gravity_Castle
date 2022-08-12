using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class advancedStageDoor_sensor : MonoBehaviour
{
    [SerializeField] bool isActived; //������ �̹� �۵��ߴ����� ����
    public GameObject stageDoor; //�� ������ ������ �������� ��
    void Start()
    {
        if (stageDoor.GetComponent<advancedStageDoor>().disposable) //disposable ������ ���� ������ �� �׻� ��Ȱ��ȭ�� �� ~> �� ���� ��Ȱ��ȭ�ؾ� �� 
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
