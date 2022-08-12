using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class advancedStageDoor_button : MonoBehaviour
{
    [SerializeField] float moveLength; //버튼이 눌릴 때 내려가야 하는 길이
    [SerializeField] bool isActived; //버튼이 이미 작동했는지의 여부
    
    public GameObject CollGuard;

    public GameObject stageDoor; //이 버튼이 제어할 스테이지 문
      
    void Start()
    {
        if (stageDoor.GetComponent<advancedStageDoor>().disposable) //disposable 설정된 씬을 시작할 때 항상 비활성화된 문 ~> 늘 button 비활성화해야 함 
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
            return; //이미 작동한 버튼이면 다시 눌러도 무시해야 함
        }
        
        if (collision.gameObject.tag == "Player" && collision.transform.up == transform.up) //플레이어가 버튼을 누르고 동시에 회전각도 같아야 작동 가능
        {
            isActived = true;
            GetComponent<BoxCollider2D>().enabled = false; //작동된 버튼은 걸리적거리지 않게 콜라이더 끄기 

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
