using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Store informations
    public bool isDie;
    public int curStage; // First stage is 0
    public int curState; // 0 ~ 4
    public bool isOpenKeyBox1;
    public bool isOpenKeyBox2;
    public bool isGetKey1;
    public bool isGetKey2;
    public bool isOpenDoor1;
    public bool isOpenDoor2;
    public enum GravityDirection {left, down, right, up};

    // If next scene
    public Vector2 nextPos;
    public Quaternion nextRot;
    public Vector2 nextGravity;
    public bool nextAfterRotating;
    public GravityDirection nextGravityDir;

    // If die
    public int[,] respawnScene;
    public Vector2[,] respawnPos;
    public Quaternion[,] respawnRot;
    public Vector2[,] respawnGravity;
    public GravityDirection[,] respawnGravityDir;

    void Awake() {
        DontDestroyOnLoad(gameObject);
        
        // Next scene
        nextPos = new Vector2(-162, -7);
        nextRot = Quaternion.Euler(0, 0, 0);
        nextGravity = new Vector2(0, -9.8f);
        nextGravityDir = GravityDirection.down;

        // Die
        respawnScene = new int[2, 5]
        {{1, 1, 1, 2, 3}, {3, 3, 3, 3, 3}};
        respawnPos = new Vector2[2, 5]
        {{new Vector2(-161.5f, -7), new Vector2(-82, -2), new Vector2(-157.5f, 2), new Vector2(-196, 2), new Vector2(-110.5f, -11)}
        , {new Vector2(-110.6f, -11), new Vector2(-110.6f, -11), new Vector2(-110.6f, -11), new Vector2(-110.6f, -11), new Vector2(-110.6f, -11)}};
        respawnRot = new Quaternion[2, 5]
        {{Quaternion.Euler(0, 0, 0), Quaternion.Euler(0, 0, -90), Quaternion.Euler(0, 0, 0), Quaternion.Euler(0, 0, 0), Quaternion.Euler(0, 0, 0)}
        , {Quaternion.Euler(0, 0, 0), Quaternion.Euler(0, 0, 0), Quaternion.Euler(0, 0, 0), Quaternion.Euler(0, 0, 0), Quaternion.Euler(0, 0, 0)}};
        respawnGravity = new Vector2[2, 5]
        {{new Vector2(0, -9.8f), new Vector2(-9.8f, 0), new Vector2(0, -9.8f), new Vector2(0, -9.8f), new Vector2(0, -9.8f)}
        , {new Vector2(0, -9.8f), new Vector2(0, -9.8f), new Vector2(0, -9.8f), new Vector2(0, -9.8f), new Vector2(0, -9.8f)}};
        respawnGravityDir = new GravityDirection[2, 5]
        {{GravityDirection.down, GravityDirection.left, GravityDirection.down, GravityDirection.down, GravityDirection.down}
        , {GravityDirection.down, GravityDirection.down, GravityDirection.down, GravityDirection.down, GravityDirection.down}};
    }
}
