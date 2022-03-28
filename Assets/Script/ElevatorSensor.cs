using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorSensor : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] Transform elevatorDoor;
    [SerializeField] int sensorNum;
    Vector2 originPos;
    Vector2 finalPos;
    bool isStarted;
    IEnumerator coroutine1;
    IEnumerator coroutine2;

    void Awake()
    {
        originPos = elevatorDoor.position;
        finalPos = originPos + (Vector2)elevatorDoor.up * 5f;
        coroutine1 = _MoveElevatorDoor(true);
        coroutine2 = _MoveElevatorDoor(false);
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (!isStarted && other.CompareTag("Player") && transform.eulerAngles.z == player.eulerAngles.z)
        {
            if (sensorNum == 1)
            {
                StopCoroutine(coroutine2);
                StartCoroutine(coroutine1);
            }
            else
            {
                StopCoroutine(coroutine1);
                StartCoroutine(coroutine2);
            }
            isStarted = true;
        }
    }

    void OnCollisionEnter2D(Collision2D other) {
        if (!isStarted && other.gameObject.CompareTag("Player") && transform.eulerAngles.z == player.eulerAngles.z)
        {
            if (sensorNum == 1)
            {
                StopCoroutine(coroutine2);
                StartCoroutine(coroutine1);
            }
            else
            {
                StopCoroutine(coroutine1);
                StartCoroutine(coroutine2);
            }
            transform.position -= transform.up * 0.5f;
            isStarted = true;
        }
    }

    IEnumerator _MoveElevatorDoor(bool isGoingFinalPos)
    {
        Vector2 targetPos = isGoingFinalPos ? finalPos : originPos;
        while ((Vector2)elevatorDoor.position != targetPos)
        {
            elevatorDoor.position = Vector2.MoveTowards(elevatorDoor.position, targetPos, 5f * Time.deltaTime);
            yield return null;
        }
    }

    public void MoveElevatorDoor(bool isGoingFinalPos)
    {
        if (!isGoingFinalPos)
        {
            StopCoroutine(coroutine1);
            StartCoroutine(coroutine2);
        }
        else
        {
            StopCoroutine(coroutine2);
            StartCoroutine(coroutine1);
        }
    }
}
