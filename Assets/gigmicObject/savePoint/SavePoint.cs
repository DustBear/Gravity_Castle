using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class SavePoint : MonoBehaviour
{
    [SerializeField] int stageNum; //1부터 시작 
    public int achievementNum; //1부터 시작

    Transform player;
    public Vector2 respawnPos; //플레이어가 세이브포인트에 닿고 나서 리스폰되는 위치    
    public Vector2 respawnDir; //플레이어가 세이브포인트에 닿고 나서 리스폰되는 중력방향

    bool isPlayerOnSensor;
    bool isSavePointActivated;

    SpriteRenderer spr;
    public Sprite[] spriteGroup;
    /* [0] : 활성화되지 않았고 플레이어가 센서위에 없음  
     * [1] : 플레이어가 센서 위에 위치 
     * [2]~[7] : 세이브포인트 스톤이 아래로 내려가는 동작
     * [7] : 세이브포인트가 이미 활성화 
     */

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        spr = GetComponent<SpriteRenderer>();

        respawnPos = transform.position; //플레이어는 해당 세이브포인트의 위치에서 부활 
        
        float savePointRot = transform.rotation.z;

        if(GameManager.instance.gameData.savePointUnlock[stageNum-1 , achievementNum-1] == true) //세이브포인트 배열의 stage/achNum 은 둘 다 0,0에서 시작 
        {
            spr.sprite = spriteGroup[7]; //세이브포인트가 이미 작동한 모습 
            isSavePointActivated = true;
        }
        else
        {
            spr.sprite = spriteGroup[0]; //세이브포인트가 작동하지 않은 모습 
            isSavePointActivated = false;
        }
    }

    private void Update()
    {
        if (isSavePointActivated) return; //이미 활성화된 세이브포인트면 굳이 input 감지 x 
        if(isPlayerOnSensor && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(SaveData());
        }
    }

    void OnTriggerEnter2D(Collider2D collision) //세이브포인트를 직접 건드려서 세이브함
    {       
        if (collision.CompareTag("Player") && transform.up == player.transform.up && !isSavePointActivated) 
        {
            //플레이어가 세이브포인트와 같은 angle을 가지고 있고 아직 활성화시키지 않은 세이브포인트일 때 
            isPlayerOnSensor = true;
            spr.sprite = spriteGroup[1];
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isSavePointActivated)
        {
            isPlayerOnSensor = false;
            spr.sprite = spriteGroup[0];
        }
    }

    IEnumerator SaveData()
    {
        isSavePointActivated = true;
        Debug.Log("savePointBackUp: " + achievementNum);
        GameManager.instance.SaveData(achievementNum, stageNum, respawnPos);

        for(int index=1; index<=7; index++)
        {
            spr.sprite = spriteGroup[index];
            yield return new WaitForSeconds(0.1f);
        }
    }
}
