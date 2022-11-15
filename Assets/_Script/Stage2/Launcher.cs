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
    void Start()
    {
        StartCoroutine(Launch());
    }

    IEnumerator Launch()
    {
        yield return new WaitForSeconds(launchOffset);

        while (true)
        {            
            GameObject curObj = ObjManager.instance.GetObj(type); //�ʿ��� Ÿ���� �߻�ü ������ 
            curObj.transform.position = transform.position + transform.up; //��ġ�� �߻���� ��ġ�κ��� �߻�������� 1��ŭ ������ ���� ���� 
            curObj.transform.eulerAngles = transform.eulerAngles; //�߻簢�� ���� 

            if(type == ObjManager.ObjType.arrow)
            {
                curObj.GetComponent<stage2_arrow>().limitSpeed = limitSpeed_arrow;
            }

            Rigidbody2D rigid = curObj.GetComponent<Rigidbody2D>();
            
            curObj.SetActive(true);
            rigid.velocity = transform.up * fireSpeed; //�߻���� ���� ������ �߻� ���� 
            yield return new WaitForSeconds(launchPeriod);
        }
    }

}
