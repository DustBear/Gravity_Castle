using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserLauncher : MonoBehaviour
{
    [SerializeField] GameObject devil;
    [SerializeField] GameObject laser;

    void Start()
    {
        Launch();
    }

    public void Launch()
    {
        laser.transform.position = devil.transform.position;
        laser.SetActive(true);
    }
}
