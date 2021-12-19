using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    // Object Pool
    public enum Type {arrow, cannon};
    public GameObject arrow;
    public GameObject cannon;
    public GameObject fire;
    public GameObject fireFalling;
    public int arrowNum;
    public int cannonNum;
    public int fireNum;
    public int fireFallingNum;
    public Queue<GameObject> arrowQueue;
    public Queue<GameObject> cannonQueue;
    public Queue<GameObject> fireQueue;
    public Queue<GameObject> fireFallingQueue;

    // Store informations
    public bool isDie;
    public int curStage; // First stage is 1
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
    public bool nextIsJumping;
    public bool nextIsRoping;

    // If die
    public int[,] respawnScene;
    public Vector2[,] respawnPos;
    public GravityDirection[,] respawnGravityDir;
    public bool[,] respawnIsRoping;

    // shaked floor
    public bool[] curIsShaked; // size is the number of floors
    public bool[] storedIsShaked; // size is the number of floors
    int storedCurState;

    // rotating
    public bool isRotating;

    void Awake() {
        instance = this;
        DontDestroyOnLoad(gameObject);

        // Object Pool
        arrowQueue = new Queue<GameObject>();
        cannonQueue = new Queue<GameObject>();
        fireQueue = new Queue<GameObject>();
        fireFallingQueue = new Queue<GameObject>();
        for (int i = 0; i < arrowNum; i++) {
            GameObject newArrow = Instantiate(arrow);
            DontDestroyOnLoad(newArrow);
            arrowQueue.Enqueue(newArrow);
            newArrow.SetActive(false);
        }
        for (int i = 0; i < cannonNum; i++) {
            GameObject newCannon = Instantiate(cannon);
            DontDestroyOnLoad(newCannon);
            cannonQueue.Enqueue(newCannon);
            newCannon.SetActive(false);
        }
        for (int i = 0; i < fireNum; i++) {
            GameObject newFire = Instantiate(fire);
            DontDestroyOnLoad(newFire);
            fireQueue.Enqueue(newFire);
            newFire.SetActive(false);
        }
        for (int i = 0; i < fireFallingNum; i++) {
            GameObject newFireFalling = Instantiate(fireFalling);
            DontDestroyOnLoad(newFireFalling);
            fireFallingQueue.Enqueue(newFireFalling);
            newFireFalling.SetActive(false);
        }

        // Die
        respawnScene = new int[5, 5]
        {{1, 2, 1, 3, 1}, {4, 5, 4, 4, 4}, {7, 7, 7, 8, 7}, {10, 9, 10, 10, 10}, {12, 12, 12, 12, 11}};
        
        respawnPos = new Vector2[5, 5]
        {{new Vector2(-161.5f, -7), new Vector2(-82, -2), new Vector2(-157.5f, 2), new Vector2(-196, 2), new Vector2(-157, 12)}
        , {new Vector2(-110.42f, -16.7f), new Vector2(-110.6f, -11), new Vector2(-20.53f, -12.1f), new Vector2(-127.3f, 1.9f), new Vector2(-125.7f, -3.2f)}
        , {new Vector2(-138.43f, -12.8f), new Vector2(-102.4f, -11.26f), new Vector2(-151.9f, 2.02f), new Vector2(-211.7f, 11.5f), new Vector2(-150.5f, -16.98f)}
        , {new Vector2(-175.52f, -14.53f), new Vector2(-245.3f, -3f), new Vector2(-189.18f, -3.97f), new Vector2(-133.82f, -9.03f), new Vector2(-197.4f, 17.91f)}
        , {new Vector2(-177.5f, -20f), new Vector2(-156.5f, -11f), new Vector2(-199f, -12f), new Vector2(-196f, 3.7f), new Vector2(-246f, 27.5f)}};

        respawnGravityDir = new GravityDirection[5, 5]
        {{GravityDirection.down, GravityDirection.left, GravityDirection.down, GravityDirection.down, GravityDirection.down}
        , {GravityDirection.down, GravityDirection.down, GravityDirection.down, GravityDirection.down, GravityDirection.down}
        , {GravityDirection.down, GravityDirection.down, GravityDirection.up, GravityDirection.left, GravityDirection.right}
        , {GravityDirection.right, GravityDirection.down, GravityDirection.down, GravityDirection.down, GravityDirection.down}
        , {GravityDirection.down, GravityDirection.down, GravityDirection.down, GravityDirection.down, GravityDirection.right}};

        respawnIsRoping = new bool[5, 5]
        {{false, false, false, false, false}
        , {true, false, false, false, false}
        , {true, false, false, false, false}
        , {true, false, false, false, false}
        , {true, false, false, false, false}};
    }

    void Update() {
        if (curStage == 4) {
            if (storedCurState != curState) {
                storedCurState = curState;
                for (int i = 0; i < 35; i++) {
                    storedIsShaked[i] = curIsShaked[i];
                }
            }
            if (isDie) {
                for (int i = 0; i < 35; i++) {
                    curIsShaked[i] = storedIsShaked[i];
                }
            }
        }
    }
}