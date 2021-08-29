using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LatingFireLauncher : MonoBehaviour
{
    public GameObject fire;
    public Vector2 leftPos, rightPos;
    public Vector2 finalPos;
    
    void Start() {
        for (float f = leftPos.x; f <= rightPos.x; f += 0.5f) {
            GameObject newFire = Instantiate(fire, new Vector2(f, transform.position.y + 0.5f), Quaternion.Euler(0f, 0f, 0f));
            newFire.transform.parent = gameObject.transform;
            newFire.transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }

    void Update() {
        transform.position = Vector2.MoveTowards(transform.position, finalPos, Time.deltaTime * 0.2f);
    }
}
