using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] GameObject inGameMenu;
    [SerializeField] GameObject existSavedGame;
    [SerializeField] GameObject noSavedGame;
    [SerializeField] Image fade;
    [SerializeField] GameObject textObj;
    IEnumerator fadeCoroutine;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        fadeCoroutine = _FadeIn(1f);

        //���� �����ϸ� UI �޴��� ���� ���� �����ϱ� 
        inGameMenu.SetActive(false);
        existSavedGame.SetActive(false);
        noSavedGame.SetActive(false);
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

    IEnumerator _FadeIn(float delayTime)
    {
        var wait = new WaitForSeconds(delayTime/50);
        Color color = fade.color;
        color.a = 1f;
        fade.color = color; 
        while (color.a > 0f)
        {
            color.a -= 0.02f;
            fade.color = color;
            yield return wait;
        }
    }

    IEnumerator _FadeOut(float delayTime)
    {
        var wait = new WaitForSeconds(delayTime/50);
        Color color = fade.color;
        while (fade.color.a < 1f)
        {           
            color.a += 0.02f;
            fade.color = color;
            yield return wait;
        }
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

    // New Game ��ư�� �����µ� �̹� ����� ������ ���� ��� �˾�â Ȱ��ȭ
    public void ExistSavedGame()
    {
        existSavedGame.SetActive(true);
    }

    // Load Game ��ư�� �����µ� ����� ������ ���� ��� �˾�â Ȱ��ȭ
    public void NoSavedGame()
    {
        noSavedGame.SetActive(true);
    }

    public void showText(string content)
    {
        textObj.SetActive(true);
        textObj.GetComponent<TextMeshProUGUI>().text = content;
    }

    public void hideText()
    {
        textObj.SetActive(false);
    }
}
