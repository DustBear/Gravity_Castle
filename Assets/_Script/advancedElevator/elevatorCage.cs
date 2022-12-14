using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class elevatorCage : MonoBehaviour
{
    public int purposePoint; 
    //���������Ͱ� ���� ��ǥ�� �ϴ� ������ ��ȣ
    //1�̸� Pos1, 2�̸� Pos2

    public bool isAchieved; //���������� ��ǥ ������ �����ߴ����� ���� 

    public Vector2 pos1;
    public Vector2 pos2;
    //����: �׻� '���� ��ġ ����'���� ���� ���� pos1���� �ؾ� �� 
    [SerializeField] float elevatorSpeed;   
    [SerializeField] int initialPos; //������ �� ��ġ�� ���� ~> 1�̸� Pos1, 2�̸� Pos2 �� �ȴ�

    [SerializeField] public bool isPlayerOnBell; //�÷��̾ �� ���� ���� �ִ����� ���� 
    [SerializeField] float playerAddforce; 
    //���������Ͱ� �Ʒ��� ������ �� ������ϱ� ������ �÷��̾ ���߿� �� �߰� �� ~> ������ ���� ���������� �༭ �ٴڿ� �پ��ְ� ���� 

    Rigidbody2D rigid;
    public GameObject weight; //������ ~> �� �� ��ǥ �߽ɿ� ���� ���������Ϳ� �ݴ�� �������� ��
    public GameObject bell; //���� �︱ ������ ���� �¿�� ������ ��
    public GameObject gear;
    public Animator gearAni;

    //AudioSource soundPlayer;
    //AudioSource bellSoundPlayer;   

    Quaternion initialBellRotation; //���� �� ���� ȸ����: ���������Ͱ� ������ �����ִ� ��쿡�� ���� ���������ǿ� ���� ȸ���ؾ� �� 
    [SerializeField] Vector3 addForceDir; //������������ ������ ���� �÷��̾�� ���� ���ϴ� ���⵵ �޶����� �� 

    AudioSource bellSound;
    AudioSource elevatorSound;//���, ���� ���� 
    AudioSource elevatorLoopSound;

    public AudioClip bellEffect;
    public AudioClip elevatorDepart;
    public AudioClip elevatorEffect;
    public AudioClip elevatorArrive;

    public GameObject E_icon;

    private void Awake()
    {
        //�����ϸ鼭 ���������� ��ġ �ʱ�ȭ
        if (initialPos == 1)
        {
            transform.position = pos1;
            purposePoint = 1;
        }
        else
        {
            transform.position = pos2;
            purposePoint = 2;
        }

        //soundPlayer = GetComponent<AudioSource>();
        //bellSoundPlayer = bell.GetComponent<AudioSource>();
        isAchieved = true;

        //�� ó�� ������ ���� purposePoint�� ���� ��ġ�� �����ϵ��� ���� ��� �� 

        bellSound = gameObject.AddComponent<AudioSource>();
        bellSound.loop = false;
        bellSound.playOnAwake = false;
        bellSound.clip = bellEffect;

        elevatorSound = gameObject.AddComponent<AudioSource>();       
        elevatorSound.playOnAwake = false;
        elevatorSound.loop = false;

        elevatorLoopSound = gameObject.AddComponent<AudioSource>();
        elevatorLoopSound.playOnAwake = false;
        elevatorLoopSound.loop = true;
        elevatorLoopSound.clip = elevatorEffect;
    }

    void Start()
    {
        bellSound.volume = GameManager.instance.optionSettingData.masterVolume_setting * GameManager.instance.optionSettingData.effectVolume_setting;
        elevatorSound.volume = GameManager.instance.optionSettingData.masterVolume_setting * GameManager.instance.optionSettingData.effectVolume_setting;
        elevatorLoopSound.volume = GameManager.instance.optionSettingData.masterVolume_setting * GameManager.instance.optionSettingData.effectVolume_setting;

        rigid = GetComponent<Rigidbody2D>();
        gearAni = gear.GetComponent<Animator>();
        initialBellRotation = bell.transform.rotation;

        gearAni.SetBool("gearMove", false);
        StartCoroutine(soundCheck());

        E_icon.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player" && collision.transform.up == transform.up)
        {
            //�÷��̾�� ������������ ȸ������ �ٸ��� Ȱ��ȭ �ȵ� 
            E_icon.SetActive(true);
            isPlayerOnBell = true;
        }       
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            E_icon.SetActive(false);
            isPlayerOnBell = false;           
        }
    }
    void Update()
    {       
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isPlayerOnBell) bellRing();
            //���� ���� �÷��̾ �ִ� ���·� ��ȣ�ۿ�Ű ������ �� �︮�鼭 purposePoint �ٲ� 
            E_icon.SetActive(false);
        }

        elevatorMove();
        weightMove();
    }

    
    IEnumerator soundCheck()
    {
        while (true)
        {
            if (rigid.velocity.magnitude >= 0.05f) //���������Ͱ� �����̴� ���̸� 
            {
                if (!elevatorLoopSound.isPlaying) //���� �����̴� ���尡 ��������� ������ 
                {
                    elevatorLoopSound.volume = GameManager.instance.optionSettingData.masterVolume_setting * GameManager.instance.optionSettingData.effectVolume_setting;
                    elevatorLoopSound.Play();
                }
            }
            else
            {
                if (elevatorLoopSound.isPlaying)
                {
                    yield return new WaitForSeconds(0.1f); 
                    //�̵� ���带 �ٷ� �ߴܽ�Ű�� ���� ����� �̵� ���� ���̿� ƴ�� ���� ���ڿ�������
                    //0.1�ʸ� �����̸� �� 
                    elevatorLoopSound.Stop();
                }
            }

            yield return null; //�� �����Ӹ��� ���� 
        }        
    }
    
    void elevatorMove()
    {
        if (isAchieved)
        {                                        
            return; //��ǥ ������ �����ߴٸ� ���̻� ������ �ʿ� ����
        }
            
        Vector2 dirVector;
        Vector2 moveDirection;

        if (purposePoint == 1) //���� pos2 ���� pos1���� �̵��ϰ� �ִ� ���̸� 
        {           
            gearAni.SetBool("gearMove", true);
            gearAni.SetFloat("gearSpeed", 2f);

            dirVector = pos1 - pos2;
            moveDirection = dirVector.normalized; //moveDirection = �������� �ϴ� ����(ũ��1 ���ͷ� ǥ��)
            rigid.velocity = moveDirection * elevatorSpeed; //���������� �ӵ� �Ҵ�    

            if (vectorJudge())
            {
                elevatorSound.Stop();
                elevatorSound.clip = elevatorArrive;

                elevatorSound.volume = GameManager.instance.optionSettingData.masterVolume_setting * GameManager.instance.optionSettingData.effectVolume_setting;
                elevatorSound.Play();

                gearAni.SetBool("gearMove", false);
                transform.position = pos1; //��ġ �����ϰ� 
                rigid.velocity = Vector3.zero; //�ӵ� ������Ű�� 
                isAchieved = true;
            }
        }
        else //���� pos1 ���� pos2 ���� �̵��ϰ� �ִ� ���̸� 
        {           
            gearAni.SetBool("gearMove", true);
            gearAni.SetFloat("gearSpeed", -2f);

            dirVector = pos2 - pos1;
            moveDirection = dirVector.normalized;
            rigid.velocity = moveDirection * elevatorSpeed; //���������� �ӵ� �Ҵ�    

            if (vectorJudge())
            {
                elevatorSound.Stop();
                elevatorSound.clip = elevatorArrive;

                elevatorSound.volume = GameManager.instance.optionSettingData.masterVolume_setting * GameManager.instance.optionSettingData.effectVolume_setting;
                elevatorSound.Play();

                gearAni.SetBool("gearMove", false);
                transform.position = pos2; //��ġ �����ϰ� 
                rigid.velocity = Vector3.zero; //�ӵ� ������Ű�� 
                isAchieved = true;
            }
        }
               
       
    }
  
    void bellRing() 
    {
        if (isAchieved) //������ �ִ� ���¿��� �� �︱ �� 
        {
            elevatorSound.Stop();
            elevatorSound.clip = elevatorDepart;

            elevatorSound.volume = GameManager.instance.optionSettingData.masterVolume_setting * GameManager.instance.optionSettingData.effectVolume_setting;
            elevatorSound.Play();
        }

        isAchieved = false;
        bellSound.Stop();

        bellSound.volume = GameManager.instance.optionSettingData.masterVolume_setting * GameManager.instance.optionSettingData.effectVolume_setting;
        bellSound.Play();

        /*
        if (bellSoundPlayer.isPlaying) bellSoundPlayer.Stop();
        bellSoundPlayer.Play();
        */

        //���� �︮�� ������ ������ ���� �ٽ� ����� �� �����̼��� ��ø���� �ʵ��� �� 
        StopCoroutine("bellShake");
        bell.transform.rotation = initialBellRotation;       
        StartCoroutine("bellShake");

        if (purposePoint == 1)
        {
            purposePoint = 2;           
            GameObject.Find("Player").GetComponent<Rigidbody2D>().AddForce(addForceDir * playerAddforce, ForceMode2D.Impulse);
            //pos1�� �̵��ϴ� ���� pos2�� ��ǥ �ٲ� ~> �÷��̾� ���߿� ���� �ʰ� ������ �� 
        }
        else
        {
            purposePoint = 1;         
        }
        //���������Ϳ� �޷� �ִ� ���� ���� ~> ���� purposePoint�� 1�̸� 2��, 2�̸� 1�� �ٲ��� 
    }

    IEnumerator bellShake() //���� �¿�� ��鸮�� �ִϸ��̼�
    {
        float maxRotate=30;
        float middleRotate=20;
        float minRotate=10;

        float[] bellRotation = new float[3] { maxRotate, middleRotate, minRotate };

        float curRotate = 0;
        for(int a=0; a<=2; a++)
        {
            curRotate = bellRotation[a];
            for(int b=0; b<=3; b++)
            {
                switch (b)
                {
                    case 0:
                        bell.transform.Rotate(0, 0, curRotate);
                        break;
                    case 1:
                        bell.transform.Rotate(0, 0, -curRotate);
                        break;
                    case 2:
                        bell.transform.Rotate(0, 0, -curRotate);
                        break;
                    case 3:
                        bell.transform.Rotate(0, 0, curRotate);
                        break;
                }
                yield return new WaitForSeconds(0.05f);
            }
        }       
    }

    bool vectorJudge() //���������Ͱ� ���� ���� �ִ��� �Ǵ���
    {
        Vector2 pos1Vector = pos1 - new Vector2(transform.position.x, transform.position.y);
        Vector2 pos2Vector = pos2 - new Vector2(transform.position.x, transform.position.y);

        if(Vector2.Dot(pos1Vector, pos2Vector) > 0)
        {
            //���������ͷκ��� �� �شܱ����� ���͸� ���� ���� �� �����ؼ� ����� �۵����� ��� �� ~> �����ߴ�!
            
            return true; 
        }
        else
        {
            return false;
        }
    }

    void weightMove()
    {
        Vector2 centerPos;
        Vector2 cagePos = new Vector2(transform.position.x, transform.position.y);

        centerPos = (pos1 + pos2) / 2;

        weight.transform.position = 2*centerPos - cagePos;
        //�����ߴ� �׻� ���������� ��� ���� �߽ɿ��� ��Ī�Ǵ� ��ġ�� ���� 
    }
}
