using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class doorSensor : MonoBehaviour
{
    GameObject player;
    public GameObject keyIcon;
    public GameObject door;
    bool isPlayerOn;
    bool isDoorOpen;

    SpriteRenderer keySpr;
    [SerializeField] Sprite[] keySprites = new Sprite[5]; //0�� ��� �ڹ���, 4�� ������ ���� �ڹ���
    [SerializeField] float keyShakeSize; //�ڹ��谡 ��鸮�� ����
    [SerializeField] bool isOntheAction; //���� Ű ��鸮�ų� �μ����� �ִϸ��̼��� ���������� 

    Vector3 keyInitialPos;
    void Start()
    {
        player = GameObject.Find("Player");      
        keyIcon.SetActive(false);
        isPlayerOn = false;

        keySpr = keyIcon.GetComponent<SpriteRenderer>();
        keySpr.sprite = keySprites[0];

        keyInitialPos = keyIcon.transform.position;
        isOntheAction = false;

        if (GameManager.instance.gameData.curAchievementNum >= door.GetComponent<Door>().achievementNeeded)
        {
            isDoorOpen = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && isPlayerOn) //�÷��̾ ���� ���� �����ִ� ���¿��� E�� ������
        {
            if (isDoorOpen) return;

            if (GameManager.instance.gameData.curAchievementNum == door.GetComponent<Door>().achievementNeeded - 1) //���൵�� ���� ����� �ϴ� ��Ȳ�̸�
            {
                if (!isOntheAction)
                {
                    StartCoroutine("keyOpen");
                }
                //door �� ��Ȱ��ȭ�Ǹ� �� �ڽ� ������Ʈ�� door sensor �� ��ũ��Ʈ�� ���� ��Ȱ��ȭ 
            }
            else //���൵�� ���� �� �� ���µ� ������ �ϸ� key �� ����� ��
            {
                StopCoroutine("keyShakeCoroutine"); //�̹� �������̴� shake ������ �ִٸ� ����ؾ� ��
                StartCoroutine("keyShakeCoroutine");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            if (isDoorOpen) return;

            keyIcon.SetActive(true);
            isPlayerOn = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (isDoorOpen) return;

            keyIcon.SetActive(false);
            isPlayerOn = false;
        }
    }

    public IEnumerator keyOpen() //sceneDoor�� �� �� ���� ��~> ���谡 ������ ����� �� �� ���� ���� ����
    {
        isOntheAction = true;

        for(int index=0; index<=3; index++) //key icon ����(0 > 1 > 2 > 3)
        {
            keySpr.sprite = keySprites[index];
            yield return new WaitForSeconds(0.1f);
        }

        keySpr.sprite = keySprites[4];
        yield return new WaitForSeconds(0.3f); //������ sprite�� ���� �� ��� �����ؾ� �� 

        keyIcon.gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic; //���� Ǯ���� ���� �߷��ۿ� �޾Ƽ� ����

        Vector3 addForceDir;
        addForceDir = keyIcon.transform.up.normalized;

        keyIcon.gameObject.GetComponent<Rigidbody2D>().AddForce(addForceDir*4, ForceMode2D.Impulse);
        for(int index=1; index<=5; index++) //���� ȸ��
        {
            keyIcon.transform.Rotate(0, 0, 9f);
            yield return new WaitForSeconds(0.02f);
        }
        
        door.GetComponent<Door>().shouldOpen = true; //�� ���� 
        yield return new WaitForSeconds(1f);
        keyIcon.SetActive(false); //key icon ��
    }

    public IEnumerator keyShakeCoroutine() //sceneDoor�� �� �� ���� �� 
    {
        keyIcon.transform.position = keyInitialPos; //Ű ��ġ �ʱ�ȭ

        for(int index=3; index>=1; index--)
        {
            //0.2�ʰ� 1ȸ �¿�պ�

            /*
            keyIcon.transform.Rotate(0, 0, 10f * index); //10n �� ȸ�� 
            yield return new WaitForSeconds(0.05f);
            keyIcon.transform.Rotate(0, 0, -10f * index); //-10n �� ȸ�� 
            yield return new WaitForSeconds(0.05f);
            keyIcon.transform.Rotate(0, 0, -10f * index); //-10n �� ȸ�� 
            yield return new WaitForSeconds(0.05f);
            keyIcon.transform.Rotate(0, 0, 10f * index); //10n �� ȸ�� 
            yield return new WaitForSeconds(0.05f);*/

            keyIcon.transform.Translate(new Vector3(keyShakeSize * index, 0, 0));
            yield return new WaitForSeconds(0.05f);
            keyIcon.transform.Translate(new Vector3(-keyShakeSize * index, 0, 0));
            yield return new WaitForSeconds(0.05f);
            keyIcon.transform.Translate(new Vector3(-keyShakeSize * index, 0, 0));
            yield return new WaitForSeconds(0.05f);
            keyIcon.transform.Translate(new Vector3(keyShakeSize * index, 0, 0));
            yield return new WaitForSeconds(0.05f);
        }
    }
}
