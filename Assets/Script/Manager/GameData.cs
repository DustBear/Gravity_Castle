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
    public bool[] storedIsShaked = new bool[36];
    public int curAchievementNum;
}
