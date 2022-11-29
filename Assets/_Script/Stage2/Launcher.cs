using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : MonoBehaviour
{
    [SerializeField] ObjManager.ObjType type;
    public Sprite[] spriteGroup;
    //[0]�� ���� �� �� ���� 
    //[0] ���� ���ڰ� �ö󰡸� ���� �� 
    //�ٽ� [0]���� ���ƿ��� �߻� 

    [SerializeField] float launchOffset; //���� ���� �� ���� �߻���� �ɸ��� �ð�
    [SerializeField] float launchPeriod; //����ü �߻� �ֱ�

    [SerializeField] float fireSpeed; //�߻� �� ����ü�� �ӵ�  

    [SerializeField] float limitSpeed_arrow; //ȭ���� �ִ�ӵ� 

    [SerializeField] float initTime;
    //���� �������� ���� �ð� ~> �������� �ð��� ���������� ��Ƽ� ������� ������ ���´� 
    float curTime;
    int launchIndex = 0; //�� ���� ���� �߻��� Ƚ�� 

    SpriteRenderer spr;

    private void Awake()
    {
        spr = GetComponent<SpriteRenderer>();
    }
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
            StartCoroutine(launchAnim());

            launchIndex++;
        }
    }

    void launch()
    {
        GameObject curObj = ObjManager.instance.GetObj(type); //�ʿ��� Ÿ���� �߻�ü ������ 
        curObj.transform.position = transform.position + transform.up*0.5f; //��ġ�� �߻���� ��ġ�κ��� �߻�������� 0.5��ŭ ������ ���� ���� 
        curObj.transform.eulerAngles = transform.eulerAngles; //�߻簢�� ���� 

        if (type == ObjManager.ObjType.arrow)
        {
            curObj.GetComponent<stage2_arrow>().limitSpeed = limitSpeed_arrow;
        }

        Rigidbody2D rigid = curObj.GetComponent<Rigidbody2D>();

        curObj.SetActive(true);
        rigid.velocity = transform.up * fireSpeed; //�߻���� ���� ������ �߻� ���� 
    }

    IEnumerator launchAnim() //�߻� ������ 0.15�ʵ��� �̷����. �߻��ֱ�� 0.15�ʺ��� ª���� �ȵ� 
    {
        for(int index=1; index<spriteGroup.Length; index++)
        {
            spr.sprite = spriteGroup[index];
            yield return new WaitForSeconds(0.05f);
        }

        launch(); //����ü �߻� 
        spr.sprite = spriteGroup[0];
    }
}
