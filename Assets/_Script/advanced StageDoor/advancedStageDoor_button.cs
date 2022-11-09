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

    AudioSource sound;
    public AudioClip popUpSound;
    public AudioClip rotateSound;
    private void Awake()
    {
        spr = GetComponent<SpriteRenderer>();
        sound = GetComponent<AudioSource>();
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
                sound.Stop();
                sound.clip = popUpSound;
                sound.Play();
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
        sound.Stop();
        sound.clip = rotateSound;
        sound.Play();

        keyIcon.SetActive(false);
        InputManager.instance.isInputBlocked = true; //���� ������ ������ �÷��̾� �������� ���� 
        for (int index=0; index<leverSpriteGroup.Length; index++)
        {
            spr.sprite = leverSpriteGroup[index];
            yield return new WaitForSeconds(0.06f);
        }

        InputManager.instance.isInputBlocked = false;
    }
}
