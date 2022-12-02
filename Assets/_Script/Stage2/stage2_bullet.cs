using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class stage2_bullet : MonoBehaviour
{
    ObjManager.ObjType type = ObjManager.ObjType.cannon;
    Scene originScene;

    void Start()
    {
        
    }

    private void OnEnable()
    {
        originScene = SceneManager.GetActiveScene(); //��ź�� ������ �� ���� �� ��ȣ �����ͼ� �Ҵ� 
    }


    void Update()
    {
        // �� ��ȣ�� �ٲ�� ������Ʈ �ı��� ��� �� 
        if (originScene != SceneManager.GetActiveScene() || GameManager.instance.gameData.SpawnSavePoint_bool)
        {
            ObjManager.instance.ReturnObj(type, gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Platform")
        {
            ObjManager.instance.ReturnObj(type, gameObject);
        }
    }
}
