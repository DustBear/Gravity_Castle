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
        nextGravityScale = 2;

        // Die
        respawnScene = new int[3, 5]
        {{1, 1, 1, 2, 1}, {3, 4, 3, 3, 3}, {4, 4, 4, 4, 4}};
        
        respawnPos = new Vector2[3, 5]
        {{new Vector2(-161.5f, -7), new Vector2(-82, -2), new Vector2(-157.5f, 2), new Vector2(-196, 2), new Vector2(-157, 12)}
        , {new Vector2(-108.5f, 1), new Vector2(-110.6f, -11), new Vector2(-20.53f, -12.1f), new Vector2(-127.3f, 1.9f), new Vector2(-125.7f, -3.2f)}
        , {new Vector2(-162, -6), new Vector2(-162, -6), new Vector2(-162, -6), new Vector2(-162, -6), new Vector2(-162, -6)}};
        
        respawnRot = new Quaternion[3, 5]
        {{Quaternion.Euler(0, 0, 0), Quaternion.Euler(0, 0, -90), Quaternion.Euler(0, 0, 0), Quaternion.Euler(0, 0, 0), Quaternion.Euler(0, 0, 0)}
        , {Quaternion.Euler(0, 0, 0), Quaternion.Euler(0, 0, 0), Quaternion.Euler(0, 0, 0), Quaternion.Euler(0, 0, 0), Quaternion.Euler(0, 0, 0)}
        , {Quaternion.Euler(0, 0, 0), Quaternion.Euler(0, 0, 0), Quaternion.Euler(0, 0, 0), Quaternion.Euler(0, 0, 0), Quaternion.Euler(0, 0, 0)}};
        
        respawnGravity = new Vector2[3, 5]
        {{new Vector2(0, -9.8f), new Vector2(-9.8f, 0), new Vector2(0, -9.8f), new Vector2(0, -9.8f), new Vector2(0, -9.8f)}
        , {new Vector2(0, -9.8f), new Vector2(0, -9.8f), new Vector2(0, -9.8f), new Vector2(0, -9.8f), new Vector2(0, -9.8f)}
        , {new Vector2(0, -9.8f), new Vector2(0, -9.8f), new Vector2(0, -9.8f), new Vector2(0, -9.8f), new Vector2(0, -9.8f)}};
        
        respawnGravityDir = new GravityDirection[3, 5]
        {{GravityDirection.down, GravityDirection.left, GravityDirection.down, GravityDirection.down, GravityDirection.down}
        , {GravityDirection.down, GravityDirection.down, GravityDirection.down, GravityDirection.down, GravityDirection.down}
        , {GravityDirection.down, GravityDirection.down, GravityDirection.down, GravityDirection.down, GravityDirection.down}};
    }
}
