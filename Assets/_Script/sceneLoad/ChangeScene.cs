using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    [SerializeField] int nextScene;
    [SerializeField] Vector2 nextSpawnPos; //다음 씬에서 스폰돼야 할 좌표 
    [SerializeField] bool useNextSpawnPosCode;
    //튜토리얼 스테이지는 맵 설계 미스로 두 씬 사이의 통로가 좌표상에서 만나지 않음.
    //튜툐리얼 스테이지의 경우에만 bool 값을 체크하고, deltaPos가 아니라 좌표를 통해 이동하도록 했음.

    [SerializeField] int enterDirection;
    [SerializeField] Vector3 deltaPos;
    //플레이어가 위/아래 방향에서 떨어지면서 진입하면 0 ~> flipX 여부 바꾸지 않고 그대로 두면 됨 
    //플레이어가 오른쪽 방향에서 왼쪽 포탈로 진입하면 -1 ~> 그 다음 씬에서 flipX 선택해줘야 함 
    //플레이어가 왼쪽 방향에서 오른쪽 포탈로 진입하면 +1 ~> 그 다음 씬에서 flipX 해제해줘야 함

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
          
            //현재의 중력과 state를 그대로 유지 
            GameManager.instance.nextGravityDir = Physics2D.gravity.normalized;
            GameManager.instance.nextState = Player.curState;

            //changeScene 오브젝트에 진입하는 방향에 따라 몸 회전해줘야 함 
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
