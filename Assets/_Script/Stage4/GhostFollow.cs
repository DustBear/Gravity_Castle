using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostFollow : MonoBehaviour
{
    [SerializeField] float speed;
    Player player;
    SpriteRenderer render;
    Vector2 targetPos;

    public int activeNum; //유령이 작동 시작하는 시점 
    public int level_IncreaseNum; //유령이 강화되는 시점

    [SerializeField] Vector2 targetPos_default; //7번 이하 세이브포인트에서 시작했을 때의 기본위치: followGhost는 savePoint7에 도달하기 전까지는 움직이지 않음
    [SerializeField] Vector2 targetPos_save8; //8번 세이브포인트에서 시작했을 때 유령의 위치
    [SerializeField] Vector2 targetPos_save9;
    [SerializeField] Vector2 targetPos_save10;
    [SerializeField] Vector2 targetPos_save11;
    [SerializeField] Vector2 targetPos_save12;
    [SerializeField] Vector2 targetPos_save13;
    void Awake()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        render = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        //어떤 세이브포인트에서 시작하냐에 따라 followGhost의 위치가 달라져야 함
        switch (GameManager.instance.gameData.curAchievementNum)
        {
            case 8:
                targetPos = targetPos_save8;
                break;
            case 9:
                targetPos = targetPos_save9;
                break;
            case 10:
                targetPos = targetPos_save10;
                break;
            case 11:
                targetPos = targetPos_save11;
                break;
            case 12:
                targetPos = targetPos_save12;
                break;
            case 13:
                targetPos = targetPos_save13;
                break;

            default: //기본값
                targetPos = targetPos_default;
                break;
        }

        transform.position = targetPos;
        StartCoroutine(IncreaseSpeed());
    }

    void Update()
    {
        if (GameManager.instance.gameData.curAchievementNum >= activeNum)
        {            
            if (Player.curState == Player.States.ChangeGravityDir)
            {
                //플레이어가 레버를 돌리면 돌린 위치가 targetPos로 바뀐다 
                targetPos = player.transform.position;
            }
            else
            {
                //레버를 돌리지 않고 있을 때는 targetPos를 향해 이동한다 
                transform.position = Vector2.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
            }

            // 유령이 레버에 도착하면 레버를 돌림
            if (Player.curState != Player.States.GhostUsingLever  //유령이 레버를 사용하는 중이 아니고 
                && Player.curState != Player.States.FallAfterGhostLevering //유령이 레버를 돌린 후 플레이어가 떨어지는 중이 아니고
                && GameManager.instance.gameData.curAchievementNum >= level_IncreaseNum //유령이 각성한 상태고 
                && (Vector2)transform.position == targetPos //유령이 targetPos에 도달했고
                && Physics2D.gravity.normalized != Vector2.down) //현재 중력방향이 아래쪽 방향이 아니면(?)
            {
                player.ChangeState(Player.States.GhostUsingLever); //유령이 레버를 작동시킨다
                Debug.Log("achievementNum: " + GameManager.instance.gameData.curAchievementNum);
            }
            
            transform.rotation = player.transform.rotation;
        }
    }

    IEnumerator IncreaseSpeed()
    {
        while (GameManager.instance.gameData.curAchievementNum < level_IncreaseNum)
        {
            yield return new WaitForSeconds(5f);
        }
        speed = 5f;
        Color color = render.color;
        color.g = 0.3f;
        color.b = 0.3f;
        color.a = 1;
        render.color = color;
    }
}
