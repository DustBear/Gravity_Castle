using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class introCanvas : MonoBehaviour
{
    public float timeOffset; //�������� ���� �� �� �� �� Ȱ��ȭ�Ǵ��� 
    public float fadeTime; //������� ��ο����µ� ���� �� �ʰ� �ɸ�����
    public float activeTime; //������ ����� �� �� �ʳ� Ȱ��ȭ�� ���·� �ִ��� 
   
    public Image _blackBackground;
    public Image _introDeco;
    public Text _introText;

    float initAlpha = (float)200/255; //�ʱ� background image�� ���İ� 

    AudioSource sound;
  
    void Start()
    {
        _blackBackground.color = new Color(0, 0, 0, 0);
        _introDeco.color = new Color(1, 1, 1, 0);
        _introText.color = new Color(1, 1, 1, 0);

        if (GameManager.instance.gameData.curAchievementNum != 0)
        {
            gameObject.SetActive(false); //introCanvas�� ���������� �� ó�� ������ ���� ����� 
        }
        else
        {
            StartCoroutine(intro());
        }                
    }

    void Update()
    {
        
    }

    int frameCount = 50;
    IEnumerator intro()
    {
        yield return new WaitForSeconds(timeOffset);

        sound.Play();
        for(int index=0; index<=frameCount; index++)
        {
            _blackBackground.color = new Color(0, 0, 0, (float)index / frameCount * initAlpha);
            _introDeco.color = new Color(1, 1, 1, (float)index/frameCount);
            _introText.color = new Color(1, 1, 1, (float)index / frameCount);
            yield return new WaitForSeconds(fadeTime / frameCount);
        }

        yield return new WaitForSeconds(activeTime);

        for (int index = frameCount; index >= 0; index--)
        {
            _blackBackground.color = new Color(0, 0, 0, (float)index / frameCount * initAlpha);
            _introDeco.color = new Color(1, 1, 1, (float)index / frameCount);
            _introText.color = new Color(1, 1, 1, (float)index / frameCount);
            yield return new WaitForSeconds(fadeTime / frameCount);
        }
    }
}
