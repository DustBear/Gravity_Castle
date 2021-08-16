using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Object Pool
    public enum Obj {arrow, cannon};
    GameObject[] arrowPool;
    GameObject[] cannonPool;
    public GameObject arrow;
    public GameObject cannon;
    public int arrowNum;
    public int cannonNum;

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
    public bool nextIsJumping;
    public bool nextIsRoping;

    // If die
    public int[,] respawnScene;
    public Vector2[,] respawnPos;
    public GravityDirection[,] respawnGravityDir;
    public bool[,] respawnIsRoping;

    void Awake() {
        DontDestroyOnLoad(gameObject);
        
        // Object Pool
        arrowPool = new GameObject[arrowNum];
        cannonPool = new GameObject[cannonNum];
        for (int i = 0; i < arrowNum; i++) {
            arrowPool[i] = Instantiate(arrow);
            DontDestroyOnLoad(arrowPool[i]);
            arrowPool[i].SetActive(false);
        }
        for (int i = 0; i < cannonNum; i++) {
            cannonPool[i] = Instantiate(cannon);
            DontDestroyOnLoad(cannonPool[i]);
            cannonPool[i].SetActive(false);
        }

        // Next scene
        nextPos = new Vector2(-161.4f, -7f);
        //nextPos = new Vector2(-174.7f, -12.5f);
        nextGravityDir = GravityDirection.down;

        // Die
        respawnScene = new int[4, 5]
        {{1, 2, 1, 3, 1}, {4, 5, 4, 4, 4}, {7, 7, 7, 8, 7}, {10, 9, 10, 10, 10}};
        
        respawnPos = new Vector2[4, 5]
        {{new Vector2(-161.5f, -7), new Vector2(-82, -2), new Vector2(-157.5f, 2), new Vector2(-196, 2), new Vector2(-157, 12)}
        , {new Vector2(-110.42f, -16.7f), new Vector2(-110.6f, -11), new Vector2(-20.53f, -12.1f), new Vector2(-127.3f, 1.9f), new Vector2(-125.7f, -3.2f)}
        , {new Vector2(-138.43f, -12.8f), new Vector2(-102.4f, -11.26f), new Vector2(-151.9f, 2.02f), new Vector2(-211.7f, 11.5f), new Vector2(-150.5f, -16.98f)}
        , {new Vector2(-175.52f, -14.53f), new Vector2(-245.3f, -3f), new Vector2(-189.18f, -3.97f), new Vector2(-133.82f, -9.03f), new Vector2(-197.4f, 17.91f)}};

        respawnGravityDir = new GravityDirection[4, 5]
        {{GravityDirection.down, GravityDirection.left, GravityDirection.down, GravityDirection.down, GravityDirection.down}
        , {GravityDirection.down, GravityDirection.down, GravityDirection.down, GravityDirection.down, GravityDirection.down}
        , {GravityDirection.down, GravityDirection.down, GravityDirection.up, GravityDirection.left, GravityDirection.right}
        , {GravityDirection.right, GravityDirection.down, GravityDirection.down, GravityDirection.down, GravityDirection.down}};;

        respawnIsRoping = new bool[4, 5]
        {{false, false, false, false, false}
        , {true, false, false, false, false}
        , {true, false, false, false, false}
        , {true, false, false, false, false}};
    }

    public GameObject MakeObj(Obj obj) {
        switch (obj) {
            case Obj.arrow:
                for (int i = 0; i < arrowNum; i++) {
                    if (!arrowPool[i].activeSelf) {
                        return arrowPool[i];
                    }
                }
                break;
            case Obj.cannon:
                for (int i = 0; i < cannonNum; i++) {
                    if (!cannonPool[i].activeSelf) {
                        return cannonPool[i];
                    }
                }
                break;
        }
        return null;
    }
}