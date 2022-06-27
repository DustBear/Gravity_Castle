using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class openingSceneEle : MonoBehaviour
{
    public GameObject elevatorDoor;
    public GameObject player;
    public Color playerColor; //�÷��̾� ��ο��� ���� ���� 
    public Vector2 playerPos; //�� �������� �� �÷��̾��� ��ġ 
    openingSceneDoor doorScript;

    SpriteRenderer spr; //�÷��̾� ���� 
    BoxCollider2D sensor; //�� ������������ �ݶ��̴� 

    public float speed; //���������� �ӵ� 
    public float startYpos; //���������Ͱ� �����̱� �����ϴ� Y��ǥ 
    public float finishYpos; //���������Ͱ� �����ϴ� Y��ǥ
    public bool isMove; //���������Ͱ� �����̰� �ִ���?

    public bool isActive; //���������͸� �۵����Ѿ� �ϴ����� ����
    public bool isArrived; //���������Ͱ� �����ߴ����� ����
    bool isInAvtiveChecked; //������������ ��Ȱ��ȭ �۾��� ����ƴ���?
            
    Rigidbody2D rigid;
    AudioSource sound;

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        doorScript = elevatorDoor.GetComponent<openingSceneDoor>();
        spr = player.GetComponent<SpriteRenderer>(); //spr�� �÷��̾��� ���� ��� ���� ���� 
        sensor = GetComponent<BoxCollider2D>();
        
        sensorCheck();       
    }

    void sensorCheck()
    {
        Vector2 colliderCenter = new Vector2(transform.position.x + sensor.offset.x, transform.position.y + sensor.offset.y); //������ �߽� ��ǥ 

        float rightX_pos = colliderCenter.x + sensor.size.x / 2;
        float leftX_pos = colliderCenter.x - sensor.size.x / 2;
        float topY_pos = colliderCenter.y + sensor.size.y / 2;
        float bottomY_pos = colliderCenter.y - sensor.size.y / 2;

        float playerX = playerPos.x;
        float playerY = playerPos.y;

        if((leftX_pos < playerX) && (playerX < rightX_pos))
        {
            if((bottomY_pos < playerY) && (playerY < topY_pos))
            {
                isActive = true; //�÷��̾ �ݶ��̴� �ȿ� �� ������ isActive = true
                transform.position = new Vector2(transform.position.x, startYpos);

                spr.sortingLayerID = SortingLayer.NameToID("stageStartPlayer"); //�÷��̾�� ���������� �� ���̾�� �����ؾ� ��
                spr.color = playerColor; //�����ϸ� �÷��̾� ���� ��ο�

                InputManager.instance.isJumpBlocked = true;
                elevatorMove();
            }
        }

        else //���������� �۵���ų �ʿ�x
        {
            doorScript.inactive(); //���������� �� ��Ȱ��ȭ���Ѿ� �� 
            transform.position = new Vector2(transform.position.x, finishYpos);

            spr.sortingLayerID = SortingLayer.NameToID("Player"); //�÷��̾� ���̾� �ʱ�ȭ(�÷��̾��)
            spr.color = new Color(1, 1, 1, 1); //�÷��̾� ���� �⺻������ 
            doorScript.sceneElevatorCover.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 1);         
        }
    }

    private void OnTriggerStay2D(Collider2D collision) //�� �Լ� ����ǰ� ���� update ������ ���������� �۵� ����
    {
        /*
        if (isActive)
        {
            return; //�̹� �� �� �����ؼ� isActive�� Ȱ��ȭ���״ٸ� �� �������ʹ� �ߺ��ؼ� ��� ���� �ʿ� x 
        }

        if(collision.tag == "Player")
        {
            Debug.Log("coll");
            isActive = true; //�÷��̾ ���������� ���� �����Ƿ� ���������͸� Ȱ��ȭ���Ѿ� �Ѵ�. 
        }   
        */
    }

    void Update()
    {
        /*
        if (isArrived || (!isActive))
        {
            if (isInAvtiveChecked)
            {
                return; //�̹� �ߴ� �۾��̸� �ٽ��� �ʿ� x 
            }

            doorScript.inactive(); //���������� �� ��Ȱ��ȭ���Ѿ� �� 
            transform.position = new Vector2(transform.position.x, finishYpos);

            spr.sortingLayerID = SortingLayer.NameToID("Player"); //�÷��̾� ���̾� �ʱ�ȭ(�÷��̾��)
            spr.color = new Color(1, 1, 1, 1); //�÷��̾� ���� �⺻������ 
            isInAvtiveChecked = true;

            return;
         
            //(1)�÷��̾ ���������͸� Ÿ�� �ʾƼ� �۵���ų �ʿ䰡 ���ų�
            //(2)�� ó���� �۵������� �̹� �����ߴٸ� 
            //���̻� ����� ���� ���� + ���������ʹ� �̹� �ö� �־�� �� 
        }

        else if (isActive) //�÷��̾ ���������� ���� Ÿ �־ �۵����Ѿ� �� 
        {
            spr.sortingLayerID = SortingLayer.NameToID("stageStartPlayer"); //�÷��̾�� ���������� �� ���̾�� �����ؾ� ��
            spr.color = playerColor; //�����ϸ� �÷��̾� ���� ��ο�

            elevatorMove();
        }
        */

       
        if((transform.position.y >= finishYpos)&&isMove) //�����̴ٰ� �����ϴ� ���� �ƴ϶� �̹� �����ؼ� �������� �ʴ� ���̶�� ���� �ٽ� �ø� �ʿ䰡 ���� 
        {            
            isMove = false; //isMove �� false�� ���� 

            rigid.velocity = new Vector2(0,0); //finishYpos�� �����ϸ� ���������� ���� 

            doorScript.active(); //���������� �� �����̱� ���� 
        }

        /*
        ���������Ͱ� �̹� �����ؼ� �������� �ʴ� ���´� isActive,isMove �� �� �� false�̴�. isArrived�� true�̴�.          
         */
    }

    void elevatorMove()
    {
        isMove = true;
        rigid.velocity = new Vector2(0, speed);
    }

}
