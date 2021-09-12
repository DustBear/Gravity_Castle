using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class IcePlatform : MonoBehaviour
{
    public bool isVertical;
    public Vector2 leftmostPos, rightmostPos;
    Vector2 leftPos, rightPos, deltaPos;
    bool isStarting;
    bool isFinishLeft;
    bool isFinishRight;
    bool isFinishWaiting;
    Tilemap tilemap;
    List<GameObject> fireList;
    List<SpriteRenderer> leverRenderer;
    List<Vector2> leverPos;

    void Awake() {
        tilemap = GetComponent<Tilemap>();
        fireList = new List<GameObject>();
        if (!isVertical) {
            deltaPos = new Vector2(0.5f, 0f);
        }
        else {
            deltaPos = new Vector2(0f, 0.5f);
        }
        
        if (transform.childCount != 0) {
            leverRenderer = new List<SpriteRenderer>();
            leverPos = new List<Vector2>();
            for (int i = 0; i < transform.childCount; i++) {
                GameObject lever = transform.GetChild(i).gameObject;
                leverRenderer.Add(lever.GetComponent<SpriteRenderer>());
                leverPos.Add(lever.transform.position);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D other) {
        if (!isStarting && other.gameObject.CompareTag("Projectile")) {
            StartFired(other.transform.position);
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (!isStarting && other.CompareTag("Projectile")) {
            StartFired(other.transform.position);
        }
    }

    void StartFired(Vector2 startingPos) {
        if (!isVertical) {
                float fracPart = startingPos.x - (int)startingPos.x;
                float intPart = startingPos.x - fracPart;
                if (fracPart >= 0 && fracPart < 0.25) {
                    startingPos = new Vector2(intPart, leftmostPos.y);
                }
                else if (fracPart >= 0.25 && fracPart < 0.75) {
                    startingPos = new Vector2(intPart + 0.5f, leftmostPos.y);
                }
                else {
                    startingPos = new Vector2(intPart + 1.0f, leftmostPos.y);
                }

                if (startingPos.x < leftmostPos.x) {
                    startingPos.x = leftmostPos.x;
                }
                if (startingPos.x > rightmostPos.x) {
                    startingPos.x = rightmostPos.x;
                }
            }
            else {
                float fracPart = startingPos.y - (int)startingPos.y;
                float intPart = startingPos.y - fracPart;
                if (fracPart >= 0 && fracPart < 0.25) {
                    startingPos = new Vector2(leftmostPos.x, intPart);
                }
                else if (fracPart >= 0.25 && fracPart < 0.75) {
                    startingPos = new Vector2(leftmostPos.x, intPart + 0.5f);
                }
                else {
                    startingPos = new Vector2(leftmostPos.x, intPart + 1.0f);
                }

                if (startingPos.y < leftmostPos.y) {
                    startingPos.y = leftmostPos.y;
                }
                if (startingPos.y > rightmostPos.y) {
                    startingPos.y = rightmostPos.y;
                }
            }
            GameObject fire = GameManager.instance.fireQueue.Dequeue();
            fire.transform.position = startingPos;
            fire.SetActive(true);
            fireList.Add(fire);
            for (int i = 0; i < transform.childCount; i++) {
                if (startingPos.x == leverPos[i].x || startingPos.y == leverPos[i].y) {
                    fire = GameManager.instance.fireQueue.Dequeue();
                    fire.transform.position = leverPos[i];
                    fire.SetActive(true);
                    fireList.Add(fire);
                }
            }
            leftPos = startingPos - deltaPos;
            rightPos = startingPos + deltaPos;
            StartCoroutine("CreateFire");
            isStarting = true;
    }

    IEnumerator CreateFire() {
        while (!isFinishLeft || !isFinishRight) {
            if (!isVertical) {
                if (leftPos.x < leftmostPos.x) {
                    isFinishLeft = true;
                }
                if (rightPos.x > rightmostPos.x) {
                    isFinishRight = true;
                }
            }
            else {
                if (leftPos.y < leftmostPos.y) {
                    isFinishLeft = true;
                }
                if (rightPos.y > rightmostPos.y) {
                    isFinishRight = true;
                }
            }

            if (!isFinishLeft) {
                GameObject fire = GameManager.instance.fireQueue.Dequeue();
                fire.transform.position = leftPos;
                fire.SetActive(true);
                fireList.Add(fire);
                leftPos -= deltaPos;
                for (int i = 0; i < transform.childCount; i++) {
                    if (leftPos.x == leverPos[i].x || leftPos.y == leverPos[i].y) {
                        fire = GameManager.instance.fireQueue.Dequeue();
                        fire.transform.position = leverPos[i];
                        fire.SetActive(true);
                        fireList.Add(fire);
                    }
                }
            }
            if (!isFinishRight) {
                GameObject fire = GameManager.instance.fireQueue.Dequeue();
                fire.transform.position = rightPos;
                fire.SetActive(true);
                fireList.Add(fire);
                rightPos += deltaPos;
                for (int i = 0; i < transform.childCount; i++) {
                    if (leverRenderer != null && (rightPos.x == leverPos[i].x || rightPos.y == leverPos[i].y)) {
                        fire = GameManager.instance.fireQueue.Dequeue();
                        fire.transform.position = leverPos[i];
                        fire.SetActive(true);
                        fireList.Add(fire);
                }
                }
            }
            yield return new WaitForSeconds(0.1f);
        }
        StartCoroutine(Melt());
    }

    IEnumerator Melt() {
        while (tilemap.color.a > 0f) {
            Color tileColor = tilemap.color;
            if (tileColor.a > 0f) {
                tileColor.a -= 0.025f;
                tilemap.color = tileColor;
            }
            for (int i = 0; i < transform.childCount; i++) {
                Color leverColor = leverRenderer[i].color;
                leverColor.a -= 0.025f;
                leverRenderer[i].color = leverColor;
            }
            yield return new WaitForSeconds(0.1f);
        }
        // after melting
        for (int i = 0; i < fireList.Count; i++) {
            fireList[i].GetComponent<FireNotFalling>().isIceMelted = true;
        }
        StartCoroutine(Wait());
    }

    IEnumerator Wait() {
        yield return new WaitForSeconds(1.0f);
        for (int i = 0; i < fireList.Count; i++) {
            fireList[i].SetActive(false);
            GameManager.instance.fireQueue.Enqueue(fireList[i]);
        }
        Destroy(gameObject);
    }
}