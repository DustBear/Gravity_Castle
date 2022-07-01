using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class elevatorLever : MonoBehaviour
{
    public GameObject advancedElevator;
    elevatorCage elevatorScript;
    SpriteRenderer spr;

    [SerializeField] int leverPos; //이 레버가 pos1, pos2 중 어디로 오게 만드는 레버인지 인식 
    [SerializeField] bool isPlayerOn; //플레이어가 지금 레버를 작동시킬 수 있는 위치에 있는가

    public Sprite[] leverSprite;
    void Start()
    {
        elevatorScript = advancedElevator.GetComponent<elevatorCage>();
        spr = GetComponent<SpriteRenderer>();
        spr.sprite = leverSprite[0];
    }
 
    void Update()
    {
        if(isPlayerOn && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine("spriteAni"); //작동여부에 상관없이 레버 애니메이션은 작동

            elevatorScript.isAchieved = false;
            if (leverPos == 1)
            {               
                elevatorScript.purposePoint = 1;
            }

            else if(leverPos == 2)
            {               
                elevatorScript.purposePoint = 2;
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player" && collision.transform.up == transform.up)
        {
            isPlayerOn = true;
        }       
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            isPlayerOn = false;
        }
    }

    IEnumerator spriteAni() //스프라이트 움직임으로 애니메이션 구현
    {
        spr.sprite = leverSprite[1];
        yield return new WaitForSeconds(0.4f);

        spr.sprite = leverSprite[0];
    }
}
