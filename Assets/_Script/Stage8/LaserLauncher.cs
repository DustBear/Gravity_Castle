using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserLauncher : MonoBehaviour
{
    [SerializeField] GameObject devil;
    [SerializeField] GameObject laser;

    void Start()
    {
        StartCoroutine(StartLaunch());
    }

    IEnumerator StartLaunch()
    {
        while (GameManager.instance.gameData.curAchievementNum <= 30)
        {
            yield return new WaitForSeconds(3f);
        }
        if (GameManager.instance.gameData.curAchievementNum <= 32)
        {
            Launch();
        }
    }

    public void Launch()
    {
        laser.transform.position = devil.transform.position;
        laser.SetActive(true);
    }
}
