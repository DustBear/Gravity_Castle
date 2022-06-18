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

    public GameObject chapter_Instruction; //챕터 설명 이미지
    public GameObject chapter_name; //챕터 이름 텍스트    
    public GameObject mapOpenSensor;

    public string stageNameText;
    public int stageNum; //시작해야 하는 스테이지 번호

    public int savePointCount; //해당 스테이지에 존재하는 세이브포인트의 개수
    public List<int> savePointScene; //이동해야 하는 씬 번호
    public List<Vector2> savePointPos; //해당 스테이지에 존재하는 세이브포인트의 위치값 
    public List<Vector2> savePointGravityDir; //해당 스테이지에 존재하는 각 세이브포인트에서 중력 방향

    
    [SerializeField] bool shouldButtonWork; 
    //이 스테이지로 이동할 수 있도록 해야 하는가 ~> 현재 GameData 상에서 이 시점까지 깨지 못했다면 버튼 비활성화해야 함

    void Start()
    {        
        
    }
      
    void Update()
    {       
        
    }

    public void Onclick() //이 버튼을 클릭하면
    {
        mapOpenSensor mapSensorScript = mapOpenSensor.GetComponent<mapOpenSensor>();

        chapter_name.GetComponent<TextMeshProUGUI>().text = stageNameText; //스테이지 이름 바꿔주기 
        mapSensorScript.selectedStageNum = stageNum;
        mapSensorScript.selectedSavePointNum = 0; //스테이지가 넘어가면 세이브포인트 번호도 0으로 초기화
        mapSensorScript.savePointCount = savePointCount;
        mapSensorScript.iconMake();
    }
}
