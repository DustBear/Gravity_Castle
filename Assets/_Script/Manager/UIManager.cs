using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] GameObject inGameMenu;
    [SerializeField] GameObject existSavedGame;
    [SerializeField] GameObject noSavedGame;
    [SerializeField] Image fade;
    IEnumerator fadeCoroutine;

    [SerializeField] float fadeDelay; //ȭ�� ������ų� ��ο����µ� �ɸ��� �ð� 

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        fadeCoroutine = _FadeIn();

        //���� �����ϸ� UI �޴��� ���� ���� �����ϱ� 
        inGameMenu.SetActive(false);
        existSavedGame.SetActive(false);
        noSavedGame.SetActive(false);
    }

    // Fade in�� Fade out�� ���ÿ� ����� �� ���� �Ͽ���
    public void FadeIn()
    {
        StopCoroutine(fadeCoroutine);
        fadeCoroutine = _FadeIn();
        StartCoroutine(fadeCoroutine);
    }

    public void FadeOut()
    {
        StopCoroutine(fadeCoroutine);
        fadeCoroutine = _FadeOut();
        StartCoroutine(fadeCoroutine);
    }

    IEnumerator _FadeIn()
    {
        var wait = new WaitForSeconds(fadeDelay/20);
        Color color = fade.color;
        color.a = 1f;
        fade.color = color; 
        while (color.a > 0f)
        {
            color.a -= 0.05f;
            fade.color = color;
            yield return wait;
        }
    }

    IEnumerator _FadeOut()
    {
        InputManager.instance.isInputBlocked = true;

        var wait = new WaitForSeconds(0.1f);
        while (fade.color.a < 1f)
        {
            Color color = fade.color;
            color.a += 0.05f;
            fade.color = color;
            yield return wait;
        }

        GameManager.instance.StartGame(false);
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
}
