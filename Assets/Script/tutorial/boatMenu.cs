using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class boatMenu : MonoBehaviour
{
    public GameObject player;
    public Vector2 interiorPos; //플레이어가 방 안으로 이동할 때 스폰되는 위치
    public Vector2 outPos; //플레이어가 방 밖에 있을 때의 스폰위치
    public GameObject fadeScreen; //페이드 인,아웃을 담당하는 스크린 

    public float fadeSpeed; //페이드 인,아웃 속도(fadeSpeed 초 만큼 걸림)
    public float fadeDelay; //페이드아웃 되고 다시 밝아지는 데 걸리는 시간 
    void Start()
    {
        fadeScreen.SetActive(true);
        fadeScreen.GetComponent<Image>().color = new Color(0, 0, 0, 0);
    }

    void Update()
    {
        
    }

    public void moveInToRoom() //플레이어를 방 안으로 
    {
        StartCoroutine(doorTransport(interiorPos));
    }

    public void moveOut() //플레이어를 배 밖으로 
    {
        StartCoroutine(doorTransport(outPos));
    }
   
    IEnumerator doorTransport (Vector2 transportPos)
    {
        //페이드 아웃 
        Color fadeScreenColor = fadeScreen.GetComponent<Image>().color;

        while(fadeScreenColor.a <= 1)
        {
            fadeScreen.GetComponent<Image>().color = new Color(0, 0, 0, fadeScreen.GetComponent<Image>().color.a + 0.01f);
            fadeScreenColor = fadeScreen.GetComponent<Image>().color;
            yield return new WaitForSeconds(fadeSpeed / 100); //fadeSpeed 초 후 완전히 어두워짐 
        }

        player.transform.position = transportPos; //플레이어 이동 

        //페이드 인 
        fadeScreenColor = fadeScreen.GetComponent<Image>().color;

        while (fadeScreenColor.a >= 0)
        {
            fadeScreen.GetComponent<Image>().color = new Color(0, 0, 0, fadeScreen.GetComponent<Image>().color.a - 0.01f);
            fadeScreenColor = fadeScreen.GetComponent<Image>().color;
            yield return new WaitForSeconds(fadeSpeed / 100); //fadeSpeed 초 후 완전히 밝아짐 
        }
    }   
}
