using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public class GameData
{
    public int respawnScene;
    public Vector2 respawnPos;
    public Vector2 respawnGravityDir;
    public bool isCliffChecked;
    public bool[] storedIsShaked;
    public bool[] storedIsMelted;
    public bool[] storedIsDetected;
    public bool[] storedIsGreen;
    public Vector2[] storedPos;
    public int curAchievementNum;
    public int curStageNum;
    public int finalAchievementNum;
    public int finalStageNum;
    public int mapStageNum; // ���� �󿡼� ���µ� �ִ� ��������
    public bool[] sideStageUnlock; //true�̸� sideStage Ȱ��ȭ��Ų ��
}
