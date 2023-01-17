using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class openingElevator_v2 : MonoBehaviour
{
    public Vector2 pos1; //내려오기 전 위치
    public Vector2 pos2; //내려온 후 위치 
    public float moveSpeed;
    
    public GameObject playerObj;
    Rigidbody2D rigid;
    
    bool isElevatorArrived; //엘리베이터가 pos2에 도착했는지의 여부 

    AudioSource sound;

    public AudioClip moveSound;
    public AudioClip arriveSound;
    public AudioClip doorSound;

    public Sprite[] elevatorDoorSprites;
    public GameObject elevatorDoor;

    SpriteRenderer spr;
    bool isOpeningSceneElevator; //맨 처음 게임 실행할 때는 오프닝 엘리베이터는 코드만 작동하고 애니메이션은 동작 x
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
        if (GameManager.instance.gameData.UseOpeningElevetor_bool) //엘리베이터를 사용할지 말지는 GM 이 결정 
        {
            transform.position = pos1;
            isElevatorArrived = false;
            GameManager.instance.nextPos = pos1 + new Vector2(0, 3f); //엘리베이터 위에 플레이어 생성 
            GameManager.instance.nextGravityDir = new Vector2(0, -1); //중력 방향 설정해 줌 

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

        InputManager.instance.isInputBlocked = false; //씬이 시작하면 inputBlock 해제 
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

            //엘리베이터를 사용했으면 그 다음은 세이브포인트를 이용해야 함 
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

    IEnumerator doorOpen() //엘리베이터 도착하면 문이 열림 
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
