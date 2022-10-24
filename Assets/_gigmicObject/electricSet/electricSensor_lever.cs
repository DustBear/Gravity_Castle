using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class electricSensor_lever : MonoBehaviour
{
    public Sprite[] leverSprite;
    bool isLeverCorWork = false;

    public GameObject magPlatform; //ÀÚ¼® ÀÛ¿ëÇÏ´Â ÇÃ·§Æû 
    SpriteRenderer spr;
    SpriteRenderer sensorSpr;

    bool isPlayerOn;
    private void Awake()
    {
        spr = GetComponent<SpriteRenderer>();
        sensorSpr = magPlatform.GetComponent<SpriteRenderer>();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isPlayerOn && Input.GetKeyDown(KeyCode.E) && !isLeverCorWork)
        {
            StartCoroutine(leverAct());
            magPlatform.GetComponent<electricSensor>().magWork = !magPlatform.GetComponent<electricSensor>().magWork;
            //true ¸é false ·Î, false ÀÌ¸é true ·Î ¹Ù²ãÁÜ 

            if (magPlatform.GetComponent<electricSensor>().magWork)
            {
                sensorSpr.color = Color.red;
            }
            else
            {
                sensorSpr.color = Color.white;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player" && collision.transform.up == transform.up)
        {
            isPlayerOn = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player" && collision.transform.up == transform.up)
        {
            isPlayerOn = false;
        }
    }

    IEnumerator leverAct()
    {
        isLeverCorWork = true;
        spr.sprite = leverSprite[1];
        yield return new WaitForSeconds(0.5f);

        spr.sprite = leverSprite[0];
        isLeverCorWork = false;
    }
}
