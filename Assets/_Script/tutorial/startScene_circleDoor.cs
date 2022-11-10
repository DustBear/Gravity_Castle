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

    AudioSource sound;
    AudioSource windSound;
    private void Awake()
    {
        spr = playerObj.GetComponent<SpriteRenderer>();
        thisSpr = GetComponent<SpriteRenderer>();

        sound = GetComponents<AudioSource>()[0];
        windSound = GetComponents<AudioSource>()[1];
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

    void soundPlay(AudioClip soundFile, bool isLoop)
    {
        sound.Stop();
        sound.loop = isLoop;
        sound.clip = soundFile;
        sound.Play();
    }

    IEnumerator doorOpenScene()
    {        
        spr.sortingLayerName = "stageStartPlayer"; //ó������ �÷��̾ �� �ڿ� �־�� �� 
        InputManager.instance.isInputBlocked = true; //�÷��̾� �������� ���ϰ� �� 

        UIManager.instance.FadeIn(4f); //4�ʿ� ���� ȭ�� ����� 
        yield return new WaitForSeconds(7f);

        soundPlay(doorShake, false);
        for (int index = 0; index < 3; index++) //�����ġ �۵����� ���� 
        {

            transform.position += new Vector3(0, 1, 0) * 0.06f;
            yield return new WaitForSeconds(0.06f);
            transform.position -= new Vector3(0, 1, 0) * 0.06f;
            yield return new WaitForSeconds(0.06f);          
        }

        yield return new WaitForSeconds(1f);

        soundPlay(doorRotate, true);

        for (int num=0; num<=3; num++) //�����ġ�� 4ȸ ȸ�� 
        {
            for (int index = 0; index <= doorRotateSprites.Length - 1; index++)
            {
                thisSpr.sprite = doorRotateSprites[index];
                yield return new WaitForSeconds(0.35f);
            }
        }

        soundPlay(doorOpenComplete, false);
        thisSpr.sprite = doorRotateSprites[0];
      
        yield return new WaitForSeconds(1f);

        //�����ġ ���� 
        for (int index = 0; index < doorOpenSprites.Length - 1; index++)
        {
            thisSpr.sprite = doorOpenSprites[index];
            yield return new WaitForSeconds(0.1f);
        }

        soundPlay(doorLockOpen, false);

        yield return new WaitForSeconds(2f);

        soundPlay(doorShake, false);

        for (int index=0; index<3; index++) //�� �ö󰡱� ���� ������ 
        {           
            transform.position += new Vector3(0, 1, 0) * 0.06f;
            yield return new WaitForSeconds(0.06f);
            transform.position -= new Vector3(0, 1, 0) * 0.06f;
            yield return new WaitForSeconds(0.06f);           
        }
        yield return new WaitForSeconds(1f);

        soundPlay(doorOpen, true);

        float doorMoveSpeed = doorMoveLength / doorMoveDelay;
        GetComponent<Rigidbody2D>().velocity = new Vector2(0, doorMoveSpeed);
        yield return new WaitForSeconds(doorMoveDelay);
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        soundPlay(doorOpenComplete, false);
        windSound.clip = windBlow;
        windSound.Play();

        spr.sortingLayerName = "Player"; //�÷��̾ �� ������ �� 
        yield return new WaitForSeconds(1f);

        textShow();
        InputManager.instance.isInputBlocked = false; //�÷��̾� ������ �� ���� 
    }

    void textShow()
    {
        for(int index=0; index<firstInform.Length; index++)
        {
            firstInform[index].SetActive(true);
        }
    }
}
