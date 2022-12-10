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
    AudioSource leverSound;
    void Start()
    {
        elevatorScript = advancedElevator.GetComponent<elevatorCage>();
        spr = GetComponent<SpriteRenderer>();
        spr.sprite = leverSprite[0];
        leverSound = GetComponent<AudioSource>();
    }
 
    void Update()
    {
        if(isPlayerOn && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine("spriteAni"); //�۵����ο� ������� ���� �ִϸ��̼��� �۵�

            Vector2 elePos = advancedElevator.transform.position;
            float dis2Pos1 = (elePos - elevatorScript.pos1).magnitude;
            float dis2Pos2 = (elePos - elevatorScript.pos2).magnitude;

            if (leverPos == 1 && dis2Pos1>0.1f) //�̹� ���������Ͱ� pos1�� ������ ��Ȳ���� �۵� x
            {               
                elevatorScript.purposePoint = 1;
                elevatorScript.isAchieved = false;
            }

            else if(leverPos == 2 && dis2Pos2>0.1f) //�̹� ���������Ͱ� pos2�� ������ ��Ȳ���� �۵� x
            {               
                elevatorScript.purposePoint = 2;
                elevatorScript.isAchieved = false;
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
        leverSound.volume = GameManager.instance.optionSettingData.masterVolume_setting * GameManager.instance.optionSettingData.effectVolume_setting;
        leverSound.Play();
        spr.sprite = leverSprite[1];
        yield return new WaitForSeconds(0.4f);

        spr.sprite = leverSprite[0];
    }
}
