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

    [SerializeField] Vector2 targetPos_default; //7�� ���� ���̺�����Ʈ���� �������� ���� �⺻��ġ: followGhost�� savePoint7�� �����ϱ� �������� �������� ����
    [SerializeField] Vector2 targetPos_save8; //8�� ���̺�����Ʈ���� �������� �� ������ ��ġ
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
        //� ���̺�����Ʈ���� �����ϳĿ� ���� followGhost�� ��ġ�� �޶����� ��
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

            default: //�⺻��
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
                //�÷��̾ ������ ������ ���� ��ġ�� targetPos�� �ٲ�� 
                targetPos = player.transform.position;
            }
            else
            {
                //������ ������ �ʰ� ���� ���� targetPos�� ���� �̵��Ѵ� 
                transform.position = Vector2.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
            }

            // ������ ������ �����ϸ� ������ ����
            if (Player.curState != Player.States.GhostUsingLever  //������ ������ ����ϴ� ���� �ƴϰ� 
                && Player.curState != Player.States.FallAfterGhostLevering //������ ������ ���� �� �÷��̾ �������� ���� �ƴϰ�
                && GameManager.instance.gameData.curAchievementNum >= level_IncreaseNum //������ ������ ���°� 
                && (Vector2)transform.position == targetPos //������ targetPos�� �����߰�
                && Physics2D.gravity.normalized != Vector2.down) //���� �߷¹����� �Ʒ��� ������ �ƴϸ�(?)
            {
                player.ChangeState(Player.States.GhostUsingLever); //������ ������ �۵���Ų��
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
