using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : Singleton<UIManager>
{
    public GameObject inGameMenu;
    public GameObject fadeObj;
    Image fade;

    IEnumerator fadeCoroutine;

    AudioSource sound;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        sound = GetComponent<AudioSource>();

        fade = fadeObj.GetComponent<Image>();

        fadeCoroutine = _FadeIn(1.5f);
        fadeObj.SetActive(true);
        fade.color = new Color(0, 0, 0, 0); //�� ó�� �����ϸ� fade�� ����ȭ 

        //���� �����ϸ� UI �޴��� ���� ���� �����ϱ� 
        inGameMenu.SetActive(false);       
    }

    private void Update()
    {
        
    }
   
    // Fade in�� Fade out�� ���ÿ� ����� �� ���� �Ͽ���
    public void FadeIn(float fadeTime)
    {
        StopCoroutine(fadeCoroutine);
        fadeCoroutine = _FadeIn(fadeTime);
        StartCoroutine(fadeCoroutine);
    }

    public void FadeOut(float fadeTime)
    {
        StopCoroutine(fadeCoroutine);
        fadeCoroutine = _FadeOut(fadeTime);
        StartCoroutine(fadeCoroutine);
    }

    IEnumerator _FadeIn(float delayTime) //ȭ�� ����� 
    {        
        var wait = new WaitForSeconds(delayTime/200);
        fade.color = new Color(0, 0, 0, 1);
        
        yield return new WaitForSeconds(0.3f);
        while (fade.color.a > 0f)
        {
            fade.color = new Color(0, 0, 0, fade.color.a - 0.005f);
            yield return wait;
        }
    }

    IEnumerator _FadeOut(float delayTime) //ȭ�� ��ο��� 
    {        
        var wait = new WaitForSeconds(delayTime/200);
        fade.color = new Color(0, 0, 0, 0);
        while (fade.color.a < 1f)
        {
            fade.color = new Color(0, 0, 0, fade.color.a + 0.005f);
            yield return wait;
        }

        yield return new WaitForSeconds(1f);
    }
   
    public void OnOffInGameMenu() //�޴�â�� ���������� ���� ���������� Ų�� 
    {
        inGameMenu.SetActive(!inGameMenu.activeSelf);
        // �ΰ��� �޴� �˾�â Ȱ��ȭ �� �Ͻ�����
        if (inGameMenu.activeSelf)
        {
            Time.timeScale = 0f; //�ð� ����
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None; 
        }
        // �ΰ��� �޴� �˾�â ��Ȱ��ȭ �� �ٽ� ���
        else
        {
            Time.timeScale = 1f;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked; //���� ���߿��� ���콺 ���� �Ұ��� 
        }
    }

    public void clickSoundGen() //UI Ŭ���� �� ���� �Ҹ� �� 
    {
        sound.Play();
    }
}
