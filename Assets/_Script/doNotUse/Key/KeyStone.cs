using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyStone : MonoBehaviour
{
    GameObject playerObj;
    
    SpriteRenderer spr;
    public Sprite[] spritesGroup;
    [SerializeField] int spriteIndex;

    //������������ keyStone �������� �ٸ��� �Ź� ������ �ִϸ����͸� ����� ���ŷο� ����� ��������Ʈ ��ü�� �̿��� ������
    //sprite 0: idle
    //sprite 0~4: active
    //sprite 4~0: deactive
    //sprite 4 ~> 0: strike

    [SerializeField] bool shouldStoneActive;
    [SerializeField] bool isPlayerInSensor;

    public GameObject keyObj;
    Key keyScript;

    void Start()
    {
        spr = GetComponent<SpriteRenderer>();
        playerObj = GameObject.Find("Player").gameObject;
        isPlayerInSensor = false;
        keyScript = keyObj.GetComponent<Key>();

        spr.sprite = spritesGroup[0]; //idle
        spriteIndex = 0;

        if (GameManager.instance.gameData.curAchievementNum >= keyScript.achievementNum)
        {
            //���� �̹� �÷��̾ ���踦 ȹ���� ��Ȳ�̸� �۵����� �ʾƾ� ��
            shouldStoneActive = false;
        }
        else
        {
            shouldStoneActive = true;
        }
    }


    void Update()
    {
        if (shouldStoneActive)
        {
            SensorCheck();
        }
    }

    IEnumerator stoneActive() //�� �Ӹ��κ��� ���� �ö󰡸� Ȱ��ȭ
    {
        while (spriteIndex < 4)
        {
            spriteIndex++;
            spr.sprite = spritesGroup[spriteIndex];

            if (!isPlayerInSensor)
            {
                StartCoroutine("stoneDeActive"); //�ö󰡴� ���� �÷��̾ ���� ������ ������ �ش� ���� ���߰� ��Ȱ��ȭ���� ����
                break;
            }
            yield return new WaitForSeconds(0.1f); //0.5�� �� ������ �ö�
        }
    }

    IEnumerator stoneDeActive() //�� �Ӹ��κ��� �Ʒ��� �������� Ȱ��ȭ
    {
        while (spriteIndex > 0)
        {
            spriteIndex--;
            spr.sprite = spritesGroup[spriteIndex];          
            yield return new WaitForSeconds(0.1f); //0.5�� �� ������ ������
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!shouldStoneActive)
        {
            return;
        }

        if(collision.tag == "Player" && transform.eulerAngles.z == playerObj.transform.eulerAngles.z)
        {
            //������ �÷��̾�� ��� �ְ� ȸ������ ���� �� ���� ON
            isPlayerInSensor = true;
            StartCoroutine("stoneActive");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!shouldStoneActive)
        {
            return;
        }
        isPlayerInSensor = false;
        StartCoroutine("stoneDeActive");
    }

    void SensorCheck()
    {
        if (!isPlayerInSensor)
        {
            return; //�÷��̾ ���� ���� ���� ���� �۵���
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            spr.sprite = spritesGroup[0]; //strike 

            keyScript.StartCoroutine(keyScript.burst()); //���� ���� 
            shouldStoneActive = false; //�� �� �۵������� ��Ȱ��ȭ�ؾ� ��
        }
    }
}
