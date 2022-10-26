using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class advancedStageDoor_button : MonoBehaviour
{
    [SerializeField] bool isActived; //��ư�� �̹� �۵��ߴ����� ����
   
    public GameObject stageDoor; //�� ��ư�� ������ �������� ��
    public Sprite[] leverSpriteGroup;

    SpriteRenderer spr;
    bool isPlayerOn = false;
    public GameObject keyIcon;

    private void Awake()
    {
        spr = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        //�� ��ũ��Ʈ�� �׻� advancedStageDoor ��ũ��Ʈ���� ���߿� ����Ǿ�� �� ~> Start() �� ���� 
        if (stageDoor.GetComponent<advancedStageDoor>().disposable) //disposable ������ ���� ������ �� �׻� ��Ȱ��ȭ�� �� ~> �� button ��Ȱ��ȭ�ؾ� �� 
        {
            isActived = false;
            spr.sprite = leverSpriteGroup[0];
        }       
        else
        {
            if (GameManager.instance.gameData.curAchievementNum >= stageDoor.GetComponent<advancedStageDoor>().doorActiveThreshold)
            {
                spr.sprite = leverSpriteGroup[leverSpriteGroup.Length-1];
                isActived = true;
            }
            else
            {
                isActived = false;
            }
        }

        keyIcon.SetActive(false);
    }

    
    void Update()
    {
        if (isActived)
        {
            return; //�̹� �۵��� ��ư�̸� �ٽ� ������ �����ؾ� ��
        }

        if (isPlayerOn && Input.GetKeyDown(KeyCode.E))
        {
            isActived = true;
            stageDoor.GetComponent<advancedStageDoor>().doorMove();
            StartCoroutine(buttonSpin());
        }
    }
  
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player" && collision.transform.up == transform.up)
        {
            isPlayerOn = true;
            if (!isActived)
            {
                keyIcon.SetActive(true);
            }
        }   
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player" && collision.transform.up == transform.up)
        {
            isPlayerOn = false;
            keyIcon.SetActive(false);
        }
    }

    IEnumerator buttonSpin()
    {
        keyIcon.SetActive(false);
        for (int index=0; index<leverSpriteGroup.Length; index++)
        {
            spr.sprite = leverSpriteGroup[index];
            yield return new WaitForSeconds(0.06f);
        }
    }
}
