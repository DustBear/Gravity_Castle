using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class elevatorLever : MonoBehaviour
{
    public GameObject advancedElevator;
    elevatorCage elevatorScript;
    SpriteRenderer spr;

    [SerializeField] int leverPos; //�� ������ pos1, pos2 �� ���� ���� ����� �������� �ν� 
    [SerializeField] bool isPlayerOn; //�÷��̾ ���� ������ �۵���ų �� �ִ� ��ġ�� �ִ°�

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
            StartCoroutine("spriteAni"); //�۵����ο� ������� ���� �ִϸ��̼��� �۵�

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

    IEnumerator spriteAni() //��������Ʈ ���������� �ִϸ��̼� ����
    {
        spr.sprite = leverSprite[1];
        yield return new WaitForSeconds(0.4f);

        spr.sprite = leverSprite[0];
    }
}
