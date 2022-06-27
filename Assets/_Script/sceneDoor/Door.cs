using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    [SerializeField] int stageNum;
    public int achievementNum;
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

        if (GameManager.instance.gameData.curAchievementNum >= achievementNum)
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
        if (GameManager.instance.gameData.curAchievementNum == achievementNum - 1)
        {            
            GameManager.instance.SaveData(achievementNum, stageNum, player.transform.position); //�� �����ٴ� ���� gameData�� ����
            ani.SetBool("shouldDoorOpen", true); //�� ���� �ִϸ��̼� ����

            StartCoroutine(FadeOut());
        }
    }

    IEnumerator FadeOut()
    {
        if (achievementNum == 33 && !GameManager.instance.isCliffChecked)
        {
            InputManager.instance.isInputBlocked = true;
        }

        float aniLength = aniClip.length;
        
        yield return new WaitForSeconds(aniLength); //�� ���� �ִϸ��̼��� �� ������ ������ �۵�
        /*
        for (int i = 10; i >= 0; i--)
        {
            Color color = sprite.color;
            color.a = i / 10.0f;
            sprite.color = color;
            yield return new WaitForSeconds(0.03f); // ���̵� �ƿ� 0.3�� �ɸ�
        }
        */

        emitBurst.Play();
        gameObject.SetActive(false); //������ �� ��Ȱ��ȭ            
    }

}
