using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

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

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        sound = GetComponent<AudioSource>();
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

            sound.clip = moveSound;
            sound.Play();

            //���������͸� ��������� �� ������ ���̺�����Ʈ�� �̿��ؾ� �� 
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

            UIManager.instance.cameraShake(0.5f, 0.4f);

            sound.Stop();
            sound.PlayOneShot(arriveSound);
        }
    }
}
