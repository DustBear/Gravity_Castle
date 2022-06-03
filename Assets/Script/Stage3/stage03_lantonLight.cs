using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stage03_lantonLight : MonoBehaviour
{
    public GameObject backLight;
    public GameObject objectLight;

    [SerializeField] float dimSpeed; //불이 어두워졌다 밝아지는 속도
    [SerializeField] float dimSize; //불이 어두워졌다 밝아지는 정도 
    [SerializeField] float blinkPeriod; //불이 깜빡거리는 간격 
    [SerializeField] float randomValue; //모든 전등이 같은 주기로 깜빡거리면 안됨

    [SerializeField] float maxBlinkDelay;
    [SerializeField] float minBlinkDelay;

    [SerializeField] float curTime;
    [SerializeField] float nextDelay;

    float iniBackLight;
    float iniObjectLight;
    void Start()
    {
        backLight.SetActive(true);
        objectLight.SetActive(true);
        curTime = 0;

        nextDelay = Random.Range(minBlinkDelay, maxBlinkDelay);

        iniBackLight = backLight.GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>().intensity;
        iniObjectLight = objectLight.GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>().intensity;

        randomValue = Random.Range(0, 1 / dimSpeed);
        dimSpeed = Random.Range(dimSpeed*0.8f, dimSpeed*1.2f);
        dimSize = Random.Range(dimSize * 0.8f, dimSize * 0.8f); 
        //모든 전등이 동일한 주기와 세기로 점등하면 안 되므로 오차범위를 둠
    }

    // Update is called once per frame
    void Update()
    {
        Dim(); //불이 밝아졌다 어두워졌다 함 
        Timer(); //타이머 함수로 불이 불규칙적으로 깜빡인다
    }

    void Dim() //불이 어두워졌다 밝아졌다 함
    {
        backLight.GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>().intensity = sinCalculate(iniBackLight);
        objectLight.GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>().intensity = sinCalculate(iniObjectLight);
    }

    float sinCalculate(float defaultValue)
    {
        float output = defaultValue + dimSize * Mathf.Sin(randomValue + Time.time * dimSpeed);
        return output;
    }
    IEnumerator blink() //불이 깜빡임 
    {
        int blinkCount = Random.Range(1, 3); //깜빡이는 횟수는 1 또는 2 

        for(int num=1; num<=blinkCount; num++)
        {
            backLight.SetActive(false);
            objectLight.SetActive(false);

            yield return new WaitForSeconds(blinkPeriod);

            backLight.SetActive(true);
            objectLight.SetActive(true);

            yield return new WaitForSeconds(blinkPeriod);
        }      
    }

    void Timer()
    {
        curTime = curTime + Time.deltaTime; //타이머 작동 
        
        if(curTime >= nextDelay)
        {
            StartCoroutine("blink");
            curTime = 0;
            nextDelay = Random.Range(minBlinkDelay, maxBlinkDelay);
        }
    }
}
