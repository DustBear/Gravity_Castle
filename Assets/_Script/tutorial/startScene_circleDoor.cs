using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class startScene_circleDoor : MonoBehaviour
{
    public GameObject playerObj;
    SpriteRenderer spr;
    SpriteRenderer thisSpr;

    public float doorMoveLength;
    public float doorMoveDelay;

    public Sprite[] lock_1;
    public Sprite[] lock_2;
    public Sprite[] lock_3;

    public GameObject[] firstInform; //맨 처음 좌우조작키 설명은 문이 완전히 열린 뒤 떠야 함 

    public AudioClip doorShake;
    public AudioClip doorRotate;
    public AudioClip doorRotateComplete;
    public AudioClip doorLockOpen;
    public AudioClip doorOpen;
    public AudioClip doorOpenComplete;
    public AudioClip windBlow;

    public AudioSource sound;
    public AudioSource windSound;
    private void Awake()
    {
        spr = playerObj.GetComponent<SpriteRenderer>();
        thisSpr = GetComponent<SpriteRenderer>();
    }
    void Start()
    {
        if(GameManager.instance.gameData.curAchievementNum == 0) //해당 세이브파일을 처음 시작할 때만 사용 
        {
            StartCoroutine(doorOpenScene());
            for (int index = 0; index < firstInform.Length; index++)
            {
                firstInform[index].SetActive(false);
            }
        }
        else
        {
            transform.position += new Vector3(0, doorMoveLength, 0); //처음 시작한 것이 아니면 문은 올라가있어야 함 
            for (int index = 0; index < firstInform.Length; index++)
            {
                firstInform[index].SetActive(true);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
   
    IEnumerator doorOpenScene()
    {        
        spr.sortingLayerName = "stageStartPlayer"; //처음에는 플레이어가 문 뒤에 있어야 함 
        InputManager.instance.isInputBlocked = true; //플레이어 움직이지 못하게 함 

        UIManager.instance.FadeIn(3f); //4초에 걸쳐 화면 밝아짐 
        yield return new WaitForSeconds(6f);

        for (int index = 0; index < 3; index++) //진동 
        {

            transform.position += new Vector3(0, 1, 0) * 0.06f;
            yield return new WaitForSeconds(0.06f);
            transform.position -= new Vector3(0, 1, 0) * 0.06f;
            yield return new WaitForSeconds(0.06f);          
        }

        yield return new WaitForSeconds(1f);

        //lock_1 애니메이션 
        for (int index = 0; index < lock_1.Length; index++)
        {
            thisSpr.sprite = lock_1[index];
            yield return new WaitForSeconds(0.15f);
        }

        yield return new WaitForSeconds(1.5f);

        //lock_2 애니메이션
        for (int index = 0; index < lock_2.Length; index++)
        {
            thisSpr.sprite = lock_2[index];
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(1.5f);

        //lock_3 애니메이션 
        for (int index = 0; index < lock_3.Length; index++)
        {
            thisSpr.sprite = lock_3[index];
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(1.5f);

        for (int index = 0; index < 3; index++) //진동 
        {

            transform.position += new Vector3(0, 1, 0) * 0.06f;
            yield return new WaitForSeconds(0.06f);
            transform.position -= new Vector3(0, 1, 0) * 0.06f;
            yield return new WaitForSeconds(0.06f);
        }

        yield return new WaitForSeconds(1f);


        StartCoroutine(windSoundGen(5f, 50));

        float doorMoveSpeed = doorMoveLength / doorMoveDelay;
        GetComponent<Rigidbody2D>().velocity = new Vector2(0, doorMoveSpeed);

        yield return new WaitForSeconds(doorMoveDelay -1f);
        sound.Stop();
        sound.PlayOneShot(doorOpenComplete);

        yield return new WaitForSeconds(1f);

        GetComponent<Rigidbody2D>().velocity = Vector2.zero;


        spr.sortingLayerName = "Player"; //플레이어가 문 앞으로 옴 
        yield return new WaitForSeconds(1f);

        textShow();
        InputManager.instance.isInputBlocked = false; //플레이어 움직일 수 있음 
    }

    IEnumerator windSoundGen(float delay, int frameCount)
    {
        windSound.clip = windBlow;
        windSound.Play();
        var delayFrame = new WaitForSeconds(delay / frameCount);

        for (int index=1; index<=frameCount; index++)
        {
            windSound.volume = index / (float)frameCount;
            yield return delayFrame;
        }
    }

    void textShow()
    {
        for(int index=0; index<firstInform.Length; index++)
        {
            firstInform[index].SetActive(true);
        }
    }
}
