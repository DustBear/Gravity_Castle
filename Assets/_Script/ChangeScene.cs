using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    [SerializeField] int nextScene;
    [SerializeField] Vector2 nextSpawnPos; //���� ������ �����ž� �� ��ǥ 
    [SerializeField] bool useNextSpawnPosCode;
    //Ʃ�丮�� ���������� �� ���� �̽��� �� �� ������ ��ΰ� ��ǥ�󿡼� ������ ����.
    //Ʃ������ ���������� ��쿡�� bool ���� üũ�ϰ�, deltaPos�� �ƴ϶� ��ǥ�� ���� �̵��ϵ��� ����.

    [SerializeField] int enterDirection;
    [SerializeField] Vector3 deltaPos;
    //�÷��̾ ������ ���⿡�� ���� ��Ż�� �����ϸ� -1 ~> �� ���� ������ flipX ��������� �� 
    //�÷��̾ ���� ���⿡�� ������ ��Ż�� �����ϸ� +1 ~> �� ���� ������ flipX ��������� ��

    Player player;

    void Awake() {
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
    }

    void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.CompareTag("Player") && !GameManager.instance.shouldStartAtSavePoint) {

            if (useNextSpawnPosCode)
            {
                GameManager.instance.nextPos = nextSpawnPos;
            }
            else
            {
                GameManager.instance.nextPos = player.transform.position + deltaPos;
            }
          
            GameManager.instance.nextGravityDir = Physics2D.gravity.normalized;
            GameManager.instance.nextState = Player.curState;

            //changeScene ������Ʈ�� �����ϴ� ���⿡ ���� �� ȸ������� �� 
            if(enterDirection < 0)
            {
                GameManager.instance.isStartWithFlipX = true;
            }
            else
            {
                GameManager.instance.isStartWithFlipX = false;
            }

            SceneManager.LoadScene(nextScene);
        }
    }
}