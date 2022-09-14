using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class openingAnimation : MonoBehaviour
{
    public GameObject[] cutScene = new GameObject[6];
    bool whileTyping = false;
    
    void Start()
    {
        //처음에는 모든 컷을 다 끈 상태로 시작 
        for (int index = 0; index < cutScene.Length; index++)
        {
            for(int j=0; j < cutScene[index].transform.childCount; j++)
            {
                cutScene[index].transform.GetChild(j).gameObject.SetActive(false);
            }
            cutScene[index].SetActive(false); 
        }

        StartCoroutine(sceneStart());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            //이 시점에서 nextScene 에는 tutorial 의 index 가 할당 
            SceneManager.LoadScene(GameManager.instance.nextScene); //tab 키 누르면 스킵 가능 
        }
    }

    IEnumerator sceneStart() //노가다 ~> 나중에 수정할 것 
    {
        //cut 1----------------------------
        yield return new WaitForSeconds(3f);
        cutScene[0].SetActive(true);
        cutScene[0].transform.GetChild(0).gameObject.SetActive(true); //이미지 보여주기 
        yield return new WaitForSeconds(1.5f);

        cutScene[0].transform.GetChild(1).gameObject.SetActive(true); //텍스트 보여주기
        StartCoroutine(textTyping(cutScene[0].transform.GetChild(1).GetComponent<Text>(), cutScene[0].transform.GetChild(1).GetComponent<Text>().text, 0.06f));
        
        while (true) //이전 타이핑 끝나기 전까지 그 다음으로 안넘어가게 통제 
        {
            if (!whileTyping) break;
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);

        cutScene[0].transform.GetChild(2).gameObject.SetActive(true); //텍스트 보여주기
        StartCoroutine(textTyping(cutScene[0].transform.GetChild(2).GetComponent<Text>(), cutScene[0].transform.GetChild(2).GetComponent<Text>().text, 0.06f));

        while (true) //이전 타이핑 끝나기 전까지 그 다음으로 안넘어가게 통제 
        {
            if (!whileTyping) break;
            yield return null;
        }
        yield return new WaitForSeconds(1f);

        cutScene[0].transform.GetChild(3).gameObject.SetActive(true); //텍스트 보여주기
        StartCoroutine(textTyping(cutScene[0].transform.GetChild(3).GetComponent<Text>(), cutScene[0].transform.GetChild(3).GetComponent<Text>().text, 0.06f));

        while (true) //이전 타이핑 끝나기 전까지 그 다음으로 안넘어가게 통제 
        {
            if (!whileTyping) break;
            yield return null;
        }
        yield return new WaitForSeconds(1f);

        cutScene[0].SetActive(false);
        yield return new WaitForSeconds(1f);

        //cut 2---------------------------
        cutScene[1].SetActive(true);
        cutScene[1].transform.GetChild(0).gameObject.SetActive(true); //이미지 보여주기 
        yield return new WaitForSeconds(1.5f);

        cutScene[1].transform.GetChild(1).gameObject.SetActive(true); //텍스트 보여주기
        StartCoroutine(textTyping(cutScene[1].transform.GetChild(1).GetComponent<Text>(), cutScene[1].transform.GetChild(1).GetComponent<Text>().text, 0.06f));

        while (true) //이전 타이핑 끝나기 전까지 그 다음으로 안넘어가게 통제 
        {
            if (!whileTyping) break;
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);

        cutScene[1].transform.GetChild(2).gameObject.SetActive(true); //텍스트 보여주기
        StartCoroutine(textTyping(cutScene[1].transform.GetChild(2).GetComponent<Text>(), cutScene[1].transform.GetChild(2).GetComponent<Text>().text, 0.06f));

        while (true) //이전 타이핑 끝나기 전까지 그 다음으로 안넘어가게 통제 
        {
            if (!whileTyping) break;
            yield return null;
        }
        yield return new WaitForSeconds(1f);

        cutScene[1].transform.GetChild(3).gameObject.SetActive(true); //텍스트 보여주기
        StartCoroutine(textTyping(cutScene[1].transform.GetChild(3).GetComponent<Text>(), cutScene[1].transform.GetChild(3).GetComponent<Text>().text, 0.06f));

        while (true) //이전 타이핑 끝나기 전까지 그 다음으로 안넘어가게 통제 
        {
            if (!whileTyping) break;
            yield return null;
        }
        yield return new WaitForSeconds(1f);

        cutScene[1].SetActive(false);
        yield return new WaitForSeconds(1f);

        //cut 3---------------------------
        cutScene[2].SetActive(true);
        cutScene[2].transform.GetChild(0).gameObject.SetActive(true); //이미지 보여주기 
        yield return new WaitForSeconds(1f);

        cutScene[2].transform.GetChild(1).gameObject.SetActive(true); //텍스트 보여주기
        StartCoroutine(textTyping(cutScene[2].transform.GetChild(1).GetComponent<Text>(), cutScene[2].transform.GetChild(1).GetComponent<Text>().text, 0.06f));

        while (true) //이전 타이핑 끝나기 전까지 그 다음으로 안넘어가게 통제 
        {
            if (!whileTyping) break;
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);

        cutScene[2].transform.GetChild(2).gameObject.SetActive(true); //텍스트 보여주기
        StartCoroutine(textTyping(cutScene[2].transform.GetChild(2).GetComponent<Text>(), cutScene[2].transform.GetChild(2).GetComponent<Text>().text, 0.06f));

        while (true) //이전 타이핑 끝나기 전까지 그 다음으로 안넘어가게 통제 
        {
            if (!whileTyping) break;
            yield return null;
        }
        yield return new WaitForSeconds(2f);

        cutScene[2].transform.GetChild(3).gameObject.SetActive(true); //텍스트 보여주기
        StartCoroutine(textTyping(cutScene[2].transform.GetChild(3).GetComponent<Text>(), cutScene[2].transform.GetChild(3).GetComponent<Text>().text, 0.06f));
        yield return new WaitForSeconds(2f);

        while (true) //이전 타이핑 끝나기 전까지 그 다음으로 안넘어가게 통제 
        {
            if (!whileTyping) break;
            yield return null;
        }
        yield return new WaitForSeconds(1f);

        cutScene[2].SetActive(false);
        yield return new WaitForSeconds(2f);

        //cut 4---------------------------
        cutScene[3].SetActive(true);
        cutScene[3].transform.GetChild(0).gameObject.SetActive(true); //이미지 보여주기 
        yield return new WaitForSeconds(1f);

        cutScene[3].transform.GetChild(1).gameObject.SetActive(true); //텍스트 보여주기
        StartCoroutine(textTyping(cutScene[3].transform.GetChild(1).GetComponent<Text>(), cutScene[3].transform.GetChild(1).GetComponent<Text>().text, 0.06f));

        while (true) //이전 타이핑 끝나기 전까지 그 다음으로 안넘어가게 통제 
        {
            if (!whileTyping) break;
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);

        cutScene[3].transform.GetChild(2).gameObject.SetActive(true); //텍스트 보여주기
        StartCoroutine(textTyping(cutScene[3].transform.GetChild(2).GetComponent<Text>(), cutScene[3].transform.GetChild(2).GetComponent<Text>().text, 0.06f));

        while (true) //이전 타이핑 끝나기 전까지 그 다음으로 안넘어가게 통제 
        {
            if (!whileTyping) break;
            yield return null;
        }
        yield return new WaitForSeconds(1f);
        cutScene[3].SetActive(false);
        yield return new WaitForSeconds(2f);

        //cut 5------------------------------
        cutScene[4].SetActive(true);
        cutScene[4].transform.GetChild(0).gameObject.SetActive(true); //텍스트 보여주기 
        StartCoroutine(textTyping(cutScene[4].transform.GetChild(0).GetComponent<Text>(), cutScene[4].transform.GetChild(0).GetComponent<Text>().text, 0.06f));

        while (true) //이전 타이핑 끝나기 전까지 그 다음으로 안넘어가게 통제 
        {
            if (!whileTyping) break;
            yield return null;
        }
        yield return new WaitForSeconds(2f);
        cutScene[4].SetActive(false); //텍스트 보여주기 
        yield return new WaitForSeconds(2f);

        //cut 6----------------------------
        cutScene[5].SetActive(true);
        cutScene[5].transform.GetChild(0).gameObject.SetActive(true); //이미지 보여주기 
        yield return new WaitForSeconds(3f);
        cutScene[5].transform.GetChild(1).gameObject.SetActive(true); //텍스트 보여주기 
        yield return new WaitForSeconds(3f);

        SceneManager.LoadScene(GameManager.instance.nextScene);
    }

    IEnumerator textTyping(Text typingText, string message, float period)
    {
        whileTyping = true;

        for (int i = 0; i < message.Length; i++)
        {
            typingText.text = message.Substring(0, i + 1);
            yield return new WaitForSeconds(period);
        }

        whileTyping = false;
    }

}
