using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveBox_lever : MonoBehaviour
{
    public GameObject moveStone;
    Rigidbody2D rigid;
    SpriteRenderer spr;
    public Sprite[] spriteGroup; //[0]=idle  [1]=left  [2]=right

    public float moveSpeed;

    [SerializeField] bool isPlayerOn;
    [SerializeField] bool isLeverAct;

    public bool isMoveHorizontal; //true 이면 box 가 가로로 움직임 

    //pos1의 수치가 pos2 보다 커야 함(x든 y든 둘 다 해당)
    public Vector2 pos1;
    public Vector2 pos2;

    private void Awake()
    {
        isPlayerOn = false;
        isLeverAct = false;

        if(pos1.x == pos2.x) //x좌표가 같으면 세로로 움직이는 것 
        {
            isMoveHorizontal = false;
        }
        else
        {
            isMoveHorizontal = true;
        }
    }
    void Start()
    {
        rigid = moveStone.GetComponent<Rigidbody2D>();
        spr = GetComponent<SpriteRenderer>();

        spr.sprite = spriteGroup[0];
    }

    
    void Update()
    {
        if (!isPlayerOn) return;

        if (isPlayerOn && Input.GetKeyDown(KeyCode.E))
        {
            if (!isLeverAct)
            {
                isLeverAct = true;
                InputManager.instance.isInputBlocked = true;
            }
            else
            {
                isLeverAct = false;
                InputManager.instance.isInputBlocked = false;
            }
        }

        if(isLeverAct && Input.GetKey(KeyCode.LeftArrow)) // [<--] 누르면
        {
            
            if ((isMoveHorizontal && moveStone.transform.position.x < pos2.x) || (!isMoveHorizontal && moveStone.transform.position.y < pos2.y)) 
            {
                rigid.velocity = Vector3.zero;
                moveStone.transform.position = pos2;
                return;
            }
            
            if(isMoveHorizontal) rigid.velocity = new Vector2(-1, 0) * moveSpeed;
            else rigid.velocity = new Vector2(0, -1) * moveSpeed;

            spr.sprite = spriteGroup[1];
        }
        else if(isLeverAct && Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.LeftArrow)) // <-- 를 누르지 않은 상태에서 --> 를 누르면 
        {
            if ((moveStone.transform.position.x > pos1.x) || (!isMoveHorizontal && moveStone.transform.position.y > pos1.y))
            {
                rigid.velocity = Vector3.zero;
                moveStone.transform.position = pos1;
                return;
            }

            if (isMoveHorizontal) rigid.velocity = new Vector2(1, 0) * moveSpeed;
            else rigid.velocity = new Vector2(0, 1) * moveSpeed;

            spr.sprite = spriteGroup[2];
        }
        else if(isLeverAct && Input.GetKeyUp(KeyCode.LeftArrow) && !Input.GetKeyDown(KeyCode.RightArrow)) 
        {
            rigid.velocity = Vector3.zero;//왼쪽 화살표에서 손을 뗐고 오른쪽 화살표 또한 누르지 않고 있을 때 
            spr.sprite = spriteGroup[0];
        }
        else if (isLeverAct && !Input.GetKeyDown(KeyCode.LeftArrow) && Input.GetKeyUp(KeyCode.RightArrow))
        {
            rigid.velocity = Vector3.zero;//오른쪽 화살표에서 손을 뗐고 왼쪽 화살표 또한 누르지 않고 있을 때 
            spr.sprite = spriteGroup[0];
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player") isPlayerOn = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player") isPlayerOn = false;
        rigid.velocity = Vector3.zero; //플레이어가 레버 센서 밖으로 나가면 stone 도 정지 
        spr.sprite = spriteGroup[0];
    }
}
