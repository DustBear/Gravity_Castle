using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Experimental.Rendering.Universal;

public class TurnOnLight : MonoBehaviour
{
    [SerializeField] Light2D globalLight;
    [SerializeField] Light2D playerLight;

    void Start()
    {
        if (GameManager.instance.curAchievementNum >= 29)
        {
            globalLight.intensity = 1f;
            playerLight.intensity = 0f;
            Devil devil = FindObjectOfType<Devil>();
            if (devil != null)
            {
                devil.enabled = true;
            }
            Destroy(gameObject);
        }
    }

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
        GameManager.instance.curAchievementNum = 29;
        GameManager.instance.respawnScene = SceneManager.GetActiveScene().buildIndex;
        GameManager.instance.respawnPos = transform.position;
        GameManager.instance.respawnGravityDir = Physics2D.gravity.normalized;
        Devil devil = FindObjectOfType<Devil>();
        if (devil != null)
        {
            devil.enabled = true;
        }
        Destroy(gameObject);
    }
}
