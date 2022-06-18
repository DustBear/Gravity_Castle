using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public class GameData
{
    public int respawnScene {get; set;}
    public Vector2 respawnPos {get; set;}
    public Vector2 respawnGravityDir {get; set;}
    public bool isCliffChecked {get; set;}
    public bool[] storedIsShaked {get; set;}
    public bool[] storedIsMelted {get; set;}
    public bool[] storedIsDetected {get; set;}
    public bool[] storedIsGreen {get; set;}
    public Vector2[] storedPos {get; set;}    
    public int curAchievementNum {get; set;}
    public int curStageNum {get; set;}
}
