using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    [SerializeField] int stageNum;

    public int achievementNeeded;
    
    //���� ���� ���̺�����Ʈ�� ����� ���� �ʴ´� 
    //���� ������ �������� Ư�� ���̺�����Ʈ ���뵵 �̻��� �� ���� ������ �� ������ ���� ���� �ִ´�
    
    public bool shouldOpen;

    Animator ani;
    public GameObject sensor;
    public ParticleSystem emitBurst;
    public AnimationClip aniClip; //�� ������ �ִϸ��̼� ~> �ִϸ��̼� ���� ���� ���� �Ķ���� 

    //���� ���ο� �÷��̾ ������ ~> interactiveText�� Ȱ��ȭ��Ų��
    //���� ���ο� �÷��̾ ���� �ִ� ���¿��� �÷��̾ E�� ������ ���� ���� interactiveText�� ��Ȱ��ȭ��Ų�� 
    //���� ���ο� �÷��̾ ���� �ִٰ� ������ interactiveText�� ��Ȱ��ȭ��Ų��

    GameObject player;
    SpriteRenderer sprite;
    [SerializeField] float doorColliderForce;

    void Awake()
    {
        player = GameObject.FindWithTag("Player");
        sprite = GetComponent<SpriteRenderer>();      
        ani = GetComponent<Animator>();

        shouldOpen = false;
    }

    void Start()
    {
        sensor.SetActive(true); //���� ó�� �����Ǹ� �翬�� sensor �� �־�� �� 

        if (GameManager.instance.gameData.curAchievementNum >= achievementNeeded)
        {
            Debug.Log("door open" + GameManager.instance.gameData.curAchievementNum);
            gameObject.SetActive(false); //���� ���൵ �� ���� �̹� �����־�� �Ѵٸ� ��Ȱ��ȭ�� 
        }
    }

    private void Update()
    {
        if (shouldOpen)
        {
            doorOpen();
        }      
    }

    void doorOpen()
    {
        if (GameManager.instance.gameData.curAchievementNum == achievementNeeded - 1)
        {            
            ani.SetBool("shouldDoorOpen", true); //�� ���� �ִϸ��̼� ����

            StartCoroutine(FadeOut());
        }
    }

    IEnumerator FadeOut()
    {
        /*
        if (achievementNum == 33 && !GameManager.instance.isCliffChecked)
        {
            InputManager.instance.isInputBlocked = true;
        }
        */

        float aniLength = aniClip.length;
        
        yield return new WaitForSeconds(aniLength); //�� ���� �ִϸ��̼��� �� ������ ������ �۵�
       
        emitBurst.Play();
        gameObject.SetActive(false); //������ �� ��Ȱ��ȭ            
    }

    
}
