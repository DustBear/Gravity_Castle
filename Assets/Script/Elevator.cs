using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    public Vector2 startingPos;
    public Vector2 finishingPos;
    public float speed;
    public float waitingTime;
    
    Player player;
    bool isAllow1; // allow to go to the finishingPos
    bool isAllow2; // allow to go to the startingPos

    void Awake() {
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
    }

    void Update()
    {
        if (player.onMovingFloor) {
            // wait before going to the finishingPos
            if (!isAllow1) {
                StartCoroutine("Wait1");
            }
            // go to the finishingPos
            else {
                transform.position = Vector2.Lerp(transform.position, finishingPos, speed * Time.deltaTime);
            }
        }
        else {
            // wait before going to the startingPos
            if (!isAllow2) {
                StartCoroutine("Wait2");
            }
            // go to the startingPos
            else {
                transform.position = Vector2.Lerp(transform.position, startingPos, speed * Time.deltaTime);
            }
        }
    }

    // after waiting, allow to go to the finishingPos
    IEnumerator Wait1() {
        yield return new WaitForSeconds(waitingTime);
        isAllow1 = true;
        isAllow2 = false;
    }

    // after waiting, allow to go to the startingPos
    IEnumerator Wait2() {
        yield return new WaitForSeconds(waitingTime);
        isAllow1 = false;
        isAllow2 = true;
    }
}
