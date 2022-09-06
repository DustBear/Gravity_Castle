using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.SceneManagement;
using System.IO;

public class NewGameButton : MonoBehaviour
{
    public TextMeshProUGUI text;
    public bool isSaveFileExist;

    void Awake()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
    }


    void Start()
    {
        // SaveFile�� �ϳ��� �����ϸ� "�̾��ϱ�"�� �ؽ�Ʈ ����
        if (GameManager.instance.saveFileSeq.saveFileSeqList.Count != 0)
        {
            text.text = "이어하기";
            isSaveFileExist = true;
        }
        else
        {
            text.text = "새로하기";
            isSaveFileExist = false;
        }
    }

    public void OnClickButton() 
    {
        UIManager.instance.clickSoundGen();
        
        // SaveFile�� ������ �����ϱ� ~> GameData ���� ����� stageNum, achNum �־��� �� Ʃ�丮������� �̵���Ŵ 
        if (!isSaveFileExist)
        {
            //Debug.Log("new game");

            GameManager.instance.curSaveFileNum = 0; //ù ���̺� ���� ���� (1�� ���̺�) 
            GameManager.instance.saveFileSeq.saveFileSeqList.Add(0); //���������� ������ ���̺��ȣ ���� 
            GameManager.instance.SaveSaveFileSeq();

            //GM �� ���� ���� �� ���� ���� Ȱ��ȭ�ǹǷ� �̹� GameData �� ��������ִ� ���� ~> ���⼭ �ʱ�ȭ��Ű�� ������ 
            GameManager.instance.gameData.curStageNum = 1;
            GameManager.instance.gameData.curAchievementNum = 0; //ó�� �����ϴ� ���� ~> ���� ������������ �ٽ� ���� 
            GameManager.instance.gameData.finalStageNum = 1;
            GameManager.instance.gameData.finalAchievementNum = 0; //���������Ȳ�� ���� 

            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 50; j++)
                {
                    GameManager.instance.gameData.savePointUnlock[i, j] = false; //��� ���̺�����Ʈ ��Ȱ��ȭ (������ ó�� �����ϹǷ�) 
                }
            }

            //�ʱ�ȭ��Ų GameData [0]�� ���� 
            string ToJsonData = JsonUtility.ToJson(GameManager.instance.gameData);
            string filePath = Application.persistentDataPath + GameManager.instance.gameDataFileNames[0];
            File.WriteAllText(filePath, ToJsonData);

            GameManager.instance.nextScene = 2; //��������1 
            //nextPos, nextDir �� ������ �̵��� ���� savePointManager���� �˾Ƽ� ������ �� 

            GameManager.instance.shouldSpawnSavePoint = false;
            GameManager.instance.shouldUseOpeningElevator = true; //���� ���� �����ϹǷ� openingElevator ���� �����;� �� 

            SceneManager.LoadScene(37);
        }
        // SaveFile�� ������ ���� �ֱ� SaveFile�� Load
        else
        {
            //Debug.Log("load game");

            GameManager.instance.curSaveFileNum = GameManager.instance.saveFileSeq.saveFileSeqList.Last(); //���� �������� �÷����ߴ� ���̺����� ��ȣ ������ 
            
            string filePath = Application.persistentDataPath + GameManager.instance.gameDataFileNames[GameManager.instance.curSaveFileNum];
            string FromJsonData = File.ReadAllText(filePath);

            GameData curGameData = JsonUtility.FromJson<GameData>(FromJsonData); //���� ������ ���̺������� GameData ������

            //������ GameData ���� ������ �����ͼ� GM�� ������� 
            
            GameManager.instance.nextScene = curGameData.respawnScene;
            GameManager.instance.nextPos = curGameData.respawnPos;
            GameManager.instance.nextGravityDir = curGameData.respawnGravityDir;
            GameManager.instance.nextState = Player.States.Walk; //States.Walk �� �� ���� 

            GameManager.instance.shouldSpawnSavePoint = true;
            if(curGameData.curAchievementNum == 0)
            {
                GameManager.instance.shouldSpawnSavePoint = false;
                GameManager.instance.shouldUseOpeningElevator = true; //�ش� ���������� ó�� ������ �� opening Elevator �۵��ؾ� �� 
            }

            SceneManager.LoadScene(GameManager.instance.nextScene);
        }

    }
}
