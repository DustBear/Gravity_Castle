using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Store informations
    public bool isDie;
    public int curStage; // First stage is 0
    public int curState; // 0: no key, no door  1: key1  2: door1  3: key2   4: door2
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
    public float nextGravityScale;
    public bool nextIsJumping;
    public bool nextIsRoping;
    public bool nextIsCollideRope;

    // If die
    public int[,] respawnScene;
    public Vector2[,] respawnPos;
    public GravityDirection[,] respawnGravityDir;

    void Awake() {
        DontDestroyOnLoad(gameObject);
        
        // Next scene
        nextPos = new Vector2(-193f, 2.1f);
        nextGravityDir = GravityDirection.down;
        /*nextPos = new Vector2(-162, -7);
        nextGravityDir = GravityDirection.down;*/
        nextGravityScale = 2;

        // Die
        respawnScene = new int[4, 5]
        {{1, 2, 1, 3, 1}, {4, 5, 4, 4, 4}, {7, 7, 7, 8, 7}, {9, 9, 9, 9, 9}};
        
        respawnPos = new Vector2[4, 5]
        {{new Vector2(-161.5f, -7), new Vector2(-82, -2), new Vector2(-157.5f, 2), new Vector2(-196, 2), new Vector2(-157, 12)}
        , {new Vector2(-108.5f, 1), new Vector2(-110.6f, -11), new Vector2(-20.53f, -12.1f), new Vector2(-127.3f, 1.9f), new Vector2(-125.7f, -3.2f)}
        , {new Vector2(-141.0f, -10.0f), new Vector2(-102.4f, -11.26f), new Vector2(-151.9f, 2.02f), new Vector2(-211.7f, 11.5f), new Vector2(-150.5f, -16.98f)}
        , {new Vector2(-193f, 2.1f), new Vector2(-179.2f, -7.7f), new Vector2(-179.2f, -7.7f), new Vector2(-179.2f, -7.7f), new Vector2(-179.2f, -7.7f)}};

        respawnGravityDir = new GravityDirection[4, 5]
        {{GravityDirection.down, GravityDirection.left, GravityDirection.down, GravityDirection.down, GravityDirection.down}
        , {GravityDirection.down, GravityDirection.down, GravityDirection.down, GravityDirection.down, GravityDirection.down}
        , {GravityDirection.down, GravityDirection.down, GravityDirection.up, GravityDirection.left, GravityDirection.right}
        , {GravityDirection.down, GravityDirection.right, GravityDirection.right, GravityDirection.right, GravityDirection.right}};;
    }
}
