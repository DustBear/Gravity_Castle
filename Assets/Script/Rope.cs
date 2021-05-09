using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
    BoxCollider2D boxCollider;
    public GameObject prefab;
    public int length;
    public float interval;
    public bool isVertical;

    void Awake() {
        boxCollider = GetComponent<BoxCollider2D>();
    }
    void Start()
    {
        if (isVertical) {
            for (int i = 0; i < length; i++) {
                Instantiate(prefab, new Vector3(transform.position.x, transform.position.y - interval * i, transform.position.z), Quaternion.identity, transform);
            }
            boxCollider.size = new Vector2(0.5f, 0.5f * length + 0.25f);
            boxCollider.offset = new Vector2(0, -boxCollider.size.y / 2 + 0.25f);
        }
        else {
            for (int i = 0; i < length; i++) {
                Instantiate(prefab, new Vector3(transform.position.x - interval * i, transform.position.y, transform.position.z), Quaternion.Euler(0, 0, -90), transform);
            }
            boxCollider.size = new Vector2(0.5f, 0.5f * length + 0.25f);
            boxCollider.offset = new Vector2(0, -boxCollider.size.y / 2 + 0.25f);
        }
    }
}
