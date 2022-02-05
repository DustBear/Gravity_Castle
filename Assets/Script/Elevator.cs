using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    [SerializeField] Vector2 startingPos;
    [SerializeField] Vector2 finishingPos;
    [SerializeField] float speed;
    [SerializeField] float waitingTime;
    [SerializeField] Transform player;
    Transform elevator;
    IEnumerator coroutine;

    void Awake() {
        elevator = transform.parent;
        coroutine = Return();
    }

    void OnTriggerEnter2D(Collider2D other) {
        // When player takes the elevator, elevator moves to the finishingPos
        if (other.CompareTag("Player") && (Vector2)transform.up == -Physics2D.gravity.normalized)
        {
            player.parent = elevator;
            StopCoroutine(coroutine);
            coroutine = Go();
            StartCoroutine(coroutine);
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        // When player gets off the elevator, elevator moves to the startingPos
        if (other.CompareTag("Player"))
        {
            player.parent = null;
            StopCoroutine(coroutine);
            coroutine = Return();
            StartCoroutine(coroutine);
        }
    }

    IEnumerator Go() {
        yield return new WaitForSeconds(waitingTime);
        while (Vector2.Distance(elevator.position, finishingPos) > 0.1f)
        {
            elevator.position = Vector2.Lerp(elevator.position, finishingPos, speed * Time.deltaTime);
            yield return null;
        }
        elevator.position = finishingPos;
    }

    IEnumerator Return() {
        yield return new WaitForSeconds(waitingTime);
        while (Vector2.Distance(elevator.position, startingPos) > 0.1f)
        {
            elevator.position = Vector2.Lerp(elevator.position, startingPos, speed * Time.deltaTime);
            yield return null;
        }
        elevator.position = startingPos;
    }
}
