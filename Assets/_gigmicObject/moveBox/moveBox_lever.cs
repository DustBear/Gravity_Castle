using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveBox_lever : MonoBehaviour
{
    public GameObject moveStone;

    Rigidbody2D rigid;
    SpriteRenderer spr;
    SpriteRenderer boxSpr;

    public Sprite[] spriteGroup; //[0]=idle  [1]=left  [2]=right
    public Sprite[] boxSprites; //�����̴� moveBox�� sprite 
    [SerializeField] int curSpriteIndex;

    public float accelSpeed; //���ӵ� 
    public float maxSpeed;

    [SerializeField] bool isPlayerOn;
    [SerializeField] bool isLeverAct;

    bool isMoveHorizontal; //true �̸� box �� ���η� ������ 

    //pos1�� ��ġ�� pos2 ���� Ŀ�� ��(x�� y�� �� �� �ش�)
    public Vector2 pos1;
    public Vector2 pos2;

    public GameObject leverArrow;
   
    bool isClockCorWorking = false;
    bool isReClockCorWorking = false;

    IEnumerator clockCor;
    IEnumerator reclockCor;

    public AudioClip activeSound;
    public AudioClip moveSound;

    AudioSource sound;
    private void Awake()
    {
        rigid = moveStone.GetComponent<Rigidbody2D>();
        rigid.bodyType = RigidbodyType2D.Kinematic;

        spr = GetComponent<SpriteRenderer>();
        boxSpr = moveStone.GetComponent<SpriteRenderer>();
        sound = GetComponent<AudioSource>();

        isPlayerOn = false;
        isLeverAct = false;
    }
    void Start()
    {
        if (pos1.x == pos2.x) //x��ǥ�� ������ ���η� �����̴� �� 
        {
            isMoveHorizontal = false;
        }
        else
        {
            isMoveHorizontal = true;
        }
        spr.sprite = spriteGroup[0];
        leverArrow.SetActive(false);

        curSpriteIndex = 0;

        clockCor = gear_clockWise();
        reclockCor = gear_reClockWise();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player") isPlayerOn = true;
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player") isPlayerOn = false;
        rigid.velocity = Vector3.zero; //�÷��̾ ���� ���� ������ ������ stone �� ���� 
        spr.sprite = spriteGroup[0];
    }
    void Update()
    {
        stoneMove();       
    }

    void stoneMove()
    {
        if (!isPlayerOn) return; //�÷��̾ ���� �ȿ� ������ ���� 

        if (isPlayerOn && Input.GetKeyDown(KeyCode.E)) //�÷��̾ �νĹ��� ������ E ������ Ȱ��ȭ ~> ��Ȱ��ȭ / ��Ȱ��ȭ ~> Ȱ��ȭ ���·� ���� 
        {
            sound.Stop();
            sound.clip = activeSound;
            sound.Play();

            if (!isLeverAct)
            {
                isLeverAct = true;
                leverArrow.SetActive(true);
                rigid.bodyType = RigidbodyType2D.Dynamic;
                rigid.gravityScale = 0;
                InputManager.instance.isInputBlocked = true;
            }
            else
            {
                isLeverAct = false;
                leverArrow.SetActive(false);
                rigid.bodyType = RigidbodyType2D.Kinematic;
                InputManager.instance.isInputBlocked = false;
            }
        }

        if (isLeverAct)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                sound.Stop();
                sound.clip = moveSound;
                sound.Play();
            }

            if (Input.GetAxisRaw("Horizontal") == -1) // [<--] ������ ~> ���� �۾����� �������� ������ 
            {
                //�̹� pos2 ��ǥ�� �����ߴٸ� �� �̻� pos2 �������δ� �������� ���� 
                if ((isMoveHorizontal && moveStone.transform.position.x <= pos2.x) || (!isMoveHorizontal && moveStone.transform.position.y <= pos2.y))
                {
                    rigid.velocity = Vector3.zero;
                    moveStone.transform.position = pos2;

                    StopCoroutine(reclockCor); //�ݽð���� ȸ�� ���� 
                    isReClockCorWorking = false;
                    return;
                }

                if (isMoveHorizontal) //�����̵�
                {
                    if (rigid.velocity.magnitude >= maxSpeed) //�ִ�ӵ��� �����ߴٸ� �� �̻� �� ������ ���� 
                    {
                        return;
                    }

                    rigid.AddForce(new Vector2(-1, 0) * accelSpeed, ForceMode2D.Impulse);
                    if (!isReClockCorWorking) StartCoroutine("gear_reClockWise"); //�ݽð���� ȸ�� ���� 
                }
                else //�����̵� 
                {
                    if (rigid.velocity.magnitude >= maxSpeed)
                    {
                        return;
                    }

                    rigid.AddForce(new Vector2(0, -1) * accelSpeed, ForceMode2D.Impulse);
                    if (!isReClockCorWorking) StartCoroutine("gear_reClockWise"); //�ݽð���� ȸ�� ���� 
                }

                spr.sprite = spriteGroup[1];
            }
            else if (Input.GetAxisRaw("Horizontal") == 1) // [-->] �� ������ ~> ���� Ŀ���� �������� ������ 
            {
                //�̹� pos1 ��ǥ�� �����ߴٸ� �� �̻� pos1 �������δ� �������� ���� 
                if ((isMoveHorizontal && moveStone.transform.position.x >= pos1.x) || (!isMoveHorizontal && moveStone.transform.position.y >= pos1.y))
                {
                    rigid.velocity = Vector3.zero;
                    moveStone.transform.position = pos1;

                    StopCoroutine(clockCor); //�ð���� ȸ�� ���� 
                    isClockCorWorking = false;
                    return;
                }

                if (isMoveHorizontal)
                {
                    if (rigid.velocity.magnitude >= maxSpeed)
                    {
                        return;
                    }
                    rigid.AddForce(new Vector2(1, 0) * accelSpeed, ForceMode2D.Impulse);
                    if (!isClockCorWorking) StartCoroutine("gear_clockWise"); //�ð���� ȸ�� ���� 
                }
                else
                {
                    if (rigid.velocity.magnitude >= maxSpeed)
                    {
                        return;
                    }
                    rigid.AddForce(new Vector2(0, 1) * accelSpeed, ForceMode2D.Impulse);
                    if (!isClockCorWorking) StartCoroutine("gear_clockWise"); //�ð���� ȸ�� ���� 
                }

                spr.sprite = spriteGroup[2];
            }
            else //�ƹ��� Ű�� ������ ���� �� 
            {
                rigid.velocity = Vector3.zero;
                spr.sprite = spriteGroup[0];

                if (isClockCorWorking)
                {
                    StopCoroutine(clockCor);
                    isClockCorWorking = false;
                }
                if (isReClockCorWorking)
                {
                    StopCoroutine(reclockCor);
                    isReClockCorWorking = false;
                }
            }
        }
    }

    IEnumerator gear_clockWise() //�ð���� ȸ�� 
    {
        isClockCorWorking = true;
        while (true)
        {
            if (isClockCorWorking == false)
            {
                yield break;
            }
            if (curSpriteIndex == boxSprites.Length - 1)
            {
                curSpriteIndex = 0; //sprite �׷� ������ ���� �ٽ� 0���� ���ƿ� 
            }
            else
            {
                curSpriteIndex++;
            }

            boxSpr.sprite = boxSprites[curSpriteIndex];
            yield return new WaitForSeconds(0.08f);
        }
    }

    IEnumerator gear_reClockWise() //�ݽð���� ȸ�� 
    {
        isReClockCorWorking = true;
        while (true)
        {
            if (isReClockCorWorking == false)
            {
                yield break;
            }
            if (curSpriteIndex == 0)
            {
                curSpriteIndex = boxSprites.Length - 1;
            }
            else
            {
                curSpriteIndex--;
            }

            boxSpr.sprite = boxSprites[curSpriteIndex];           
            yield return new WaitForSeconds(0.08f);
        }
    }
}
