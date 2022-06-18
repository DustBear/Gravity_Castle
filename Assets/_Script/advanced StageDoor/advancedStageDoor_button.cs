using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class advancedStageDoor_button : MonoBehaviour
{
    [SerializeField] float moveLength; //��ư�� ���� �� �������� �ϴ� ����
    [SerializeField] bool isActived; //��ư�� �̹� �۵��ߴ����� ����
    public int activeThreshold; //�÷��̾��� achieveNum �� �� ���� '�ʰ�' �̸� ��Ȱ��ȭ�Ѵ�.   

    public GameObject stageDoor; //�� ��ư�� ������ �������� ��
    void Start()
    {
        if(GameManager.instance.gameData.curAchievementNum > activeThreshold)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y - moveLength, 0);
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isActived) 
        {
            return; //�̹� �۵��� ��ư�̸� �ٽ� ������ �����ؾ� ��
        }
        
        if (collision.gameObject.tag == "Player" && collision.transform.rotation == transform.rotation) //�÷��̾ ��ư�� ������ ���ÿ� ȸ������ ���ƾ� �۵� ����
        {
            isActived = true;
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
