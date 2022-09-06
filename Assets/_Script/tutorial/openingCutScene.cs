using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class openingCutScene : MonoBehaviour
{
    public GameObject nextCut;
    public float startOffset; //텍스트 시작하기 전 오프셋 
    public float showLength; //텍스트를 보여주는 시간 

    float typingInterval = 0.085f;
    public bool thisTextFirst = true;
    public bool thisTextLast = false;

    Text content;
    string initMessage;

    private void Awake()
    {
        content = GetComponent<Text>();
        initMessage = content.text;
        content.text = "";
    }

    private void OnEnable()
    {
        StartCoroutine(textShow());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            SceneManager.LoadScene(GameManager.instance.nextScene);
        }
    }

    IEnumerator textShow()
    {
        yield return new WaitForSeconds(startOffset);
        StartCoroutine(textTyping(content, initMessage, typingInterval));
    }
    IEnumerator textTyping(Text typingText, string message, float period)
    {
        for (int i = 0; i < message.Length; i++)
        {
            typingText.text = message.Substring(0, i + 1);
            yield return new WaitForSeconds(period);
        }

        yield return new WaitForSeconds(showLength);

        if(nextCut != null)
        {
            nextCut.SetActive(true);
            gameObject.SetActive(false);
        }
        else
        {
            SceneManager.LoadScene(GameManager.instance.nextScene);
        }       
    }

}
