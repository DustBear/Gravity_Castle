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

    public Sprite[] doorRotateSprites;
    public Sprite[] doorOpenSprites;

    public GameObject[] firstInform; //�� ó�� �¿�����Ű ������ ���� ������ ���� �� ���� �� 

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
        if(GameManager.instance.gameData.curAchievementNum == 0) //�ش� ���̺������� ó�� ������ ���� ��� 
        {
            StartCoroutine(doorOpenScene());
            for (int index = 0; index < firstInform.Length; index++)
            {
                firstInform[index].SetActive(false);
            }
        }
        else
        {
            transform.position += new Vector3(0, doorMoveLength, 0); //ó�� ������ ���� �ƴϸ� ���� �ö��־�� �� 
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
        spr.sortingLayerName = "stageStartPlayer"; //ó������ �÷��̾ �� �ڿ� �־�� �� 
        InputManager.instance.isInputBlocked = true; //�÷��̾� �������� ���ϰ� �� 

        UIManager.instance.FadeIn(3f); //4�ʿ� ���� ȭ�� ����� 
        yield return new WaitForSeconds(7f);

        sound.PlayOneShot(doorShake);
        for (int index = 0; index < 3; index++) //�����ġ �۵����� ���� 
        {

            transform.position += new Vector3(0, 1, 0) * 0.06f;
            yield return new WaitForSeconds(0.06f);
            transform.position -= new Vector3(0, 1, 0) * 0.06f;
            yield return new WaitForSeconds(0.06f);          
        }

        yield return new WaitForSeconds(1f);

        sound.clip = doorRotate;
        sound.Play();
        for (int num=0; num<=1; num++) //�����ġ�� 2ȸ ȸ�� 
        {
            for (int index = 0; index <= doorRotateSprites.Length - 1; index++)
            {
                thisSpr.sprite = doorRotateSprites[index];
                yield return new WaitForSeconds(0.35f);
            }
        }

        sound.Stop();
        sound.PlayOneShot(doorRotateComplete);
        thisSpr.sprite = doorRotateSprites[0];
      
        yield return new WaitForSeconds(1f);

        //�����ġ ���� 
        for (int index = 0; index < doorOpenSprites.Length - 1; index++)
        {
            thisSpr.sprite = doorOpenSprites[index];
            yield return new WaitForSeconds(0.1f);
        }

        sound.PlayOneShot(doorLockOpen);

        yield return new WaitForSeconds(1f);

        sound.PlayOneShot(doorShake);

        for (int index=0; index<3; index++) //�� �ö󰡱� ���� ������ 
        {           
            transform.position += new Vector3(0, 1, 0) * 0.06f;
            yield return new WaitForSeconds(0.06f);
            transform.position -= new Vector3(0, 1, 0) * 0.06f;
            yield return new WaitForSeconds(0.06f);           
        }
        yield return new WaitForSeconds(1f);

        sound.Stop();
        sound.clip = doorOpen;
        sound.Play();

        StartCoroutine(windSoundGen(5f, 50));

        float doorMoveSpeed = doorMoveLength / doorMoveDelay;
        GetComponent<Rigidbody2D>().velocity = new Vector2(0, doorMoveSpeed);

        yield return new WaitForSeconds(doorMoveDelay -1f);
        sound.Stop();
        sound.PlayOneShot(doorOpenComplete);

        yield return new WaitForSeconds(1f);

        GetComponent<Rigidbody2D>().velocity = Vector2.zero;


        spr.sortingLayerName = "Player"; //�÷��̾ �� ������ �� 
        yield return new WaitForSeconds(1f);

        textShow();
        InputManager.instance.isInputBlocked = false; //�÷��̾� ������ �� ���� 
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
