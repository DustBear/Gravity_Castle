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
            cutScene[n].SetActive(false); //시작하면 모든 컷씬 비활성화 
        }
        for (int n = 0; n < textObj.Length; n++)
        {
            textObj[n].gameObject.SetActive(false); //시작하면 모든 텍스트 비활성화 
        }

        message = new string[3]; //message는 각 텍스트의 내용 담고 있음 

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
        yield return new WaitForSeconds(2f); //시작하면 잠시 쉼 

        //스튜디오 레프 크레딧
        cutScene[0].SetActive(true);
        yield return new WaitForSeconds(2f);
        cutScene[0].SetActive(false);

        yield return new WaitForSeconds(2f);

        //아티브 사운드 크레딧
        cutScene[1].SetActive(true);
        yield return new WaitForSeconds(2f);
        cutScene[1].SetActive(false);

        yield return new WaitForSeconds(2f);

        //#1 : 주인공의 망토가 바람에 흩날림 
        StartCoroutine(cutSceneStart(cutScene[2], 2));        
        while (isSceneWorking)
        {
            yield return null;
        }
        yield return new WaitForSeconds(2f); //잠시 어두운 화면 

        //#2 : 텍스트: 혹시 알아? 순례길에 오르기 전, 영혼 비석을 세우는 이유 말이야 

        textObj[0].text = message[0];
        textObj[0].gameObject.SetActive(true);

        yield return new WaitForSeconds(4.5f); //잠시 어두운 화면 
        textObj[0].gameObject.SetActive(false);

        //#3 : 돌아가는 톱니바퀴들 
        StartCoroutine(cutSceneStart(cutScene[3], 2));
        while (isSceneWorking)
        {
            yield return null;
        }
        yield return new WaitForSeconds(1f);

        //#4 : 엘리베이터로 내려가기 직전의 플레이어 
        StartCoroutine(cutSceneStart(cutScene[4], 2));
        while (isSceneWorking)
        {
            yield return null;
        }
        yield return new WaitForSeconds(2f);

        //#5 : 텍스트타이핑 : 죽은 이후에도 이름을 남기기 위해서인가?

        textObj[1].text = message[1];
        textObj[1].gameObject.SetActive(true);

        yield return new WaitForSeconds(4.5f); //잠시 어두운 화면 
        textObj[1].gameObject.SetActive(false);

        //#6 : 엘리베이터가 하강함 
        StartCoroutine(cutSceneStart(cutScene[5], 1));
        while (isSceneWorking)
        {
            yield return null;
        }
        yield return new WaitForSeconds(3f);


        //#7 : 즐비한 비석을 보여줌 
        StartCoroutine(cutSceneStart(cutScene[6], 1));
        while (isSceneWorking)
        {
            yield return null;
        }
        yield return new WaitForSeconds(1.5f);

        //#8 : 텍스트타이핑 : 얼마나 많은 무지한 자들이 목숨을 낭비했는지, 경고하기 위해서야

        textObj[2].text = message[2];
        textObj[2].gameObject.SetActive(true);
        yield return new WaitForSeconds(4.5f);

        UIManager.instance.FadeOut(0.5f);
        textObj[2].gameObject.SetActive(false);

        yield return new WaitForSeconds(2f);

        UIManager.instance.FadeIn(3f);
        //#9 : 그래비티 캐슬의 입구가 드러남 : 이 씬은 시작과 끝에 페이드인/아웃 효과를 넣음 
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

    IEnumerator cutSceneStart(GameObject scene, int repeatTime) //원하는 씬 애니메이션을 실행시키는 함수 
    {
        scene.SetActive(true);
        isSceneWorking = true;

        openingAnim_cut sceneScr = scene.GetComponent<openingAnim_cut>();
        float frameDelay = sceneScr.frameDelay;

        for(int r=0; r<repeatTime; r++) //애니메이션이 루프인 경우 여러 번 반복해야 하는 경우가 있을 수 있음 
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
