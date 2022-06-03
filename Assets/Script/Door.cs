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

    //센서 내부에 플레이어가 들어오면 ~> interactiveText를 활성화시킨다
    //센서 내부에 플레이어가 들어와 있는 상태에서 플레이어가 E를 누르면 문을 열고 interactiveText를 비활성화시킨다 
    //센서 내부에 플레이어가 들어와 있다가 나가면 interactiveText를 비활성화시킨다

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
        sensor.SetActive(true); //문이 처음 생성되면 당연히 sensor 도 있어야 함 

        if (GameManager.instance.gameData.curAchievementNum >= achievementNum)
        {
            Destroy(gameObject); //이미 플레이어가 문을 열었다면 sensor는 없애도 됨
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
