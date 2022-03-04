using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] GameObject inGameMenu;
    [SerializeField] Image fade;
    IEnumerator fadeCoroutine;

    protected UIManager() {}

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        fadeCoroutine = _FadeIn();
    }

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
        Color color = fade.color;
        color.a = 1f;
        fade.color = color; 
        while (color.a > 0f)
        {
            //color = fade.color;
            color.a -= 0.05f;
            fade.color = color;
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator _FadeOut()
    {
        while (fade.color.a < 1f)
        {
            Color color = fade.color;
            color.a += 0.05f;
            fade.color = color;
            yield return new WaitForSeconds(0.1f);
        }
        SceneManager.LoadScene(GameManager.instance.respawnScene);
    }

    public void OnOffInGameMenu()
    {
        inGameMenu.SetActive(!inGameMenu.activeSelf);
        if (inGameMenu.activeSelf)
        {
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
