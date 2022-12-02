using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

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

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        sound = GetComponent<AudioSource>();
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

            sound.clip = moveSound;
            sound.Play();

            //엘리베이터를 사용했으면 그 다음은 세이브포인트를 이용해야 함 
            GameManager.instance.gameData.UseOpeningElevetor_bool = false;
            GameManager.instance.shouldUseOpeningElevator = false;
            GameManager.instance.gameData.SpawnSavePoint_bool = true;
            GameManager.instance.shouldSpawnSavePoint = true;

            string ToJsonData = JsonUtility.ToJson(GameManager.instance.gameData);
            string filePath = Application.persistentDataPath + GameManager.instance.gameDataFileNames[GameManager.instance.curSaveFileNum];
            File.WriteAllText(filePath, ToJsonData);
        }
        else
        {
            transform.position = pos2;
            isElevatorArrived = true;
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

            UIManager.instance.cameraShake(0.5f, 0.4f);

            sound.Stop();
            sound.PlayOneShot(arriveSound);
        }
    }
}
