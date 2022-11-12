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
    //�÷��̾ ��/�Ʒ� ���⿡�� �������鼭 �����ϸ� 0 ~> flipX ���� �ٲ��� �ʰ� �״�� �θ� �� 
    //�÷��̾ ������ ���⿡�� ���� ��Ż�� �����ϸ� -1 ~> �� ���� ������ flipX ��������� �� 
    //�÷��̾ ���� ���⿡�� ������ ��Ż�� �����ϸ� +1 ~> �� ���� ������ flipX ��������� ��

    Player player;

    void Awake() {
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
    }

    void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.CompareTag("Player") && !GameManager.instance.shouldSpawnSavePoint) {

            if (useNextSpawnPosCode)
            {
                GameManager.instance.nextPos = nextSpawnPos;
            }
            else
            {
                GameManager.instance.nextPos = player.transform.position + deltaPos;
            }
          
            //������ �߷°� state�� �״�� ���� 
            GameManager.instance.nextGravityDir = Physics2D.gravity.normalized;
            GameManager.instance.nextState = Player.curState;

            //changeScene ������Ʈ�� �����ϴ� ���⿡ ���� �� ȸ������� �� 
            if(enterDirection < 0)
            {
                GameManager.instance.isStartWithFlipX = true;
            }
            else if(enterDirection > 0)
            {
                GameManager.instance.isStartWithFlipX = false;
            }

            UIManager.instance.FadeOut(.8f);
            Invoke("loadSceneInvoke", 1f);
        }
    }

    void loadSceneInvoke()
    {
        SceneManager.LoadScene(nextScene);
    }
}
