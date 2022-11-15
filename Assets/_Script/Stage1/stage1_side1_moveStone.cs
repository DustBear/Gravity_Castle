using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stage1_side1_moveStone : MonoBehaviour
{
    [SerializeField] float moveTime; //stone�� �������� �����ϴ� �� �ɸ��� �ð�
    float moveSpeed; //�̵��Ÿ��� moveTime���� ���� �� 

    [SerializeField] float limitSpeed; //������ �� ���ϼӵ� ���� 
    [SerializeField] float floatTime; //stone�� �ְ����� �����ϰ� ������ �ִ� �ð�

    [SerializeField] Vector3 startPos;
    [SerializeField] Vector3 finishPos;

    public bool shouldStoneStart;
    public bool shouldStoneMove;
    public bool shouldStoneFall;
    public Sprite[] spriteGroup; //[0]�� �� ���� ���� [1]~[14] �� ���� ������ ������ ����(14 frame) 
    Rigidbody2D rigid;

    float fallDelay; //stone�� ������ �� �ɸ� �ð� 
    SpriteRenderer spr;
   
    public AudioSource sound;
    public AudioSource loopSound;

    public AudioClip act_ready;
    public AudioClip lightOn;
    public AudioClip act_start;
    public AudioClip moving;
    public AudioClip arrive;
    public AudioClip blink_turnOn;
    public AudioClip blink_turnOff;
    public AudioClip windBlow;
    public AudioClip crush;

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        spr = GetComponent<SpriteRenderer>();

        shouldStoneStart = false;
        shouldStoneMove = false;

        moveTime = moveTime - 0.82f;
        moveSpeed = (finishPos - startPos).magnitude / moveTime; //moveSpeed�� moveTime�� ���� �޶����� 

        spr.sprite = spriteGroup[0]; //�� ���� ���� 
    }

    void Update()
    {
        fallDelay += Time.deltaTime;
        if (shouldStoneStart)
        {
            StartCoroutine(stoneStart());
        }       
    }

    IEnumerator stoneStart()
    {
        shouldStoneStart = false;

        Vector3 vibrateDir = (startPos - finishPos).normalized;
        sound.PlayOneShot(act_ready);

        //stone�� �����̱� �� �� �Ʒ��� ��¦ �����ϸ鼭 ������ 
        transform.position += vibrateDir * 0.05f;
        spr.sprite = spriteGroup[spriteGroup.Length - 1];
        sound.PlayOneShot(lightOn);
        yield return new WaitForSeconds(0.08f);

        transform.position -= vibrateDir * 0.05f;
        yield return new WaitForSeconds(0.08f);

        transform.position -= vibrateDir * 0.05f;
        spr.sprite = spriteGroup[0];
        yield return new WaitForSeconds(0.08f);

        transform.position += vibrateDir * 0.05f;
        yield return new WaitForSeconds(0.08f);

        yield return new WaitForSeconds(0.5f); //������ ���� 0.82�ʰ� ������ ���� �����̱� ����

        //stone ������ ������ ��ǥ�������� �����̱� ���� 
        shouldStoneMove = true;

        sound.PlayOneShot(act_start);

        loopSound.clip = moving;
        loopSound.Play();

        while (shouldStoneMove)
        {
            stoneMove();            
            yield return null;
        }
        
    }

    float spriteTimer = 0; //stone�� �����̴� ���� Ÿ�̸Ӱ� ���ư��� ���൵�� ���� sprite �ٲ��� 
    int spriteIndex = 1;

    void stoneMove()
    {        
        spriteTimer += Time.deltaTime;
        rigid.velocity = (finishPos - startPos).normalized * moveSpeed;

        if(spriteTimer >= moveTime / (spriteGroup.Length - 1))
        {
            if(spriteIndex < spriteGroup.Length - 1)
            {
                spriteIndex++;
                spr.sprite = spriteGroup[spriteIndex];
                spriteTimer = 0; //Ÿ�̸� �ʱ�ȭ 
            }            
        }

        Vector3 toStartPos = startPos - transform.position;
        Vector3 toFinishPos = finishPos - transform.position;

        if((Vector3.Dot(toStartPos,toFinishPos) > 0) && rigid.velocity.magnitude > 0) 
            //stone���κ��� finishPos ������ ������ ������ stone-startPos ������ ����� ���� stone�� �ӵ��� 0�̻��̸� ��ǥ������ �����Ѱ����� ��
        {
            spriteTimer = 0;
            shouldStoneMove = false;
            rigid.velocity = Vector3.zero;
            transform.position = finishPos;
            spriteIndex = 1;

            sound.PlayOneShot(arrive);
            loopSound.Stop();
            StartCoroutine("stoneFloat");
        }
    }
    
    IEnumerator stoneFloat() //stone�� ���߿� �ӹ��� ���¿��� ��� �ӹ����ٰ� ������
    {
        for (int index = 1; index <= 4; index++) //4�� ������ ���� ���� 
        {
            yield return new WaitForSeconds(floatTime / 8);
            spr.sprite = spriteGroup[0];
            sound.PlayOneShot(blink_turnOff);

            yield return new WaitForSeconds(floatTime / 8);
            spr.sprite = spriteGroup[spriteGroup.Length-1];
            sound.PlayOneShot(blink_turnOn);
        }

        shouldStoneFall = true;
        fallDelay = 0; //Ÿ�̸� �ʱ�ȭ 

        loopSound.clip = windBlow;
        loopSound.Play();

        while (shouldStoneFall)
        {
            stoneFall();
            yield return null;
        }
    }
 
    void stoneFall()
    {
        int gravityScale = 3;
        if (rigid.velocity.y <= -limitSpeed) //�ӵ� ���ѿ� �����ϸ� �ӵ� ���� 
        {
            rigid.velocity = -(finishPos - startPos).normalized * limitSpeed;
        }
        else
        {
            float fallSpeed = gravityScale * 4.9f * fallDelay*fallDelay; //���ӵ� 9.8�� ��ӵ��(1/2*t^2�� ���)
            rigid.velocity = -(finishPos - startPos).normalized * fallSpeed;
        }
        
        
        Vector3 toStartPos = startPos - transform.position;
        Vector3 toFinishPos = finishPos - transform.position;

        if ((Vector3.Dot(toStartPos, toFinishPos) > 0) && rigid.velocity.magnitude > 0)
        //stone���κ��� finishPos ������ ������ ������ stone-startPos ������ ����� ���� stone�� �ӵ��� 0�̻��̸� ��ǥ������ �����Ѱ����� ��
        {
            shouldStoneFall = false;
            rigid.velocity = Vector3.zero;
            transform.position = startPos;
            spr.sprite = spriteGroup[0];

            loopSound.Stop();
            sound.PlayOneShot(crush);
            UIManager.instance.cameraShake(0.3f, 0.4f);            
        }
    }
}
