using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class openingCutSceneImage : MonoBehaviour
{
    public GameObject nextObj;
    public float delay;
    void Start()
    {
        StartCoroutine(imageShow());
    }

    
    void Update()
    {
        
    }

    IEnumerator imageShow()
    {
        yield return new WaitForSeconds(delay);
        nextObj.SetActive(true);

    }
}
