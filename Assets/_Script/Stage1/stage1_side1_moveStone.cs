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
    GameObject cameraObj;

    public AudioClip moveSound;
    public AudioClip smashSound;
    public AudioClip bibSound;

    AudioSource sound;
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        spr = GetComponent<SpriteRenderer>();
        cameraObj = GameObject.Find("Main Camera");
        sound = GetComponent<AudioSource>();

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

        sound.Stop();
        sound.clip = bibSound;
        sound.Play();

        Vector3 vibrateDir = (startPos - finishPos).normalized;

        //stone�� �����̱� �� �� �Ʒ��� ��¦ �����ϸ鼭 ������ 
        transform.position += vibrateDir * 0.05f;
        spr.sprite = spriteGroup[spriteGroup.Length - 1]; 
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

        sound.Stop();
        sound.clip = moveSound;
        sound.Play();

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

            StartCoroutine("stoneFloat");
        }
    }
    
    IEnumerator stoneFloat() //stone�� ���߿� �ӹ��� ���¿��� ��� �ӹ����ٰ� ������
    {
        for (int index = 1; index <= 4; index++) //4�� ������ ���� ���� 
        {
            yield return new WaitForSeconds(floatTime / 8);
            spr.sprite = spriteGroup[0];
            yield return new WaitForSeconds(floatTime / 8);
            spr.sprite = spriteGroup[spriteGroup.Length-1];
        }

        shouldStoneFall = true;
        fallDelay = 0; //Ÿ�̸� �ʱ�ȭ 

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

            cameraObj.GetComponent<MainCamera>().cameraShake(0.3f, 0.4f);
            sound.Stop();
            sound.clip = smashSound;
            sound.Play();
        }
    }
}
