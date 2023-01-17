using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class openingElevator_v2 : MonoBehaviour
{
    public Vector2 pos1; //�������� �� ��ġ
    public Vector2 pos2; //������ �� ��ġ 
    public float moveSpeed;
    
    public GameObject playerObj;
    Rigidbody2D rigid;
    
    bool isElevatorArrived; //���������Ͱ� pos2�� �����ߴ����� ���� 

    AudioSource sound;

    public AudioClip moveSound;
    public AudioClip arriveSound;
    public AudioClip doorSound;

    public Sprite[] elevatorDoorSprites;
    public GameObject elevatorDoor;

    SpriteRenderer spr;
    bool isOpeningSceneElevator; //�� ó�� ���� ������ ���� ������ ���������ʹ� �ڵ常 �۵��ϰ� �ִϸ��̼��� ���� x
    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        sound = GetComponent<AudioSource>();

        spr = GetComponent<SpriteRenderer>();
        if(SceneManager.GetActiveScene().buildIndex == 4)
        {
            isOpeningSceneElevator = true;
        }
        else
        {
            isOpeningSceneElevator = false;
        }
    }
    void Start()
    {
        if (GameManager.instance.gameData.UseOpeningElevetor_bool) //���������͸� ������� ������ GM �� ���� 
        {
            transform.position = pos1;
            isElevatorArrived = false;
            GameManager.instance.nextPos = pos1 + new Vector2(0, 3f); //���������� ���� �÷��̾� ���� 
            GameManager.instance.nextGravityDir = new Vector2(0, -1); //�߷� ���� ������ �� 

            rigid.velocity = new Vector2(0, -moveSpeed);

            if (!isOpeningSceneElevator)
            {
                sound.volume = GameManager.instance.optionSettingData.masterVolume_setting * GameManager.instance.optionSettingData.effectVolume_setting;
                sound.clip = moveSound;
                sound.Play();
            }
           
            spr.sprite = elevatorDoorSprites[0];
            elevatorDoor.SetActive(true);

            string ToJsonData = JsonUtility.ToJson(GameManager.instance.gameData);
            string filePath = Application.persistentDataPath + GameManager.instance.gameDataFileNames[GameManager.instance.curSaveFileNum];
            File.WriteAllText(filePath, ToJsonData);
        }
        else
        {
            transform.position = pos2;
            isElevatorArrived = true;

            spr.sprite = elevatorDoorSprites[elevatorDoorSprites.Length-1];
            elevatorDoor.SetActive(false);
        }

        InputManager.instance.isInputBlocked = false; //���� �����ϸ� inputBlock ���� 
    }
  
    void Update()
    {
        if (isElevatorArrived) return;
        stopCheck();
    }

    void stopCheck()
    {
        if(transform.position.y <= pos2.y)
        {
            rigid.velocity = Vector2.zero;
            isElevatorArrived = true;

            //���������͸� ��������� �� ������ ���̺�����Ʈ�� �̿��ؾ� �� 
            GameManager.instance.gameData.UseOpeningElevetor_bool = false;
            GameManager.instance.gameData.SpawnSavePoint_bool = false;

            if (!isOpeningSceneElevator)
            {
                UIManager.instance.cameraShake(0.5f, 0.4f);

                sound.Stop();

                sound.volume = GameManager.instance.optionSettingData.masterVolume_setting * GameManager.instance.optionSettingData.effectVolume_setting;
                sound.PlayOneShot(arriveSound);
            }

            StartCoroutine(doorOpen());
        }
    }

    IEnumerator doorOpen() //���������� �����ϸ� ���� ���� 
    {
        if (!isOpeningSceneElevator)
        {
            sound.volume = GameManager.instance.optionSettingData.masterVolume_setting * GameManager.instance.optionSettingData.effectVolume_setting;
            sound.PlayOneShot(doorSound);
        }
        
        for (int index = 0; index < elevatorDoorSprites.Length - 1; index++)
        {
            spr.sprite = elevatorDoorSprites[index];
            yield return new WaitForSeconds(0.1f);
        }

        elevatorDoor.SetActive(false);
    }
}
