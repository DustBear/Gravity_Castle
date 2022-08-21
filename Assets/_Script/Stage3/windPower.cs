using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class windPower : MonoBehaviour
{
    GameObject playerObj;
    Player playerScript;
    Rigidbody2D rigid;
    public float windForceSize; //�÷��̾�� windZone ������ �������� ���ӵ� 
    public float maxWindForce; //�÷��̾ windZone ������ ������ �ְ� �ӵ� 

    public float InertialTime = 1; //�������� ����Ǵ� �ð� 
    public float timer; //�÷��̾ �������� �޴� �ð� Ÿ�̸�  
    bool shouldTimerWork;
    float exitPlayerVel; //�÷��̾ windZone�� ���������� ���� ������ �ӵ� 

    bool shouldInertialWork; //�������� �ۿ��ؾ� �ϴ����� ���� 

    private void Awake()
    {
        playerObj = GameObject.Find("Player");
        shouldTimerWork = false;

        rigid = playerObj.GetComponent<Rigidbody2D>();
        playerScript = playerObj.GetComponent<Player>();
    }

    private void OnTriggerEnter2D(Collider2D collision) //�� windZone ���� ������ windForce�� player ��ũ��Ʈ�� ���� 
    {
        playerScript.windForce = windForceSize;
        playerScript.maxWindSpeed = maxWindForce;
    }

    //�÷��̾�� ������ �¿� ȭ��ǥ�� ���ؼ��� horizontal �� ������ �ӵ��� ������ 
    //horizontal wind �� ������ �޾� �����ϴٰ� ������� ����� ���� ��ٷ� �����ϰ� ��. 
    //���ڿ������� ������ ������ ������ �޴� ��� ������ �ʿ䰡 ����

    //�÷��̾ windZone �� ����� �������� windForceSize �� ����Ͽ� x�� �������� �߰����� �ӵ��� ���� 
    //�߰����� �ӵ��� �ð��� ������ ���ݾ� �پ��
    //�÷��̾ ���� ��ų� ���� �ε����� ���� �ӵ� �߰��� ���� 
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(gameObject.tag == "UpWind" || gameObject.tag == "DownWind") 
        {
            if(playerObj.transform.up == new Vector3(1,0,0) || playerObj.transform.up == new Vector3(-1, 0, 0)) //�÷��̾ ������/�������� ������ �� 
            {
                shouldInertialWork = true;
            }
            else
            {
                shouldInertialWork = false;
            }
        }
        else
        {
            if (playerObj.transform.up == new Vector3(0, 1, 0) || playerObj.transform.up == new Vector3(0, -1, 0)) //�÷��̾ ����/�Ʒ������� ������ ��
            {
                shouldInertialWork = true;
            }
            else
            {
                shouldInertialWork = false;
            }
        }

        if (!shouldInertialWork) //�÷��̾�� wind�� transform.up ������ 1�� �ƴ� ~> ������ 0 ~> �ٶ��� �÷��̾��� x ������ �δ� �� 
        {           
            return;
        }
        else //�÷��̾�� wind �� ������ ������ �� ~> �÷��̾�� x �� �������� �߰��ӵ��� �޾ƾ� �� 
        {
            playerScript.isPlayerExitFromWInd = true;

            shouldTimerWork = true;
            timer = InertialTime; //Ÿ�̸� �ʱ�ȭ 
            exitPlayerVel = transform.InverseTransformDirection(rigid.velocity).x; //�÷��̾ windZone �� ���������� ���� x�� ������ �ӵ�
        }
    }

    private void Update()
    {        
        if (shouldTimerWork)
        {
            timer -= Time.deltaTime; //Ÿ�̸� �ð� ��� �پ�� 
            float addedVel = exitPlayerVel * (timer / InertialTime); //���ӵǴ� addedVel �� �ð��� ������ ���������� �޾� ���� 

            Vector2 locVel = transform.InverseTransformDirection(rigid.velocity); //locvel �� �÷��̾��� ������ǥ�� ���� �̵�����    
            if(Mathf.Abs(locVel.x) < 0.1f) //�ӵ��� �������� �۾����ٴ� ���� ���� �ε��� �����شٴ� �� ~> addedVel �� ������� �Ѵ� 
            {
                playerScript.isPlayerExitFromWInd = false; //�ٽ� player ������ �ֵ����� Player ��ũ��Ʈ�� ������ 
                timer = 0;
                shouldTimerWork = false;

                return;
            }

            locVel = new Vector2(InputManager.instance.horizontal * playerScript.walkSpeed + addedVel, locVel.y); //locVel �� ���ο� addedVel ���� �� 

            rigid.velocity = transform.TransformDirection(locVel); //rigid �� ������ǥ �ӵ� �Ҵ�

            if (timer <= 0 || playerObj.GetComponent<Player>().isGrounded) //Ÿ�̸Ӱ� �� �������ų� �÷��̾ ���鿡 �����ϸ� �ӵ� �߰� ���� 
            {
                playerScript.isPlayerExitFromWInd = false; //�ٽ� player ������ �ֵ����� Player ��ũ��Ʈ�� ������ 
                timer = 0;
                shouldTimerWork = false;
            }
        }      
    }
}
