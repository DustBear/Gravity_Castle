using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostFollow : MonoBehaviour
{
    [SerializeField] float speed;
    Player player;
    SpriteRenderer render;
    Vector2 targetPos;

    public int activeNum; //������ �۵� �����ϴ� ���� 
    public int level_IncreaseNum; //������ ��ȭ�Ǵ� ����

    void Awake()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        render = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        if (GameManager.instance.gameData.curAchievementNum == level_IncreaseNum)
        {
            transform.position = new Vector2(-162.4f, 10.4f);
        }
        targetPos = transform.position;
        StartCoroutine(IncreaseSpeed());
    }

    void Update()
    {
        if (GameManager.instance.gameData.curAchievementNum >= activeNum)
        {
            // Timing when player start to fall after using lever
            if (Player.curState == Player.States.ChangeGravityDir)
            {
                targetPos = player.transform.position;
            }
            else
            {
                // Follow
                transform.position = Vector2.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
            }

            // Finish follow
            if (Player.curState != Player.States.GhostUsingLever && Player.curState != Player.States.FallAfterGhostLevering && GameManager.instance.gameData.curAchievementNum >= activeNum && (Vector2)transform.position == targetPos && Physics2D.gravity.normalized != Vector2.down)
            {
                player.ChangeState(Player.States.GhostUsingLever);
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
