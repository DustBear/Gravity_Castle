using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class advancedStageDoor_sensor : MonoBehaviour
{
    [SerializeField] bool isActived; //������ �̹� �۵��ߴ����� ����
    public int activeThreshold; //�÷��̾��� achieveNum �� �� ���� '�ʰ�' �̸� ��Ȱ��ȭ�Ѵ�.   

    public GameObject stageDoor; //�� ������ ������ �������� ��
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
