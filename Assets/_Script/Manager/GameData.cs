using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public class GameData
{
    public int respawnScene;
    public Vector2 respawnPos;
    public Vector2 respawnGravityDir;

    /*
    public bool isCliffChecked;
    public bool[] storedIsShaked;
    public bool[] storedIsMelted;
    public bool[] storedIsDetected;
    public bool[] storedIsGreen;
    public Vector2[] storedPos;
    */

    public int curAchievementNum;
    public int curStageNum;

    public int finalAchievementNum;
    public int finalStageNum;

    public int[] savePointUnlock; //0부터 7까지 8개 스테이지, 각 최대 50개의 세이브포인트 ~> 0이면 false, 1이면 true
    public bool[] collectionUnlock; //각 수집요소 수집 여부 저장. 각 최대 30개의 수집요소 
}
