using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Test : MonoBehaviour
{
    Tilemap tilemap;

    void Awake()
    {
        tilemap = GetComponent<Tilemap>();
    }
    void Start()
    {
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        while (tilemap.color.a > 0f)
        {
            Color color = tilemap.color;
            color.a -= 0.1f;
            tilemap.color = color;
            yield return new WaitForSeconds(0.1f);
        }
    }
}
