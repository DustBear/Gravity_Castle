using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class temporalSensor : MonoBehaviour
{
    //�÷��̾��� interactive Text�� ���� ������ �� ����ؼ� ��µǴ� ���װ� ���� 
    //�ذ��ϱ⿣ �ð��� �����ϹǷ� �ϴ� �÷��̾��� �������� �ٷ� ���� temporal collider �� ���� �� 
    //�� collider�� ����� �� interactive Text�� �� �ִ� ������� �ϴ� �ذ�����.
    //���Ŀ� �ð��� �鿩 ��ĥ ��
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            collision.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(false);
        }
    }
}
