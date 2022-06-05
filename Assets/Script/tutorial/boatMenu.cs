using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class boatMenu : MonoBehaviour
{
    public GameObject player;
    public Vector2 interiorPos; //�÷��̾ �� ������ �̵��� �� �����Ǵ� ��ġ
    public Vector2 outPos; //�÷��̾ �� �ۿ� ���� ���� ������ġ
    public GameObject fadeScreen; //���̵� ��,�ƿ��� ����ϴ� ��ũ�� 

    public float fadeSpeed; //���̵� ��,�ƿ� �ӵ�(fadeSpeed �� ��ŭ �ɸ�)
    public float fadeDelay; //���̵�ƿ� �ǰ� �ٽ� ������� �� �ɸ��� �ð� 
    void Start()
    {
        fadeScreen.SetActive(true);
        fadeScreen.GetComponent<Image>().color = new Color(0, 0, 0, 0);
    }

    void Update()
    {
        
    }

    public void moveInToRoom() //�÷��̾ �� ������ 
    {
        StartCoroutine(doorTransport(interiorPos));
    }

    public void moveOut() //�÷��̾ �� ������ 
    {
        StartCoroutine(doorTransport(outPos));
    }
   
    IEnumerator doorTransport (Vector2 transportPos)
    {
        //���̵� �ƿ� 
        Color fadeScreenColor = fadeScreen.GetComponent<Image>().color;

        while(fadeScreenColor.a <= 1)
        {
            fadeScreen.GetComponent<Image>().color = new Color(0, 0, 0, fadeScreen.GetComponent<Image>().color.a + 0.01f);
            fadeScreenColor = fadeScreen.GetComponent<Image>().color;
            yield return new WaitForSeconds(fadeSpeed / 100); //fadeSpeed �� �� ������ ��ο��� 
        }

        player.transform.position = transportPos; //�÷��̾� �̵� 

        //���̵� �� 
        fadeScreenColor = fadeScreen.GetComponent<Image>().color;

        while (fadeScreenColor.a >= 0)
        {
            fadeScreen.GetComponent<Image>().color = new Color(0, 0, 0, fadeScreen.GetComponent<Image>().color.a - 0.01f);
            fadeScreenColor = fadeScreen.GetComponent<Image>().color;
            yield return new WaitForSeconds(fadeSpeed / 100); //fadeSpeed �� �� ������ ����� 
        }
    }   
}
