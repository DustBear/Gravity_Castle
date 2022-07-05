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

    public float activeTime; //타이머 내장된 button이 작동하는 시간 
    bool isPressed;
    bool isColorChanging;

    void Awake()
    {
        render = GetComponent<SpriteRenderer>();
        switch (transform.eulerAngles.z) //버튼 돌아가 있는 각도에 따라 rotation angle 조정 
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
        if (isColorChanging) return; //색이 변하고 있는 중에는 작동 무시 

        RaycastHit2D rayHit = Physics2D.BoxCast(transform.position, new Vector2(0.7f, 0.1f), transform.eulerAngles.z, transform.up, 0.5f, 1 << 10);
        if (!isPressed && rayHit.collider != null && Physics2D.gravity.normalized == rotationAngle) //버튼이 눌리면 
        {
            render.color = new Color(1 - render.color.r, 1 - render.color.g, 0f, 1f);
            isGreen = !isGreen; //isGreen = false 

            //GameManager.instance.curIsGreen[buttonNum] = isGreen;
            if (!isGreen && isButtonType2) //buttonType2 는 타이머 내장된 버튼
            {
                StartCoroutine(ChangeIntoGreen()); //타이머 작동 
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

        while (render.color.r > 0f) //100프레임 걸쳐서 색 바뀜
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