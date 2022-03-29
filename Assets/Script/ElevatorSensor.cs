using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorSensor : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] Transform elevatorDoor;
    [SerializeField] Animator elevatorDoorAnim;
    [SerializeField] Vector2 openedPos;
    [SerializeField] Vector2 closedPos;
    bool isStarted;
    IEnumerator coroutine1;
    IEnumerator coroutine2;

    void Awake()
    {
        coroutine1 = _MoveElevatorDoor(true);
        coroutine2 = _MoveElevatorDoor(false);
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (!isStarted && other.CompareTag("Player") && transform.eulerAngles.z == player.eulerAngles.z)
        {
            StopCoroutine(coroutine1);
            StartCoroutine(coroutine2);
            isStarted = true;
        }
    }

    void OnCollisionEnter2D(Collision2D other) {
        if (!isStarted && other.gameObject.CompareTag("Player") && transform.eulerAngles.z == player.eulerAngles.z)
        {
            StopCoroutine(coroutine2);
            StartCoroutine(coroutine1);
            transform.position -= transform.up * 0.5f;
            isStarted = true;
        }
    }

    IEnumerator _MoveElevatorDoor(bool isOpened)
    {
        if (isOpened)
        {
            elevatorDoorAnim.SetBool("isOpened", true);
            yield return new WaitForSeconds(1f);
        }
        Vector2 targetPos = isOpened ? openedPos : closedPos;
        while ((Vector2)elevatorDoor.position != targetPos)
        {
            elevatorDoor.position = Vector2.MoveTowards(elevatorDoor.position, targetPos, 5f * Time.deltaTime);
            yield return null;
        }
        if (!isOpened)
        {
            elevatorDoorAnim.SetBool("isOpened", false);
        }
    }

    public void MoveElevatorDoor(bool isOpened)
    {
        if (!isOpened)
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
