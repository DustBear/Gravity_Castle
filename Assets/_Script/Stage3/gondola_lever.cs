using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gondola_lever : MonoBehaviour
{
    public GameObject gondolaObj;
    public GameObject gondolaPlatform;
    public gondola gondolaScr;

    Rigidbody2D platformRigid;

    public Sprite[] leverSprites;
    public Sprite[] platformSprites;

    SpriteRenderer leverSpr;
    SpriteRenderer platformSpr;

    bool isPlayerOn;
    bool isGondolaActed;

    public float movePeriod; //�ﵹ�� ���� ���� ������ �� �ɸ��� �ð� 
    public Vector2 pos1;
    public Vector2 pos2;
    private void Awake()
    {
        gondolaObj = transform.parent.gameObject;
        gondolaScr = gondolaObj.GetComponent<gondola>();
        platformRigid = gondolaPlatform.GetComponent<Rigidbody2D>();

        leverSpr = GetComponent<SpriteRenderer>();
        platformSpr = gondolaPlatform.GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {       
        if(isPlayerOn && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(leverAct());
            if (!isGondolaActed)
            {
                StartCoroutine(gondolaMove());
                isGondolaActed = true;
            }           
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            isPlayerOn = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        isPlayerOn = false;
    }

    IEnumerator leverAct()
    {
        leverSpr.sprite = leverSprites[1];
        yield return new WaitForSeconds(1f);

        leverSpr.sprite = leverSprites[0];
    }

    IEnumerator gondolaMove()
    {
        InputManager.instance.isInputBlocked = true; //�ﵹ�� �����̸� �������̰� ���� 
        GameObject.Find("Main Camera").GetComponent<MainCamera>().cameraShake(0.2f, 1f); //ī�޶� ��鸲 

        yield return new WaitForSeconds(2f); //2�ʰ� ��ٸ��� ��� 

        float moveVel = (pos2 - pos1).magnitude / movePeriod;
        platformRigid.velocity = new Vector2(-moveVel, 0);
        //StartCoroutine(gondolaShake(1)); //����ϴ� ���� �ﵹ�� ��鸲 
        StartCoroutine(platformAni());

        yield return new WaitForSeconds(movePeriod); //��ǥ���� ���� 
        gondolaPlatform.transform.position = pos2;
        platformRigid.velocity = Vector2.zero; //�ӵ� ���� 
        //StartCoroutine(gondolaShake(-1)); //�����ϴ� ���� �ﵹ�� ��鸲 

        GameObject.Find("Main Camera").GetComponent<MainCamera>().cameraShake(0.3f, 0.3f); //ī�޶� ��鸲 
        InputManager.instance.isInputBlocked = false;

    }

    IEnumerator gondolaShake(int dir) //1�̸� ���������� �� ���� ��鸲, -1�̸� �������� �� ���� ��鸲 
    {
        GameObject.Find("Player").GetComponent<Rigidbody2D>().freezeRotation = false;

        for(int index=0; index<10; index++)
        {
            gondolaObj.transform.Rotate(new Vector3(0, 0, dir * 1f));
            yield return new WaitForSeconds(0.05f);
        }
        for (int index = 0; index < 15; index++)
        {
            gondolaObj.transform.Rotate(new Vector3(0, 0, -dir * 1f));
            yield return new WaitForSeconds(0.05f);
        }
        for (int index = 0; index < 5; index++)
        {
            gondolaObj.transform.Rotate(new Vector3(0, 0, dir * 1f));
            yield return new WaitForSeconds(0.05f);
        }

        GameObject.Find("Player").GetComponent<Rigidbody2D>().freezeRotation = true;
        GameObject.Find("Player").transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    IEnumerator platformAni()
    {
        while(platformRigid.velocity.magnitude > 0.1f)
        {
            for(int index=0; index<platformSprites.Length; index++)
            {
                platformSpr.sprite = platformSprites[index];
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}
