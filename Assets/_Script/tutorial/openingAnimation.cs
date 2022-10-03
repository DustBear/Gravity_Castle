using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class openingAnimation : MonoBehaviour
{
    public GameObject[] cutScene;
    public Text[] textObj;
    public string[] message;

    bool isSceneWorking = false;

    private void Start()
    {
        for(int n=0; n<cutScene.Length; n++)
        {
            cutScene[n].SetActive(false); //�����ϸ� ��� �ƾ� ��Ȱ��ȭ 
        }
        for (int n = 0; n < textObj.Length; n++)
        {
            textObj[n].gameObject.SetActive(false); //�����ϸ� ��� �ؽ�Ʈ ��Ȱ��ȭ 
        }

        message = new string[textObj.Length]; //message�� �� �ؽ�Ʈ�� ���� ��� ���� 
        for(int index=0; index<textObj.Length; index++)
        {
            message[index] = textObj[index].text;
        }

        StartCoroutine(openingSceneStart());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(4);
        }
    }

    IEnumerator openingSceneStart()
    {
        yield return new WaitForSeconds(3f); //�����ϸ� ��� �� 

        //#1 : ���ΰ��� ���䰡 �ٶ��� �𳯸� 
        StartCoroutine(cutSceneStart(cutScene[0], 2));        
        while (isSceneWorking)
        {
            yield return null;
        }
        yield return new WaitForSeconds(2f); //��� ��ο� ȭ�� 

        //#2 : �ؽ�ƮŸ����: Ȥ�� �˾�? ���ʱ濡 ������ ��, ��ȥ ���� ����� ���� ���̾� 
        StartCoroutine(textTyping(textObj[0], message[0], 0.08f));
        while (whileTyping)
        {
            yield return null;
        }
        yield return new WaitForSeconds(1.5f); //��� ��ο� ȭ�� 

        //#3 : ���ư��� ��Ϲ����� 
        StartCoroutine(cutSceneStart(cutScene[1], 2));
        while (isSceneWorking)
        {
            yield return null;
        }
        yield return new WaitForSeconds(1f);

        //#4 : ���������ͷ� �������� ������ �÷��̾� 
        StartCoroutine(cutSceneStart(cutScene[2], 2));
        while (isSceneWorking)
        {
            yield return null;
        }
        yield return new WaitForSeconds(2f);

        //#5 : �ؽ�ƮŸ���� : ���� ���Ŀ��� �̸��� ����� ���ؼ��ΰ�?
        StartCoroutine(textTyping(textObj[1], message[1], 0.08f));
        while (whileTyping)
        {
            yield return null;
        }
        yield return new WaitForSeconds(2f); //��� ��ο� ȭ�� 

        //#6 : ���������Ͱ� �ϰ��� 
        StartCoroutine(cutSceneStart(cutScene[3], 1));
        while (isSceneWorking)
        {
            yield return null;
        }
        yield return new WaitForSeconds(3f);


        //#7 : ����� ���� ������ 
        StartCoroutine(cutSceneStart(cutScene[4], 1));
        while (isSceneWorking)
        {
            yield return null;
        }
        yield return new WaitForSeconds(1.5f);

        //#8 : �ؽ�ƮŸ���� : �󸶳� ���� ������ �ڵ��� ����� �����ߴ���, ����ϱ� ���ؼ���
        StartCoroutine(textTyping(textObj[2], message[2], 0.08f));
        while (whileTyping)
        {
            yield return null;
        }

        UIManager.instance.FadeOut(0.1f);
        yield return new WaitForSeconds(2f);

        UIManager.instance.FadeIn(3f);
        //#9 : �׷���Ƽ ĳ���� �Ա��� �巯�� : �� ���� ���۰� ���� ���̵���/�ƿ� ȿ���� ���� 
        StartCoroutine(cutSceneStart(cutScene[5], 2));

        float timer = 0;
        while (isSceneWorking)
        {
            timer += Time.deltaTime;
            if (timer >= 10f)
            {
                UIManager.instance.FadeOut(3f);
                yield return new WaitForSeconds(4f);
                SceneManager.LoadScene(4);
            }
            yield return null;
        }

        /*
        //#10 : �ؽ�Ʈ ���̵���(Ÿ����ȿ�� ����) : ���� �뼭���� ���ϸ���. 
        textObj[3].gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);

        for(int index=1; index<=100; index++)
        {
            textObj[3].color = new Color(1, 1, 1, 1 - index * 0.01f);
            yield return new WaitForSeconds(0.015f);
        }
        */

    }

    IEnumerator cutSceneStart(GameObject scene, int repeatTime) //���ϴ� �� �ִϸ��̼��� �����Ű�� �Լ� 
    {
        scene.SetActive(true);
        isSceneWorking = true;

        openingAnim_cut sceneScr = scene.GetComponent<openingAnim_cut>();
        float frameDelay = sceneScr.frameDelay;

        for(int r=0; r<repeatTime; r++) //�ִϸ��̼��� ������ ��� ���� �� �ݺ��ؾ� �ϴ� ��찡 ���� �� ���� 
        {
            for (int index = 0; index < sceneScr.spriteGroup.Length; index++)
            {
                scene.GetComponent<SpriteRenderer>().sprite = sceneScr.spriteGroup[index];
                yield return new WaitForSeconds(frameDelay);
            }
        }

        scene.SetActive(false);
        isSceneWorking = false;
    }

    bool whileTyping;

    IEnumerator textTyping(Text typingText, string message, float period)
    {
        whileTyping = true;
        typingText.gameObject.SetActive(true);

        for (int i = 0; i < message.Length; i++)
        {
            typingText.text = message.Substring(0, i + 1);
            yield return new WaitForSeconds(period);
        }

        yield return new WaitForSeconds(1.5f);
        typingText.gameObject.SetActive(false);
        whileTyping = false;
    }


}
