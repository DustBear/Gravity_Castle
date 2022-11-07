using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class magnaticObj : MonoBehaviour
{
    Rigidbody2D rigid;

    [SerializeField] bool isContact; //���� �ű׳�ƽ�ʵ忡 ����ִ����� ���� 
    [SerializeField] bool isMagnetic; //���� �ű׳�ƽ�ʵ忡 �ڱ����� �ɷ� �ִ����� ����
    bool isMagnetic_last;

    GameObject magneticField;
    Vector3 initLocalPos;

    public int activeAchNum; //���� ������ Ǯ�� �ִ� ���ȸ� �۵� 

    [SerializeField] bool isTopMag;
    [SerializeField] bool isRightMag;
    [SerializeField] bool isBottomMag;
    [SerializeField] bool isLeftMag;

    public float xSize; //�� ������Ʈ�� ���� ũ�� 
    public float ySize; //�� ������Ʈ�� ���� ũ�� 

    RaycastHit2D topMagHit;
    RaycastHit2D rightMagHit;
    RaycastHit2D bottomMagHit;
    RaycastHit2D leftMagHit;

    float rayDistance = 0.1f;

    public Sprite[] spriteGroup;
    SpriteRenderer spr;
    //[0]: ���Ⱑ �� �帧 [1]: ���Ⱑ �帧 
    private void Awake()
    {        
        rigid = GetComponent<Rigidbody2D>();
        spr = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
       if(GameManager.instance.gameData.curAchievementNum != activeAchNum)
        {
            rigid.bodyType = RigidbodyType2D.Kinematic; //��Ȱ��ȭ ���¿��� ����� 
        }
        else
        {
            rigid.bodyType = RigidbodyType2D.Dynamic;
        }

        spr.sprite = spriteGroup[0];
    }
   
    void Update()
    {
        if(GameManager.instance.gameData.curAchievementNum != activeAchNum)
        {
            //Ȱ��ȭ�Ǿ� ���� ������ ���� �ڵ� ����x 

            rigid.bodyType = RigidbodyType2D.Kinematic;
            return;
        }
        else
        {
            if(rigid.bodyType != RigidbodyType2D.Dynamic)
            {
                rigid.bodyType = RigidbodyType2D.Dynamic;
            }
        }

        if (isContact && isMagnetic) //�ڼ��� ��������鼭 ������ �帣�� ���¸� �״�� ���� 
        {
            transform.localPosition = initLocalPos; //��ġ ���� 
            rigid.gravityScale = 0f; //�߷� ���� ����
            rigid.velocity = Vector2.zero;
            if(spr.sprite != spriteGroup[1])
            {
                spr.sprite = spriteGroup[1];
            }
        }
        else
        {
            rigid.gravityScale = 3f;
            if (spr.sprite != spriteGroup[0])
            {
                spr.sprite = spriteGroup[0];
            }
        }

        contact_Check();

        if (isContact) //magnetic Field �� ������ �ִ� ���� ���Ⱑ �帣���� üũ 
        {
            isMagnetic_last = isMagnetic;
            isMagnetic = magneticField.GetComponent<electricSensor>().magWork;

            if(!isMagnetic_last && isMagnetic)
            {
                initLocalPos = transform.localPosition;
            }
        }
    }
   
    void contact_Check() //�� ������ �ݺ� 
    {        
        int layerMask = 1 << LayerMask.NameToLayer("electricPlatform");
        topMagHit = Physics2D.BoxCast(transform.position, new Vector2(xSize, ySize), 0f, new Vector2(0, 1), rayDistance, layerMask);
        rightMagHit = Physics2D.BoxCast(transform.position, new Vector2(xSize, ySize), 0f, new Vector2(1, 0), rayDistance, layerMask);
        bottomMagHit = Physics2D.BoxCast(transform.position, new Vector2(xSize, ySize), 0f, new Vector2(0, -1), rayDistance, layerMask);
        leftMagHit = Physics2D.BoxCast(transform.position, new Vector2(xSize, ySize), 0f, new Vector2(-1, 0), rayDistance, layerMask);

        //top ���⿡ �ڼ��� �ִ��� üũ
        if (topMagHit.collider != null)
        {
            isTopMag = true;
            magneticField = topMagHit.collider.gameObject;
        }
        else
        {
            isTopMag = false;
        }

        //rigth ���⿡ �ڼ��� �ִ��� üũ
        if (rightMagHit.collider != null)
        {
            isRightMag = true;
            magneticField = rightMagHit.collider.gameObject;
        }
        else
        {
            isRightMag = false;
        }

        //bottom ���⿡ �ڼ��� �ִ��� üũ 
        if (bottomMagHit.collider != null)
        {
            isBottomMag = true;
            magneticField = bottomMagHit.collider.gameObject;
        }
        else
        {
            isBottomMag = false;
        }

        //left ���⿡ �ڼ��� �ִ��� üũ 
        if (leftMagHit.collider != null)
        {
            isLeftMag = true;
            magneticField = leftMagHit.collider.gameObject;
        }
        else
        {
            isLeftMag = false;
        }

        if(isTopMag || isRightMag || isBottomMag || isLeftMag) //�� ���� �� �ϳ��� �����ϸ� �ڼ��� ������ �� 
        {
            isContact = true;
            transform.parent = magneticField.transform;
        }
        else
        {
            isContact = false;
            transform.parent = null;
        }
    }
}
