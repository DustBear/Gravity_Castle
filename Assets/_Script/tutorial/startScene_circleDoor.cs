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

    public AudioClip activeSound;
    public AudioClip openSound;
    public AudioClip rotateSound;

    AudioSource sound;
    private void Awake()
    {
        spr = playerObj.GetComponent<SpriteRenderer>();
        thisSpr = GetComponent<SpriteRenderer>();
        sound = GetComponent<AudioSource>();
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

        UIManager.instance.FadeIn(4f); //4�ʿ� ���� ȭ�� ����� 
        yield return new WaitForSeconds(7f);

        for (int index = 0; index < 3; index++) //�����ġ �۵����� ���� 
        {
            sound.clip = activeSound;
            sound.Play();

            transform.position += new Vector3(0, 1, 0) * 0.06f;
            yield return new WaitForSeconds(0.06f);
            transform.position -= new Vector3(0, 1, 0) * 0.06f;
            yield return new WaitForSeconds(0.06f);          
        }
        yield return new WaitForSeconds(1f);

        sound.Stop();
        sound.loop = true;
        sound.clip = rotateSound;
        sound.Play();

        for (int num=0; num<=3; num++) //�����ġ�� 4ȸ ȸ�� 
        {
            for (int index = 0; index <= doorRotateSprites.Length - 1; index++)
            {
                thisSpr.sprite = doorRotateSprites[index];
                yield return new WaitForSeconds(0.35f);
            }
        }
        thisSpr.sprite = doorRotateSprites[0];

        sound.Stop();
        sound.loop = false;

        yield return new WaitForSeconds(1f);

        for (int index = 0; index <= doorOpenSprites.Length - 1; index++)
        {
            thisSpr.sprite = doorOpenSprites[index];
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(2f);
     
        sound.clip = activeSound;
        sound.Play();

        for (int index=0; index<3; index++) //�� �ö󰡱� ���� ������ 
        {           
            transform.position += new Vector3(0, 1, 0) * 0.06f;
            yield return new WaitForSeconds(0.06f);
            transform.position -= new Vector3(0, 1, 0) * 0.06f;
            yield return new WaitForSeconds(0.06f);           
        }
        yield return new WaitForSeconds(1f);

        sound.Stop();
        sound.clip = openSound;
        sound.Play();

        for (int index=0; index<200; index++) //�� �ö� 
        {
            transform.position += new Vector3(0, 1, 0) * (doorMoveLength / 200);
            yield return new WaitForSeconds(doorMoveDelay / 200);
        }

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
