using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class stageMoveButton : MonoBehaviour
{
    //�� �� ǥ�õǴ� �̵� ��Ŀ�� ���� �Լ�
    //�ش� �������� ��ȣ�� ���̺�����Ʈ ���� �����ϰ� ������ŭ�� ���̺�����Ʈ ������ ����
    //Ŭ���ϸ� chapter_instruction ȭ�� ���̺�����Ʈ ��ġ�� �ٲ��ְ� chapter_name �ش� �������� �̸����� �ٲ���
    //���� �� ���������� �̵��� gameStartButton ������ ����

    public bool isActive;
    //�� �׸��� üũ�� ������ Ȱ��ȭ���� ���� �������� 

    public GameObject chapter_Instruction; //é�� ���� �̹���
    public GameObject chapter_name; //é�� �̸� �ؽ�Ʈ    
    public GameObject stageManager;

    public string stageNameText;
    public int stageNum; //�����ؾ� �ϴ� �������� ��ȣ

    public int savePointCount; //�ش� ���������� �����ϴ� ���̺�����Ʈ�� ����
    public List<int> savePointScene; //�̵��ؾ� �ϴ� �� ��ȣ
    
    public Sprite instruction_image;
    [SerializeField] bool shouldButtonWork; 
    //�� ���������� �̵��� �� �ֵ��� �ؾ� �ϴ°� ~> ���� GameData �󿡼� �� �������� ���� ���ߴٸ� ��ư ��Ȱ��ȭ�ؾ� ��

    Button button;
    public Sprite ActiveIcon;
    public Sprite deActiveIcon;

    AudioSource sound;
    public AudioClip correct;
    public AudioClip incorrect;
    void Awake()
    {
        button = GetComponent<Button>();
        button.interactable = true;
        sound = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (GameManager.instance.gameData.finalStageNum >= stageNum) //�� ��ư�� stageNum���� ���� �����߰ų� ������ 
        {
            shouldButtonWork = true;
            var colors = button.colors;

            button.image.sprite = ActiveIcon;
            button.colors = colors;
        }
        else
        {
            shouldButtonWork = false;
            button.image.sprite = deActiveIcon;
        }
    }

    public void Onclick() //�� ��ư�� Ŭ���ϸ�
    {
        if (!shouldButtonWork)
        {
            sound.Stop();
            sound.clip = incorrect;
            sound.Play();

            Debug.Log("not enough achievement");

            StopCoroutine("iconShake");
            StartCoroutine("iconShake");
            return;
        }

        sound.Stop();
        sound.clip = correct;
        sound.Play();

        stageManager stageManagerScr = stageManager.GetComponent<stageManager>();

        chapter_name.GetComponent<Text>().text = stageNameText; //�������� �̸� �ٲ��ֱ� 
        //chapter_Instruction.GetComponent<Image>().sprite = instruction_image;
        stageManagerScr.selectedStageNum = stageNum;
        stageManagerScr.selectedSavePointNum = 1; //���������� �Ѿ�� ���̺�����Ʈ ��ȣ�� 1�� �ʱ�ȭ
        stageManagerScr.savePointCount = savePointCount;
        stageManagerScr.iconMake();
    }

    IEnumerator iconShake()
    {
        for(int index=3; index>=1; index--)
        {
            transform.position = transform.position + new Vector3(0.08f, 0, 0);
            yield return new WaitForSeconds(0.05f);
            transform.position = transform.position + new Vector3(-0.08f, 0, 0);
            yield return new WaitForSeconds(0.05f);
            transform.position = transform.position + new Vector3(-0.08f, 0, 0);
            yield return new WaitForSeconds(0.05f);
            transform.position = transform.position + new Vector3(0.08f, 0, 0);
        }
    }
}
