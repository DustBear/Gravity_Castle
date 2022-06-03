using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    [SerializeField] int stageNum;
    public int achievementNum;
    public bool shouldOpen;

    public GameObject sensor;

    //���� ���ο� �÷��̾ ������ ~> interactiveText�� Ȱ��ȭ��Ų��
    //���� ���ο� �÷��̾ ���� �ִ� ���¿��� �÷��̾ E�� ������ ���� ���� interactiveText�� ��Ȱ��ȭ��Ų�� 
    //���� ���ο� �÷��̾ ���� �ִٰ� ������ interactiveText�� ��Ȱ��ȭ��Ų��

    GameObject player;
    SpriteRenderer sprite;
    BoxCollider2D collid;

    void Awake()
    {
        player = GameObject.FindWithTag("Player");
        sprite = GetComponent<SpriteRenderer>();
        collid = GetComponent<BoxCollider2D>();
        sensor = transform.GetChild(0).gameObject;

        shouldOpen = false;
    }

    void Start()
    {
        sensor.SetActive(true); //���� ó�� �����Ǹ� �翬�� sensor �� �־�� �� 

        if (GameManager.instance.gameData.curAchievementNum >= achievementNum)
        {
            Destroy(gameObject); //�̹� �÷��̾ ���� �����ٸ� sensor�� ���ֵ� ��
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
            StartCoroutine(FadeOut());
            GameManager.instance.SaveData(achievementNum, stageNum, player.transform.position);
        }
    }

    IEnumerator FadeOut()
    {
        for (int i = 10; i >= 0; i--)
        {
            Color color = sprite.color;
            color.a = i / 10.0f;
            sprite.color = color;
            yield return new WaitForSeconds(0.1f);
        }
        collid.isTrigger = true;
        if (achievementNum == 33 && !GameManager.instance.isCliffChecked)
        {
            InputManager.instance.isInputBlocked = true;
        }
    }

}
