using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class slowFallingPlatform : MonoBehaviour
{
    public float fallSpeed; //일정한 속도로 등속 낙하함 
    public GameObject player; //플레이어의 회전각을 기준으로 낙하방향을 정함 
    public int activeNum;

    public Vector2 pos1;
    public Vector2 pos2; //이동 가능범위 
    /*
    ----pos2 
    --------
    pos1----     무조건 pos2는 x,y 중 하나는 pos1 보다 커야 함 
    */
    
    bool isMoveHorizontal; //플랫폼이 가로로 움직이면 true, 세로로 움직이면 false
    int moveDirection;
    /*
    _ 1 _
    4 _ 2 //플레이어 머리가 향하는 방향 
    _ 3 _   
    */

    Rigidbody2D rigid;
    public AudioSource sound;
    public AudioSource loopSound;

    public AudioClip moveSound;
    public AudioClip startSound;

    void Start()
    {
        if(pos1.x == pos2.x) //두 지점의 x좌표가 같다 ~> y축 방향으로만 움직이는 플랫폼이다 
        {
            isMoveHorizontal = false;
        }
        else
        {
            isMoveHorizontal = true;
        }

        rigid = GetComponent<Rigidbody2D>();
        loopSound.clip = moveSound;
    }
    
    void Update()
    {
        directionCheck();
        speedCheck();
        soundCheck();
    }

    [SerializeField] bool isMoving;
    [SerializeField] bool wasMoving; //이전 프레임에 움직이고 있었는지의 여부 

    void soundCheck()
    {
        if(GameManager.instance.gameData.curAchievementNum != activeNum)
        {
            return;
        }

        if(rigid.velocity.magnitude > 0.1f)
        {
            isMoving = true;
            if (!loopSound.isPlaying)
            {                
                loopSound.Play();
            }
        }
        else
        {
            isMoving = false;
            loopSound.Stop();
        }

        if(!wasMoving && isMoving)
        {
            //이전 프레임에선 정지해 있다가 그 다음 프레임에서는 출발 
            sound.PlayOneShot(startSound);
        }

        wasMoving = isMoving;
    }

    void directionCheck()
    {
        if (player.transform.up == new Vector3(0, 1, 0)) //플레이어 머리가 위쪽을 향함 
        {
            moveDirection = 1;
        }
        else if(player.transform.up == new Vector3(1, 0, 0)) //플레이어 머리가 오른쪽을 향함 
        {
            moveDirection = 2;
        }
        else if (player.transform.up == new Vector3(0, -1, 0)) //플레이어 머리가 아래쪽을 향함 
        {
            moveDirection = 3;
        }
        else //플레이어 머리가 왼쪽을 향함 
        {
            moveDirection = 4;
        }
    }

    void speedCheck()
    {
        if (isMoveHorizontal) //가로로 움직이는 플랫폼일 때: moveDir이 1,3일 땐 가만히 있고 2일땐 -x 방향으로, 4일 땐 +x 방향으로 이동해야 함 
        {
            if(moveDirection==1 || moveDirection == 3)
            {
                rigid.velocity = Vector3.zero;
                isMoving = false;
                loopSound.Stop();
            }
            else if (moveDirection == 2)
            {
                if(transform.position.x <= pos1.x)
                {
                    transform.position = pos1;
                    rigid.velocity = Vector3.zero;

                    isMoving = false;
                    loopSound.Stop();
                }
                else
                {
                    rigid.velocity = new Vector3(-fallSpeed, 0, 0);
                }               
            }
            else
            {
                if (transform.position.x >= pos2.x)
                {
                    transform.position = pos2;
                    rigid.velocity = Vector3.zero;

                    isMoving = false;
                    loopSound.Stop();
                }
                else
                {
                    rigid.velocity = new Vector3(fallSpeed, 0, 0);
                }               
            }
        }
        else //세로로 움직이는 플랫폼일 때: moveDir이 2,4일 땐 가만히 있고 1일땐 -y 방향으로, 3일 땐 +y 방향으로 이동해야 함 
        {
            if (moveDirection == 2 || moveDirection == 4)
            {
                rigid.velocity = Vector3.zero;
                isMoving = false;
                loopSound.Stop();
            }
            else if (moveDirection == 1)
            {
                if (transform.position.y <= pos1.y)
                {
                    transform.position = pos1;
                    rigid.velocity = Vector3.zero;

                    isMoving = false;
                    loopSound.Stop();
                }
                else
                {
                    rigid.velocity = new Vector3(0, -fallSpeed, 0);
                }               
            }
            else
            {
                if (transform.position.y >= pos2.y)
                {
                    transform.position = pos2;
                    rigid.velocity = Vector3.zero;

                    isMoving = false;
                    loopSound.Stop();
                }
                else
                {
                    rigid.velocity = new Vector3(0, fallSpeed, 0);
                }            
            }
        }
    }  
}
