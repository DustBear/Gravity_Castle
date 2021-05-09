using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHome : MonoBehaviour
{
    public GameObject prefab;
    public float timeInterval;
    float nextTime;

    void FixedUpdate() {
        if (Time.time > nextTime) {
            Instantiate(prefab, new Vector3(transform.position.x - 0.5f, transform.position.y, transform.position.z), Quaternion.identity);
            nextTime = Time.time + timeInterval;
        }
    }
}
