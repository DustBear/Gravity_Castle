using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class advancedStageDoor_button : MonoBehaviour
{
    [SerializeField] float moveLength; //��ư�� ���� �� �������� �ϴ� ����
    [SerializeField] bool isActived; //��ư�� �̹� �۵��ߴ����� ����
    public int activeThreshold; //�÷��̾��� achieveNum �� �� ���� '�ʰ�' �̸� ��Ȱ��ȭ�Ѵ�.   

    public GameObject CollGuard;

    public GameObject stageDoor; //�� ��ư�� ������ �������� ��

    public bool isOnSideStage;
    [SerializeField] int doorNum; //1���� ����
    //�� stageDoor�� side stage���� �ִ����� ���� üũ ~> �ش� ��ȣ�� sideStage�� Ȱ��ȭ�Ǿ� ���� ������ ��ư�� ���� ���·� ���� 

    public bool disposable;
    //�� ������ ������ ���� ���̵� �������� ���� �����Ϳ� ���Ե� ������ �������� �� ���� '�Ź�' �� ���·� �ʱ�ȭ�ȴ�
    void Start()
    {
        if (disposable)
        {
            isActived = false;
        }

        else if (isOnSideStage) //���̵彺�������� stageDoor�� ���� ���̸� 
        {
            if (GameManager.instance.gameData.sideStageUnlock[doorNum - 1]) //�ش� ���̵彺�������� �̹� unlock �� ���¸�
            {                
                transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y-moveLength, 0);
                isActived = true;
            }
            else
            {
                //�ش� ���������� ���� unlock���� �������¸�
                isActived = false;             
            }
        }

        else
        {
            if (GameManager.instance.gameData.curAchievementNum > activeThreshold)
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
            CollGuard.SetActive(false);

            if (isOnSideStage)
            {
                GameManager.instance.gameData.sideStageUnlock[doorNum - 1] = true; //���̵� �������� �Ա��� ���� ���� ���� unlock ��
            }         
            //���� ���� �ʿ� 
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
    }
}
