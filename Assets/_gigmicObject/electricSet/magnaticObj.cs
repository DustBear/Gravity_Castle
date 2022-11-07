using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class magnaticObj : MonoBehaviour
{
    Rigidbody2D rigid;

    [SerializeField] bool isContact; //현재 매그네틱필드에 닿아있는지의 여부 
    [SerializeField] bool isMagnetic; //현재 매그네틱필드에 자기장이 걸려 있는지의 여부
    bool isMagnetic_last;

    GameObject magneticField;
    Vector3 initLocalPos;

    public int activeAchNum; //현재 퍼즐을 풀고 있는 동안만 작동 

    [SerializeField] bool isTopMag;
    [SerializeField] bool isRightMag;
    [SerializeField] bool isBottomMag;
    [SerializeField] bool isLeftMag;

    public float xSize; //이 오브젝트의 가로 크기 
    public float ySize; //이 오브젝트의 세로 크기 

    RaycastHit2D topMagHit;
    RaycastHit2D rightMagHit;
    RaycastHit2D bottomMagHit;
    RaycastHit2D leftMagHit;

    float rayDistance = 0.1f;

    public Sprite[] spriteGroup;
    SpriteRenderer spr;
    //[0]: 전기가 안 흐름 [1]: 전기가 흐름 
    private void Awake()
    {        
        rigid = GetComponent<Rigidbody2D>();
        spr = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
       if(GameManager.instance.gameData.curAchievementNum != activeAchNum)
        {
            rigid.bodyType = RigidbodyType2D.Kinematic; //비활성화 상태에선 멈춘다 
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
            //활성화되어 있지 않으면 이후 코드 실행x 

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

        if (isContact && isMagnetic) //자석에 닿아있으면서 전류가 흐르는 상태면 그대로 고정 
        {
            transform.localPosition = initLocalPos; //위치 고정 
            rigid.gravityScale = 0f; //중력 영향 없앰
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

        if (isContact) //magnetic Field 에 접촉해 있는 동안 전기가 흐르는지 체크 
        {
            isMagnetic_last = isMagnetic;
            isMagnetic = magneticField.GetComponent<electricSensor>().magWork;

            if(!isMagnetic_last && isMagnetic)
            {
                initLocalPos = transform.localPosition;
            }
        }
    }
   
    void contact_Check() //매 프레임 반복 
    {        
        int layerMask = 1 << LayerMask.NameToLayer("electricPlatform");
        topMagHit = Physics2D.BoxCast(transform.position, new Vector2(xSize, ySize), 0f, new Vector2(0, 1), rayDistance, layerMask);
        rightMagHit = Physics2D.BoxCast(transform.position, new Vector2(xSize, ySize), 0f, new Vector2(1, 0), rayDistance, layerMask);
        bottomMagHit = Physics2D.BoxCast(transform.position, new Vector2(xSize, ySize), 0f, new Vector2(0, -1), rayDistance, layerMask);
        leftMagHit = Physics2D.BoxCast(transform.position, new Vector2(xSize, ySize), 0f, new Vector2(-1, 0), rayDistance, layerMask);

        //top 방향에 자석이 있는지 체크
        if (topMagHit.collider != null)
        {
            isTopMag = true;
            magneticField = topMagHit.collider.gameObject;
        }
        else
        {
            isTopMag = false;
        }

        //rigth 방향에 자석이 있는지 체크
        if (rightMagHit.collider != null)
        {
            isRightMag = true;
            magneticField = rightMagHit.collider.gameObject;
        }
        else
        {
            isRightMag = false;
        }

        //bottom 방향에 자석이 있는지 체크 
        if (bottomMagHit.collider != null)
        {
            isBottomMag = true;
            magneticField = bottomMagHit.collider.gameObject;
        }
        else
        {
            isBottomMag = false;
        }

        //left 방향에 자석이 있는지 체크 
        if (leftMagHit.collider != null)
        {
            isLeftMag = true;
            magneticField = leftMagHit.collider.gameObject;
        }
        else
        {
            isLeftMag = false;
        }

        if(isTopMag || isRightMag || isBottomMag || isLeftMag) //네 군데 중 하나라도 반응하면 자석과 접촉한 것 
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
