using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : Singleton<UIManager>
{
    public GameObject inGameMenu;
    public GameObject optionMenu;
    public GameObject fadeObj;
    public Image fade;

    public GameObject collectionMenu;
    public GameObject collectionMenu_fade;
    public GameObject col_alarm;

    string col_getText = "Ž�谡 ���ڸ� Ȯ���߽��ϴ�.\n����� ��ȥ �񼮿��� �����ϰ� ������ �� �ֽ��ϴ�." ;
    string col_saveText = "������ Ž�谡 ���ڸ� ��ȥ �񼮿� �����߽��ϴ�.";

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
        optionMenu.SetActive(false);

        //Ž�谡���� ���� �޴��� �� ���� ��. ��, ��ο� ����� ���ּ� ������ �ʰ� ����� 
        collectionMenu.SetActive(true);
        collectionMenu_fade.SetActive(false);
        col_alarm.GetComponent<Text>().color = new Color(1, 1, 1, 0); //�˶� �ؽ�Ʈ ����ȭ 
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
            inGameMenu.GetComponent<InGameMenu>().collectionIconDel(); //������� ������ ����� 
        }
    }

    public void collectionAlarm(int type)
    {
        switch (type)
        {
            case 1: //Ž�谡���ڸ� ó�� �������� �� 
                StopCoroutine("collectionGet");
                StopCoroutine("collectionSave");

                StartCoroutine(collectionGet());
                break;

            case 2: //Ž�谡���ڸ� �������� �� 
                StopCoroutine("collectionGet");
                StopCoroutine("collectionSave");

                StartCoroutine(collectionSave());
                break;
        }
    }

    public IEnumerator collectionGet() //Ž�谡���ڸ� ó�� �������� �� ���� 
    {
        Text alarmText = col_alarm.GetComponent<Text>();
        col_alarm.GetComponent<Text>().color = new Color(1, 1, 1, 0);

        float alphaTimer = 0f;

        alarmText.text = col_getText;
        
        while (alarmText.color.a <= 1)
        {
            alarmText.color = new Color(1, 1, 1, alphaTimer/0.5f);
            alphaTimer += Time.deltaTime;
            yield return null;
        }
        alarmText.color = new Color(1, 1, 1, 1); //0.5�ʿ� ���� �˶� ����� 

        alphaTimer = 0.5f;
        yield return new WaitForSeconds(4f);

        while (alarmText.color.a >= 0 )
        {
            alarmText.color = new Color(1, 1, 1, alphaTimer / 0.5f);
            alphaTimer -= Time.deltaTime;
            yield return null;
        }
        alarmText.color = new Color(1, 1, 1, 0); //0.5�ʿ� ���� �˶� ��ο���  
    }

    public IEnumerator collectionSave() //Ž�谡���ڸ� ���̺꿡�� �������� �� ���� 
    {
        Text alarmText = col_alarm.GetComponent<Text>();
        col_alarm.GetComponent<Text>().color = new Color(1, 1, 1, 0);

        float alphaTimer = 0f;

        alarmText.text = col_saveText;

        while (alarmText.color.a <= 1)
        {
            alarmText.color = new Color(1, 1, 1, alphaTimer / 0.5f);
            alphaTimer += Time.deltaTime;
            yield return null;
        }
        alarmText.color = new Color(1, 1, 1, 1); //0.5�ʿ� ���� �˶� ����� 

        alphaTimer = 0.5f;
        yield return new WaitForSeconds(4f);

        while (alarmText.color.a >= 0)
        {
            alarmText.color = new Color(1, 1, 1, alphaTimer / 0.5f);
            alphaTimer -= Time.deltaTime;
            yield return null;
        }
        alarmText.color = new Color(1, 1, 1, 0); //0.5�ʿ� ���� �˶� ��ο���  
    }

    public void clickSoundGen() //UI Ŭ���� �� ���� �Ҹ� �� 
    {
        sound.Play();
    }

    public void cameraShake(float size, float length)
    {
        GameObject.Find("Main Camera").GetComponent<MainCamera>().cameraShake(size, length);
    }

    public void OnClickOptionMenu() //�ɼ� â �ѱ� ��ư�� ���� �� 
    {
        clickSoundGen();
        optionMenu.SetActive(true);
    }

    public void optionDisable()
    {
        clickSoundGen();
        optionMenu.SetActive(false);
    }
}

