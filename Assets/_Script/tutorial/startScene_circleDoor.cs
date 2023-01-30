using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class startScene_circleDoor : MonoBehaviour
{
    public GameObject playerObj;
    SpriteRenderer spr;
    SpriteRenderer thisSpr;

    public GameObject doorLock;
    [SerializeField] float doorLock_MoveLength;
    [SerializeField] float doorLock_MoveDelay;

    public GameObject doorLockChain;
    SpriteRenderer doorLockChainSpr;
    public Sprite[] doorLockChainSprite;
    [SerializeField] float doorLockChain_frameDelay;

    public GameObject leftDoor;
    public GameObject rightDoor;
    [SerializeField] float door_openLength;
    [SerializeField] float door_openDelay;

    public GameObject[] firstInform; //�� ó�� �¿�����Ű ������ ���� ������ ���� �� ���� �� 

    [SerializeField] AudioClip lockShake;
    [SerializeField] AudioClip lockMove;
    [SerializeField] AudioClip lockComplete;
    [SerializeField] AudioClip lockChainMove;
    [SerializeField] AudioClip doorMove;
    [SerializeField] AudioClip doorMove_complete;

    public AudioClip windBlow;

    public AudioSource sound;
    public AudioSource windSound;
    private void Awake()
    {
        spr = playerObj.GetComponent<SpriteRenderer>();
        doorLockChainSpr = doorLockChain.GetComponent<SpriteRenderer>();
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
            doorLock.transform.position += new Vector3(0, doorLock_MoveLength, 0);
            //ó�� ������ ���� �ƴϸ� ���� �ö��־�� �� 

            doorLockChainSpr.sprite = doorLockChainSprite[doorLockChainSprite.Length - 1];

            leftDoor.transform.position -= new Vector3(door_openLength, 0, 0);
            rightDoor.transform.position += new Vector3(door_openLength, 0, 0);
            //���� ���� ���� ���¿��� �� 

            for (int index = 0; index < firstInform.Length; index++)
            {
                firstInform[index].SetActive(true);
            }
            // Ʃ�丮�� �ؽ�Ʈ �� �־� �� 
        }
    }

    void Update()
    {
        sound.volume = GameManager.instance.optionSettingData.masterVolume_setting * GameManager.instance.optionSettingData.effectVolume_setting;
        //�ǽð� ȿ���� ���� ���� 
    }
   
    IEnumerator doorOpenScene()
    {        
        spr.sortingLayerName = "stageStartPlayer"; //ó������ �÷��̾ �� �ڿ� �־�� �� 
        spr.color = new Color(0, 0, 0); //�÷��̾�� �������̶� ������ ����
        InputManager.instance.isInputBlocked = true; //�÷��̾� �������� ���ϰ� �� 

        UIManager.instance.FadeIn(3f); //3�ʿ� ���� ȭ�� ����� 
        yield return new WaitForSeconds(9f);

        sound.PlayOneShot(lockShake);

        for (int index = 0; index < 3; index++) // doorLock ���� 
        {
            doorLock.transform.position += new Vector3(0, 1, 0) * 0.05f;
            yield return new WaitForSeconds(0.06f);
            doorLock.transform.position -= new Vector3(0, 1, 0) * 0.05f;
            yield return new WaitForSeconds(0.06f);          
        }

        yield return new WaitForSeconds(1.5f);

        //doorLockChain �ִϸ��̼� ��� 
        int animFrameCount = 0;

        for (int index = 0; index < doorLockChainSprite.Length; index++)
        {
            animFrameCount++;
            doorLockChainSpr.sprite = doorLockChainSprite[index];

            if (index == 51) //��� �����Ǵ� �Ҹ� 
            {
                sound.PlayOneShot(lockChainMove);
            }       
            yield return new WaitForSeconds(doorLockChain_frameDelay);
        }

        yield return new WaitForSeconds(1.5f);

        //doorLock �� ���� �ö� 

        sound.PlayOneShot(lockMove);
        float doorLockSpeed = doorLock_MoveLength / doorLock_MoveDelay;
        doorLock.GetComponent<Rigidbody2D>().velocity = new Vector3(0, doorLockSpeed, 0);

        yield return new WaitForSeconds(doorLock_MoveDelay);

        sound.Stop();
        sound.PlayOneShot(lockComplete);
        doorLock.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        doorLock.transform.localPosition = new Vector3(0, doorLock_MoveLength, 0);
      
        yield return new WaitForSeconds(1.5f);

        //���� ���� 
        float doorOpenSpeed = door_openLength / door_openDelay;

        sound.PlayOneShot(doorMove);
        rightDoor.GetComponent<Rigidbody2D>().velocity = new Vector3(doorOpenSpeed, 0, 0);
        leftDoor.GetComponent<Rigidbody2D>().velocity = new Vector3(-doorOpenSpeed, 0, 0);

        yield return new WaitForSeconds(door_openDelay-0.2f);

        sound.PlayOneShot(doorMove_complete);

        yield return new WaitForSeconds(0.2f);

        rightDoor.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        leftDoor.GetComponent<Rigidbody2D>().velocity = Vector3.zero;

        spr.sortingLayerName = "Player"; //�÷��̾ �� ������ �� 

        //�÷��̾ �ٽ� ������鼭 �þ߿� ���� 
        float playerFadeTime = 2f;
        float playerFade_timer = 0f;

        while(playerFade_timer < playerFadeTime)
        {
            playerFade_timer += Time.deltaTime;
            float playerColor = playerFade_timer / playerFadeTime;

            spr.color = new Color(playerColor, playerColor, playerColor);
            yield return null;
        }
        spr.color = new Color(1, 1, 1);

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
            float volumeValue = GameManager.instance.optionSettingData.masterVolume_setting * GameManager.instance.optionSettingData.effectVolume_setting;
            windSound.volume = (index / (float)frameCount) * volumeValue;
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
