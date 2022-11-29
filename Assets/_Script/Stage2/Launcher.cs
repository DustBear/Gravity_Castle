using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : MonoBehaviour
{
    [SerializeField] ObjManager.ObjType type;

    [SerializeField] float launchOffset; //���� ���� �� ���� �߻���� �ɸ��� �ð�
    [SerializeField] float launchPeriod; //����ü �߻� �ֱ�

    [SerializeField] float fireSpeed; //�߻� �� ����ü�� �ӵ�  

    [SerializeField] float limitSpeed_arrow; //ȭ���� �ִ�ӵ� 

    [SerializeField] float initTime;
    //���� �������� ���� �ð� ~> �������� �ð��� ���������� ��Ƽ� ������� ������ ���´� 
    float curTime;
    int launchIndex = 0; //�� ���� ���� �߻��� Ƚ�� 
    void Start()
    {
        initTime = Time.time;
    }

    private void FixedUpdate()
    {
        LaunchManager();
    }
   
    void LaunchManager()
    {      
        curTime = Time.time; //���� �ð�
        if (curTime - initTime >= launchIndex * launchPeriod + launchOffset)
        {
            launch();
            launchIndex++;
        }
    }

    void launch()
    {
        GameObject curObj = ObjManager.instance.GetObj(type); //�ʿ��� Ÿ���� �߻�ü ������ 
        curObj.transform.position = transform.position + transform.up; //��ġ�� �߻���� ��ġ�κ��� �߻�������� 1��ŭ ������ ���� ���� 
        curObj.transform.eulerAngles = transform.eulerAngles; //�߻簢�� ���� 

        if (type == ObjManager.ObjType.arrow)
        {
            curObj.GetComponent<stage2_arrow>().limitSpeed = limitSpeed_arrow;
        }

        Rigidbody2D rigid = curObj.GetComponent<Rigidbody2D>();

        curObj.SetActive(true);
        rigid.velocity = transform.up * fireSpeed; //�߻���� ���� ������ �߻� ���� 
    }


}
