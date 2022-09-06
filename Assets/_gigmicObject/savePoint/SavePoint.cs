using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class SavePoint : MonoBehaviour
{
    GameObject cameraObj;
    [SerializeField] int stageNum; //1���� ���� 
    public int achievementNum; //1���� ����

    Transform player;
    public Vector2 respawnPos; //�÷��̾ ���̺�����Ʈ�� ��� ���� �������Ǵ� ��ġ    
    public Vector2 respawnDir; //�÷��̾ ���̺�����Ʈ�� ��� ���� �������Ǵ� �߷¹���

    bool isPlayerOnSensor;
    bool isSavePointActivated;

    SpriteRenderer spr;
    public Sprite[] spriteGroup;
    /* [0] : Ȱ��ȭ���� �ʾҰ� �÷��̾ �������� ����  
     * [1] : �÷��̾ ���� ���� ��ġ 
     * [2]~[7] : ���̺�����Ʈ ������ �Ʒ��� �������� ����
     * [7] : ���̺�����Ʈ�� �̹� Ȱ��ȭ 
     */

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        spr = GetComponent<SpriteRenderer>();
        cameraObj = GameObject.FindWithTag("MainCamera");

        respawnPos = transform.position; //�÷��̾�� �ش� ���̺�����Ʈ�� ��ġ���� ��Ȱ 
        
        float savePointRot = transform.rotation.z;

        if (GameManager.instance.gameData.savePointUnlock[stageNum - 1, achievementNum - 1] == true) //���̺�����Ʈ �迭�� stage/achNum �� �� �� 0,0���� ���� 
        {           
            spr.sprite = spriteGroup[7]; //���̺�����Ʈ�� �̹� �۵��� ��� 
            isSavePointActivated = true;
        }
        else
        {
            spr.sprite = spriteGroup[0]; //���̺�����Ʈ�� �۵����� ���� ��� 
            isSavePointActivated = false;
        }
    }

    private void Update()
    {
        if (isSavePointActivated) return; //�̹� Ȱ��ȭ�� ���̺�����Ʈ�� ���� input ���� x 
        if(isPlayerOnSensor && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(SaveData());
        }
    }

    void OnTriggerEnter2D(Collider2D collision) //���̺�����Ʈ�� ���� �ǵ���� ���̺���
    {       
        if (collision.CompareTag("Player") && transform.up == player.transform.up && !isSavePointActivated) 
        {
            //�÷��̾ ���̺�����Ʈ�� ���� angle�� ������ �ְ� ���� Ȱ��ȭ��Ű�� ���� ���̺�����Ʈ�� �� 
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

        //Debug.Log("savePointBackUp: " + achievementNum);
        GameManager.instance.SaveData(achievementNum, stageNum, respawnPos);

        for(int index=1; index<=7; index++)
        {
            spr.sprite = spriteGroup[index];
            yield return new WaitForSeconds(0.03f);
        }
        cameraObj.GetComponent<MainCamera>().cameraShake(0.3f, 0.5f); //세이브스톤이 박힐 때 카메라 진동 
    }
}
