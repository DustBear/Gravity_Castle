using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class helpWindowButton : MonoBehaviour
{
    [SerializeField] GameObject helpWindow;

    private void Start()
    {
        helpWindow.SetActive(false);
    }

    private void OnEnable()
    {
        helpWindow.SetActive(false);
    }
    void Update()
    {
        
    }

    public void onClick()
    {
        helpWindow.SetActive(!helpWindow.activeSelf);
        //ƒ—¡Æ ¿÷¿∏∏È ≤Ù∞Ì ≤®¡Æ ¿÷¿∏∏È ≈¥ 
    }

    private void OnDisable()
    {
        helpWindow.SetActive(false);
    }
}
