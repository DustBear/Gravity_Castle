using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stage03_lantonLight : MonoBehaviour
{
    public GameObject backLight;
    public GameObject objectLight;

    [SerializeField] float dimSpeed; //���� ��ο����� ������� �ӵ�
    [SerializeField] float dimSize; //���� ��ο����� ������� ���� 
    [SerializeField] float blinkPeriod; //���� �����Ÿ��� ���� 
    [SerializeField] float randomValue; //��� ������ ���� �ֱ�� �����Ÿ��� �ȵ�

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
        //��� ������ ������ �ֱ�� ����� �����ϸ� �� �ǹǷ� ���������� ��
    }

    // Update is called once per frame
    void Update()
    {
        Dim(); //���� ������� ��ο����� �� 
        Timer(); //Ÿ�̸� �Լ��� ���� �ұ�Ģ������ �����δ�
    }

    void Dim() //���� ��ο����� ������� ��
    {
        backLight.GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>().intensity = sinCalculate(iniBackLight);
        objectLight.GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>().intensity = sinCalculate(iniObjectLight);
    }

    float sinCalculate(float defaultValue)
    {
        float output = defaultValue + dimSize * Mathf.Sin(randomValue + Time.time * dimSpeed);
        return output;
    }
    IEnumerator blink() //���� ������ 
    {
        int blinkCount = Random.Range(1, 3); //�����̴� Ƚ���� 1 �Ǵ� 2 

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
        curTime = curTime + Time.deltaTime; //Ÿ�̸� �۵� 
        
        if(curTime >= nextDelay)
        {
            StartCoroutine("blink");
            curTime = 0;
            nextDelay = Random.Range(minBlinkDelay, maxBlinkDelay);
        }
    }
}
