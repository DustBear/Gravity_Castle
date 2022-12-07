using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class introCanvas : MonoBehaviour
{
    [SerializeField] int stageNum; //이 스테이지의 번호 
    public float timeOffset; //스테이지 시작 후 몇 초 뒤 활성화되는지 
    public float fadeTime; //밝아지고 어두워지는데 각각 몇 초가 걸리는지
    public float activeTime; //완전히 밝아진 후 몇 초나 활성화된 상태로 있는지 
   
    public Image _introDeco;
    public Text _introText;

    AudioSource sound;

    private void Awake()
    {
        sound = GetComponent<AudioSource>();
    }
    void Start()
    {
        _introText.text = gameTextManager.instance.stageNameManager(stageNum);
        _introDeco.color = new Color(1, 1, 1, 0);
        _introText.color = new Color(1, 1, 1, 0);

        if (GameManager.instance.gameData.curAchievementNum != 0)
        {
            gameObject.SetActive(false); //introCanvas는 스테이지를 맨 처음 시작할 때만 띄워줌 
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
            _introDeco.color = new Color(1, 1, 1, (float)index/frameCount);
            _introText.color = new Color(1, 1, 1, (float)index / frameCount);
            yield return new WaitForSeconds(fadeTime / frameCount);
        }

        yield return new WaitForSeconds(activeTime);

        for (int index = frameCount; index >= 0; index--)
        {
            _introDeco.color = new Color(1, 1, 1, (float)index / frameCount);
            _introText.color = new Color(1, 1, 1, (float)index / frameCount);
            yield return new WaitForSeconds(fadeTime / frameCount);
        }
    }
}
