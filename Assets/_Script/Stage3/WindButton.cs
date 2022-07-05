using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindButton : MonoBehaviour
{
    [SerializeField] int buttonNum;
    [SerializeField] WindHome windHome;
    [SerializeField] GameObject windZone;
    [SerializeField] GameObject wind;
    [SerializeField] bool isChanged;

    SpriteRenderer render;
    Vector2 rotationAngle;
    [SerializeField] bool isGreen;
    [SerializeField] bool isButtonType2;

    public float activeTime; //Ÿ�̸� ����� button�� �۵��ϴ� �ð� 
    bool isPressed;
    bool isColorChanging;

    void Awake()
    {
        render = GetComponent<SpriteRenderer>();
        switch (transform.eulerAngles.z) //��ư ���ư� �ִ� ������ ���� rotation angle ���� 
        {
            case 0f:
                rotationAngle = new Vector2(0, -1);
                break;
            case 90f:
                rotationAngle = new Vector2(1, 0);
                break;
            case 180f:
                rotationAngle = new Vector2(0, 1);
                break;
            default:
                rotationAngle = new Vector2(-1, 0);
                break;
        }
    }

    void Start()
    {
        // if (GameManager.instance.curIsGreen[buttonNum])
        // {
        //     render.color = new Color(0f, 1f, 0f, 1f);
        //     isGreen = true;
        //     windHome.enabled = true;
        //     windZone.SetActive(true);
        //     wind.SetActive(true);
        // }
    }

    void Update()
    {
        if (isColorChanging) return; //���� ���ϰ� �ִ� �߿��� �۵� ���� 

        RaycastHit2D rayHit = Physics2D.BoxCast(transform.position, new Vector2(0.7f, 0.1f), transform.eulerAngles.z, transform.up, 0.5f, 1 << 10);
        if (!isPressed && rayHit.collider != null && Physics2D.gravity.normalized == rotationAngle) //��ư�� ������ 
        {
            render.color = new Color(1 - render.color.r, 1 - render.color.g, 0f, 1f);
            isGreen = !isGreen; //isGreen = false 

            //GameManager.instance.curIsGreen[buttonNum] = isGreen;
            if (!isGreen && isButtonType2) //buttonType2 �� Ÿ�̸� ����� ��ư
            {
                StartCoroutine(ChangeIntoGreen()); //Ÿ�̸� �۵� 
            }
            else
            {
                windHome.enabled = isGreen;
                windZone.SetActive(isGreen);
                wind.SetActive(isGreen);
            }
            isPressed = true;
        }
        if (rayHit.collider == null)
        {
            isPressed = false;
        }
    }

    IEnumerator ChangeIntoGreen()
    {
        var wait = new WaitForSeconds(activeTime/100);

        isGreen = false;
        isColorChanging = true;
        windHome.enabled = false;
        windZone.SetActive(false);
        wind.SetActive(false);

        while (render.color.r > 0f) //100������ ���ļ� �� �ٲ�
        {
            Color color = render.color;
            color.r -= 0.01f;
            color.g += 0.01f;
            render.color = color;
            yield return wait;
        }
        isGreen = true;
        isColorChanging = false;
        windHome.enabled = true;
        windZone.SetActive(true);
        wind.SetActive(true);
    }
}