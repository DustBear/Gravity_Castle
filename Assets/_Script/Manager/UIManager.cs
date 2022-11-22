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
    public Image fade;

    IEnumerator fadeCoroutine;

    AudioSource sound;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        sound = GetComponent<AudioSource>();

        fade = fadeObj.GetComponent<Image>();

        fadeCoroutine = _FadeIn(1f);
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

    float fadeTimer;
    IEnumerator _FadeIn(float delayTime) //����� ~> ���İ� �������� �� 
    {
        fade.color = new Color(0, 0, 0, 1);

        while(fade.color.a > 0f)
        {
            float alphaValue = fade.color.a;

            alphaValue -= Time.deltaTime / delayTime;
            fade.color = new Color(0, 0, 0, alphaValue);
            yield return null;
        }

        fade.color = new Color(0, 0, 0, 0);
    }

    IEnumerator _FadeOut(float delayTime) //��ο��� ~> ���İ� �������� �� 
    {
        while (fade.color.a < 1f)
        {
            float alphaValue = fade.color.a;

            alphaValue += Time.deltaTime / delayTime;
            fade.color = new Color(0, 0, 0, alphaValue);
            yield return null;
        }

        fade.color = new Color(0, 0, 0, 1);
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

    public void cameraShake(float size, float length)
    {
        GameObject.Find("Main Camera").GetComponent<MainCamera>().cameraShake(size, length);
    }
}
