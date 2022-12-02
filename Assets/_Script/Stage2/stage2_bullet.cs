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
        originScene = SceneManager.GetActiveScene(); //포탄이 생성될 때 마다 씬 번호 가져와서 할당 
    }


    void Update()
    {
        // 씬 번호가 바뀌면 오브젝트 파괴해 줘야 함 
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
