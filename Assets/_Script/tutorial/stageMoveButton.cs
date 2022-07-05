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

    public GameObject chapter_Instruction; //é�� ���� �̹���
    public GameObject chapter_name; //é�� �̸� �ؽ�Ʈ    
    public GameObject mapOpenSensor;

    public string stageNameText;
    public int stageNum; //�����ؾ� �ϴ� �������� ��ȣ

    public int savePointCount; //�ش� ���������� �����ϴ� ���̺�����Ʈ�� ����
    public List<int> savePointScene; //�̵��ؾ� �ϴ� �� ��ȣ
    public List<Vector2> savePointPos; //�ش� ���������� �����ϴ� ���̺�����Ʈ�� ��ġ�� 
    public List<Vector2> savePointGravityDir; //�ش� ���������� �����ϴ� �� ���̺�����Ʈ���� �߷� ����

    public Sprite instruction_image;
    [SerializeField] bool shouldButtonWork; 
    //�� ���������� �̵��� �� �ֵ��� �ؾ� �ϴ°� ~> ���� GameData �󿡼� �� �������� ���� ���ߴٸ� ��ư ��Ȱ��ȭ�ؾ� ��

    Button button;

    void Awake()
    {
        button = GetComponent<Button>();
    }

    void Start()
    {
        if (GameManager.instance.gameData.finalStageNum >= stageNum)
        {
            button.interactable = true;
            var colors = button.colors;
            colors.normalColor = new Color(1f, 1f, 1f, 1f);
            button.colors = colors;
        }
    }

    public void Onclick() //�� ��ư�� Ŭ���ϸ�
    {
        mapOpenSensor mapSensorScript = mapOpenSensor.GetComponent<mapOpenSensor>();

        chapter_name.GetComponent<TextMeshProUGUI>().text = stageNameText; //�������� �̸� �ٲ��ֱ� 
        chapter_Instruction.GetComponent<Image>().sprite = instruction_image;
        mapSensorScript.selectedStageNum = stageNum;
        mapSensorScript.selectedSavePointNum = 0; //���������� �Ѿ�� ���̺�����Ʈ ��ȣ�� 0���� �ʱ�ȭ
        mapSensorScript.savePointCount = savePointCount;
        mapSensorScript.iconMake();
    }
}
