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

    /*
    public GameObject fadeCircle;
    public Vector3 playerPos;   
    public float fadeCircleDelay;
    */
    AudioSource sound;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        sound = GetComponent<AudioSource>();

        fadeCoroutine = _FadeIn(1.5f);

        //���� �����ϸ� UI �޴��� ���� ���� �����ϱ� 
        inGameMenu.SetActive(false);
        existSavedGame.SetActive(false);
        noSavedGame.SetActive(false);

        //fadeCircle.SetActive(false);
    }

    private void Update()
    {
        
    }
    /*
    public void circleFade(bool bigger)
    {
        fadeCircle.SetActive(true);
        StartCoroutine(circleFadeCor(bigger));
    }
    IEnumerator circleFadeCor(bool bigger)
    {       
        if (bigger) //circle�� Ŀ���� �ϴ� ��� ~> fadeOut �� ���(��ο���)
        {           
            fadeCircle.SetActive(true);
            fadeCircle.GetComponent<RectTransform>().localScale = new Vector3(0,0,0);

            Vector2 circleAimPos = Camera.main.WorldToScreenPoint(playerPos);
            fadeCircle.GetComponent<RectTransform>().position = new Vector3(circleAimPos.x, circleAimPos.y, 5f);

            for (int index = 1; index <= 200; index++)
            {
                fadeCircle.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1) * (index / 200);
                yield return new WaitForSeconds(fadeCircleDelay / 200);
            }
        }
        else
        {
            fadeCircle.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);

            Vector2 circleAimPos = Camera.main.WorldToScreenPoint(playerPos);
            Debug.Log(circleAimPos);
            fadeCircle.GetComponent<RectTransform>().position = new Vector3(circleAimPos.x, circleAimPos.y, 5f);

            for (int index = 200; index >= 1; index--)
            {
                Debug.Log(fadeCircle.GetComponent<RectTransform>().localScale);
                fadeCircle.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1) * (index / 200);
                yield return new WaitForSeconds(fadeCircleDelay / 200);
            }

            fadeCircle.SetActive(false);
        }      
    }
    */

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
        Color color = fade.color;
        color.a = 1f;
        fade.color = color;

        yield return new WaitForSeconds(0.3f);
        while (color.a > 0f)
        {
            color.a -= 0.005f;
            fade.color = color;
            yield return wait;
        }
    }

    IEnumerator _FadeOut(float delayTime) //ȭ�� ��ο��� 
    {        
        var wait = new WaitForSeconds(delayTime/200);
        Color color = fade.color;
        while (fade.color.a < 1f)
        {           
            color.a += 0.005f;
            fade.color = color;
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

    public void clickSoundGen() //UI Ŭ���� �� ���� �Ҹ� �� 
    {
        sound.Play();
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
