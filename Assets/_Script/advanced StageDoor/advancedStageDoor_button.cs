using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class advancedStageDoor_button : MonoBehaviour
{
    [SerializeField] float moveLength; //버튼이 눌릴 때 내려가야 하는 길이
    [SerializeField] bool isActived; //버튼이 이미 작동했는지의 여부
    public int activeThreshold; //플레이어의 achieveNum 이 이 숫자 '초과' 이면 비활성화한다.   

    public GameObject CollGuard;

    public GameObject stageDoor; //이 버튼이 제어할 스테이지 문

    public bool isOnSideStage;
    [SerializeField] int doorNum; //1부터 시작
    //이 stageDoor가 side stage내에 있는지의 여부 체크 ~> 해당 번호의 sideStage가 활성화되어 있을 때에만 버튼이 눌린 상태로 존재 

    public bool disposable;
    //이 조건이 설정된 문은 사이드 스테이지 내의 퍼즐기믹에 포함된 문으로 리스폰할 때 마다 '매번' 원 상태로 초기화된다
    void Start()
    {
        if (disposable)
        {
            isActived = false;
        }

        else if (isOnSideStage) //사이드스테이지의 stageDoor을 여는 문이면 
        {
            if (GameManager.instance.gameData.sideStageUnlock[doorNum - 1]) //해당 사이드스테이지가 이미 unlock 된 상태면
            {                
                transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y-moveLength, 0);
                isActived = true;
            }
            else
            {
                //해당 스테이지가 아직 unlock되지 않은상태면
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
            return; //이미 작동한 버튼이면 다시 눌러도 무시해야 함
        }
        
        if (collision.gameObject.tag == "Player" && collision.transform.up == transform.up) //플레이어가 버튼을 누르고 동시에 회전각도 같아야 작동 가능
        {
            isActived = true;
            GetComponent<BoxCollider2D>().enabled = false; //작동된 버튼은 걸리적거리지 않게 콜라이더 끄기 
            CollGuard.SetActive(false);

            if (isOnSideStage)
            {
                GameManager.instance.gameData.sideStageUnlock[doorNum - 1] = true; //사이드 스테이지 입구로 들어가는 문을 열면 unlock 됨
            }         
            //이후 수정 필요 
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
