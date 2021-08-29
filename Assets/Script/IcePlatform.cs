using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class IcePlatform : MonoBehaviour
{
    public Vector2 leftmostPos, rightmostPos;
    Vector2 startingPos, leftPos, rightPos, deltaPos;
    bool isStarting;
    bool isFinishLeft;
    bool isFinishRight;
    bool isFinishWaiting;
    Tilemap tilemap;
    List<GameObject> fireList;

    void Awake() {
        tilemap = GetComponent<Tilemap>();
        fireList = new List<GameObject>();
        if (Physics2D.gravity.x == 0) {
            deltaPos = new Vector2(0.5f, 0f);
        }
        else {
            deltaPos = new Vector2(0f, 0.5f);
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (!isStarting && other.CompareTag("Projectile")) {
            startingPos = other.transform.position;
            if (Physics2D.gravity.x == 0) {
                float fracPart = startingPos.x - (int)startingPos.x;
                float intPart = startingPos.x - fracPart;
                if (fracPart >= 0 && fracPart < 0.25) {
                    startingPos = new Vector2(intPart, leftmostPos.y - 0.5f * Physics2D.gravity.normalized.y);
                }
                else if (fracPart >= 0.25 && fracPart < 0.75) {
                    startingPos = new Vector2(intPart + 0.5f, leftmostPos.y - 0.5f * Physics2D.gravity.normalized.y);
                }
                else {
                    startingPos = new Vector2(intPart + 1.0f, leftmostPos.y - 0.5f * Physics2D.gravity.normalized.y);
                }
            }
            else {
                float fracPart = startingPos.y - (int)startingPos.y;
                float intPart = startingPos.y - fracPart;
                if (fracPart >= 0 && fracPart < 0.25) {
                    startingPos = new Vector2(leftmostPos.x - 0.5f * Physics2D.gravity.normalized.x, intPart);
                }
                else if (fracPart >= 0.25 && fracPart < 0.75) {
                    startingPos = new Vector2(leftmostPos.x - 0.5f * Physics2D.gravity.normalized.x, intPart + 0.5f);
                }
                else {
                    startingPos = new Vector2(leftmostPos.x - 0.5f * Physics2D.gravity.normalized.x, intPart + 1.0f);
                }
            }
            GameObject fire = GameManager.instance.fireQueue.Dequeue();
            fire.transform.position = startingPos;
            fire.SetActive(true);
            fireList.Add(fire);
            leftPos = startingPos - deltaPos;
            rightPos = startingPos + deltaPos;
            StartCoroutine("CreateFire");
            isStarting = true;
        }
    }

    IEnumerator CreateFire() {
        while (!isFinishLeft || !isFinishRight) {
            if (!isFinishLeft) {
                GameObject fire = GameManager.instance.fireQueue.Dequeue();
                fire.transform.position = leftPos;
                fire.SetActive(true);
                fireList.Add(fire);
                leftPos -= deltaPos;
            }
            if (!isFinishRight) {
                GameObject fire = GameManager.instance.fireQueue.Dequeue();
                fire.transform.position = rightPos;
                fire.SetActive(true);
                fireList.Add(fire);
                rightPos += deltaPos;
            }
            if (Physics.gravity.x == 0) {
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
            yield return new WaitForSeconds(0.1f);
        }
        StartCoroutine(Melt());
    }

    IEnumerator Melt() {
        while (tilemap.color.a > 0f) {
            Color color = tilemap.color;
            if (color.a > 0f) {
                color.a -= 0.025f;
                tilemap.color = color;
            }
            yield return new WaitForSeconds(0.1f);
        }
        // after melting
        for (int i = 0; i < fireList.Count; i++) {
            fireList[i].GetComponent<ParticleSystem>().Stop();
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