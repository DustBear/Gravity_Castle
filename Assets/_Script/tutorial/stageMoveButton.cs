using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class stageMoveButton : MonoBehaviour
{
    //맵 상에 표시되는 이동 마커에 들어가는 함수
    //해당 스테이지 번호와 세이브포인트 개수 저장하고 개수만큼의 세이브포인트 아이콘 만듦
    //클릭하면 chapter_instruction 화면 세이브포인트 위치로 바꿔주고 chapter_name 해당 스테이지 이름으로 바꿔줌
    //이후 그 스테이지로 이동은 gameStartButton 눌러서 수행

    public bool isActive;
    //이 항목이 체크돼 있으면 활성화되지 않은 스테이지 

    public GameObject chapter_Instruction; //챕터 설명 이미지
    public GameObject chapter_name; //챕터 이름 텍스트    
    public GameObject stageManager;

    public string stageNameText;
    public int stageNum; //시작해야 하는 스테이지 번호

    public int savePointCount; //해당 스테이지에 존재하는 세이브포인트의 개수
    public List<int> savePointScene; //이동해야 하는 씬 번호
    
    public Sprite instruction_image;
    [SerializeField] bool shouldButtonWork; 
    //이 스테이지로 이동할 수 있도록 해야 하는가 ~> 현재 GameData 상에서 이 시점까지 깨지 못했다면 버튼 비활성화해야 함

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
        if (GameManager.instance.gameData.finalStageNum >= stageNum) //이 버튼의 stageNum보다 많이 진행했거나 같으면 
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

    public void Onclick() //이 버튼을 클릭하면
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

        chapter_name.GetComponent<Text>().text = stageNameText; //스테이지 이름 바꿔주기 
        //chapter_Instruction.GetComponent<Image>().sprite = instruction_image;
        stageManagerScr.selectedStageNum = stageNum;
        stageManagerScr.selectedSavePointNum = 1; //스테이지가 넘어가면 세이브포인트 번호도 1로 초기화
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
