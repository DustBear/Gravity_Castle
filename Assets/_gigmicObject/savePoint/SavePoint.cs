using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class SavePoint : MonoBehaviour
{
    GameObject cameraObj;
    [SerializeField] int stageNum; 
    public int achievementNum; 

    Transform player;
    public Vector2 respawnPos; //리스폰 위치 
    public Vector2 respawnDir; //리스폰 방향

    bool isPlayerOnSensor;
    bool isSavePointActivated;

    SpriteRenderer spr;
    public Sprite[] spriteGroup;
    /* [0] : 비활성화 상태(불 꺼짐)
     * [1] : 비활성화 상태(불 켜짐)
     * [2]~[7] : stone이 아래로 내려가는 동작 
     * [7] : 활성화 상태
     */

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        spr = GetComponent<SpriteRenderer>();
        cameraObj = GameObject.FindWithTag("MainCamera");

        respawnPos = transform.position; //세이브포인트의 위치가 이 세이브를 이용할 때 리스폰돼야 하는 위치         
        float savePointRot = transform.rotation.z;
    }

    private void Start()
    {
        if (GameManager.instance.gameData.savePointUnlock[stageNum - 1, achievementNum - 1] == true) 
        {
            spr.sprite = spriteGroup[7]; //활성화 상태
            isSavePointActivated = true;
        }
        else
        {
            spr.sprite = spriteGroup[0]; //비활성화 상태 
            isSavePointActivated = false;
        }
    }

    private void Update()
    {
        if (isSavePointActivated) return; //활성화된 이후에는 따로 작동x 
        if(isPlayerOnSensor && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(SaveData());
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {       
        if (collision.CompareTag("Player") && transform.up == player.transform.up && !isSavePointActivated) 
        {
            //플레이어가 세이브포인트에 닿아 있고 각도가 같을 때 저장 가능 
            isPlayerOnSensor = true;
            spr.sprite = spriteGroup[1]; //불 켜짐 
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
            yield return new WaitForSeconds(0.03f);
        }
        cameraObj.GetComponent<MainCamera>().cameraShake(0.3f, 0.5f); //세이브스톤이 박힐 때 카메라 진동 
    }
}
