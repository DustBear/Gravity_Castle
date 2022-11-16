using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveWall : MonoBehaviour
{
    [SerializeField] Vector3 pos1;
    [SerializeField] Vector3 pos2;
    public float moveTime; //�����̴� �� �ɸ��� �ð�
    public int curPos; //���� stone�� ��ġ�� ������� 
    public int initialPos; //������ ���� ��ġ 
    public bool isMoving; //�����̰� �ִ� ������ ���� ����x 

    public Sprite[] spriteGroup;
    public int cycleNum; //spriteGroup �ֱ⸦ �� �� ������ 
    SpriteRenderer spr;

    AudioSource sound;
    Rigidbody2D rigid;

    public AudioClip startSound;
    public AudioClip movingSound;
    public AudioClip arriveSound;

    private void Awake()
    {
        sound = GetComponent<AudioSource>();
        rigid = GetComponent<Rigidbody2D>();
    }
    void Start()
    {
        spr = GetComponent<SpriteRenderer>();
        spr.sprite = spriteGroup[0];

        switch (initialPos)
        {
            case 1:
                transform.position = pos1;
                curPos = 1;
                break;
            case 2:
                transform.position = pos2;
                curPos = 2;
                break;
        }
    }

    void Update()
    {
       
    }

    public void stoneMove()
    {
        if (!isMoving)
        {
            if (curPos == 1) StartCoroutine(stoneMoveCor(2));
            else if (curPos == 2) StartCoroutine(stoneMoveCor(1));
        }
    }

    IEnumerator moveAni_right()
    {
        //moveTime ���� 1���� ȸ��(�ð����)

        var waitFrame = new WaitForSeconds(moveTime / (spriteGroup.Length * cycleNum + 1));
        for(int count=1; count<=cycleNum; count++)
        {
            for (int index = 0; index < spriteGroup.Length; index++)
            {
                spr.sprite = spriteGroup[index];
                yield return waitFrame;
            }            
        }       
    }
    IEnumerator moveAni_left()
    {
        //moveTime ���� 1���� ȸ��(�ݽð����)

        var waitFrame = new WaitForSeconds(moveTime / (spriteGroup.Length * cycleNum + 1));
        for (int count=1; count<=cycleNum; count++)
        {
            for (int index = spriteGroup.Length - 1; index >= 0; index--)
            {
                spr.sprite = spriteGroup[index];
                yield return waitFrame;
            }
        }       
    }

    IEnumerator stoneMoveCor(int dirPos) //dirPos=1,2 �� �� ������ �̵� 
    {
        isMoving = true;

        float distance = (pos2 - pos1).magnitude; //�������� �� �Ÿ� 
        Vector3 direction = (pos1 - pos2).normalized; //pos1�� ��ǥ�� �� 

        sound.PlayOneShot(startSound);

        for(int index=1; index<=3; index++) //3ȸ�� ���� ���� 
        {
            transform.position += direction * 0.05f;
            yield return new WaitForSeconds(0.05f);
            transform.position -= direction * 0.05f;
            yield return new WaitForSeconds(0.05f);
        }
        yield return new WaitForSeconds(0.5f);

        if(dirPos == 2)
        {
            direction = -direction;//���� �ݴ�� �ٲ���� �� 
        }
       
        switch (dirPos) //��� ���ư��� �ִϸ��̼� ���� 
        {
            case 1:
                StartCoroutine(moveAni_right());
                break;
            case 2:
                StartCoroutine(moveAni_left());
                break;
        }

        sound.clip = movingSound;
        sound.Play();

        float moveSpeed = distance / moveTime;
        rigid.velocity = moveSpeed * direction;

        yield return new WaitForSeconds(moveTime-0.1f);

        sound.Stop();
        sound.PlayOneShot(arriveSound);

        yield return new WaitForSeconds(0.1f);
        rigid.velocity = Vector3.zero;

        switch (dirPos) //�������� ������ �߻��� �� �����Ƿ� stone�� �����ϰ� ���� ��ǥ�� ������ ������ �ش� 
        {
            case 1:
                transform.position = pos1;
                curPos = 1;
                break;
            case 2:
                transform.position = pos2;
                curPos = 2;
                break;
        }

        isMoving = false;
    }
}
