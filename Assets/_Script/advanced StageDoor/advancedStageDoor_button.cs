using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class advancedStageDoor_button : MonoBehaviour
{
    [SerializeField] float moveLength; //��ư�� ���� �� �������� �ϴ� ����
    [SerializeField] bool isActived; //��ư�� �̹� �۵��ߴ����� ����
    
    public GameObject CollGuard;

    public GameObject stageDoor; //�� ��ư�� ������ �������� ��
      
    void Start()
    {
        if (stageDoor.GetComponent<advancedStageDoor>().disposable) //disposable ������ ���� ������ �� �׻� ��Ȱ��ȭ�� �� ~> �� button ��Ȱ��ȭ�ؾ� �� 
        {
            isActived = false;
        }       
        else
        {
            if (GameManager.instance.gameData.curAchievementNum >= stageDoor.GetComponent<advancedStageDoor>().DoorActiveTrheshold)
            {
                transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y - moveLength, 0);
                isActived = true;
            }
            else
            {
                isActived = false;
            }
        }        
    }

    
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isActived) 
        {
            return; //�̹� �۵��� ��ư�̸� �ٽ� ������ �����ؾ� ��
        }
        
        if (collision.gameObject.tag == "Player" && collision.transform.up == transform.up) //�÷��̾ ��ư�� ������ ���ÿ� ȸ������ ���ƾ� �۵� ����
        {
            isActived = true;
            GetComponent<BoxCollider2D>().enabled = false; //�۵��� ��ư�� �ɸ����Ÿ��� �ʰ� �ݶ��̴� ���� 

            StartCoroutine("buttonMove");
            stageDoor.GetComponent<advancedStageDoor>().doorMove();
        }
    }

    IEnumerator buttonMove()
    {
        for(int index=1; index<=10; index++)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y - moveLength/10, 0);
            yield return new WaitForSeconds(0.03f);
        }
        CollGuard.SetActive(false);
    }
}
