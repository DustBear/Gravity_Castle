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
    bool isEscPressed = false;

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

        message = new string[3]; //message�� �� �ؽ�Ʈ�� ���� ��� ���� 

        message[0] = gameTextManager.instance.systemTextManager(27);
        message[1] = gameTextManager.instance.systemTextManager(28);
        message[2] = gameTextManager.instance.systemTextManager(29);

        StartCoroutine(openingSceneStart());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !isEscPressed)
        {
            SceneManager.LoadScene(4);
            isEscPressed = true;
        }
    }

    IEnumerator openingSceneStart()
    {
        UIManager.instance.FadeIn(1f);
        yield return new WaitForSeconds(2f); //�����ϸ� ��� �� 

        //��Ʃ��� ���� ũ����
        cutScene[0].SetActive(true);
        yield return new WaitForSeconds(2f);
        cutScene[0].SetActive(false);

        yield return new WaitForSeconds(2f);

        //��Ƽ�� ���� ũ����
        cutScene[1].SetActive(true);
        yield return new WaitForSeconds(2f);
        cutScene[1].SetActive(false);

        yield return new WaitForSeconds(2f);

        //#1 : ���ΰ��� ���䰡 �ٶ��� �𳯸� 
        StartCoroutine(cutSceneStart(cutScene[2], 2));        
        while (isSceneWorking)
        {
            yield return null;
        }
        yield return new WaitForSeconds(2f); //��� ��ο� ȭ�� 

        //#2 : �ؽ�Ʈ: Ȥ�� �˾�? ���ʱ濡 ������ ��, ��ȥ ���� ����� ���� ���̾� 

        textObj[0].text = message[0];
        textObj[0].gameObject.SetActive(true);

        yield return new WaitForSeconds(4.5f); //��� ��ο� ȭ�� 
        textObj[0].gameObject.SetActive(false);

        //#3 : ���ư��� ��Ϲ����� 
        StartCoroutine(cutSceneStart(cutScene[3], 2));
        while (isSceneWorking)
        {
            yield return null;
        }
        yield return new WaitForSeconds(1f);

        //#4 : ���������ͷ� �������� ������ �÷��̾� 
        StartCoroutine(cutSceneStart(cutScene[4], 2));
        while (isSceneWorking)
        {
            yield return null;
        }
        yield return new WaitForSeconds(2f);

        //#5 : �ؽ�ƮŸ���� : ���� ���Ŀ��� �̸��� ����� ���ؼ��ΰ�?

        textObj[1].text = message[1];
        textObj[1].gameObject.SetActive(true);

        yield return new WaitForSeconds(4.5f); //��� ��ο� ȭ�� 
        textObj[1].gameObject.SetActive(false);

        //#6 : ���������Ͱ� �ϰ��� 
        StartCoroutine(cutSceneStart(cutScene[5], 1));
        while (isSceneWorking)
        {
            yield return null;
        }
        yield return new WaitForSeconds(3f);


        //#7 : ����� ���� ������ 
        StartCoroutine(cutSceneStart(cutScene[6], 1));
        while (isSceneWorking)
        {
            yield return null;
        }
        yield return new WaitForSeconds(1.5f);

        //#8 : �ؽ�ƮŸ���� : �󸶳� ���� ������ �ڵ��� ����� �����ߴ���, ����ϱ� ���ؼ���

        textObj[2].text = message[2];
        textObj[2].gameObject.SetActive(true);
        yield return new WaitForSeconds(4.5f);

        UIManager.instance.FadeOut(0.5f);
        textObj[2].gameObject.SetActive(false);

        yield return new WaitForSeconds(2f);

        UIManager.instance.FadeIn(3f);
        //#9 : �׷���Ƽ ĳ���� �Ա��� �巯�� : �� ���� ���۰� ���� ���̵���/�ƿ� ȿ���� ���� 
        StartCoroutine(cutSceneStart(cutScene[7], 2));

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

    /*
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
    */

}
