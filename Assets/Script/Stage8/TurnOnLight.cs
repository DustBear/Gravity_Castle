using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class TurnOnLight : MonoBehaviour
{
    [SerializeField] Light2D globalLight;
    [SerializeField] Light2D playerLight;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other != null && other.CompareTag("Player"))
        {
            StartCoroutine(StartTurnOnLight());
        }
    }

    IEnumerator StartTurnOnLight()
    {
        while (globalLight.intensity < 1f)
        {
            globalLight.intensity += 0.02f;
            playerLight.intensity -= 0.02f;
            yield return new WaitForSeconds(0.05f);
        }
        Destroy(gameObject);
    }
}
