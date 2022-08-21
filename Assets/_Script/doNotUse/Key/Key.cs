using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Key : MonoBehaviour
{
    [SerializeField] int stageNum;
    public int achievementNum;

    public Vector2 respawnPos; //플레이어가 키를 먹고 나서 리스폰되는 위치    
    public Vector2 respawnDir; //플레이어가 키를 먹고 나서 리스폰되는 중력방향

    Transform player;
    SpriteRenderer sprite;

    [SerializeField] float wavePeriod;
    [SerializeField] float waveSize;

    public ParticleSystem rightBurst;
    public ParticleSystem leftBurst;
    public GameObject keyLight; //열쇠가 사라지기 전에 빛이 서서히 꺼지게 만듦
    public GameObject keyParticles;

    float initialXPos;
    float initialYPos;

    AudioSource sound;
    void Awake()
    {
        player = GameObject.FindWithTag("Player").transform;
        sprite = GetComponent<SpriteRenderer>();
        sound = GetComponent<AudioSource>();

        if (GameManager.instance.gameData.curAchievementNum >= achievementNum) //이미 먹은 열쇠라면 제거해야 함 
        {
            Destroy(gameObject);
        }

        respawnPos = transform.position - transform.up; //key는 keystone보다 조금 위에 있으므로 좌표에서 1 뺀 위치에 플레이어 소환 
    }

    void Start()
    {
        initialXPos = transform.position.x;
        initialYPos = transform.position.y;
    }

    private void FixedUpdate()
    {
        sinMove(); //열쇠는 가만히 있을 때 위 아래로 흔들려야 함
    }

    void sinMove()
    {
        float delta;
        delta = waveSize * Mathf.Sin(Time.time / wavePeriod);
        
        if(transform.rotation == Quaternion.Euler(0,0,0) || transform.rotation == Quaternion.Euler(0, 0, 180f))
        {
            transform.position = new Vector3(transform.position.x, initialYPos + delta, 0);
        }
        else
        {
            transform.position = new Vector3(initialXPos + delta, transform.position.y, 0);
        }
    }

    public IEnumerator burst()
    {
        GameManager.instance.SaveData(achievementNum, stageNum, player.position);
        Debug.Log("savePointBackUp: " + achievementNum);

        rightBurst.Play();
        leftBurst.Play();
        sound.Play();

        sprite.color = new Color(1, 1, 1, 0); //잠시 투명하게 만들고 
        StartCoroutine("lightFade");

        var emit = keyParticles.GetComponent<ParticleSystem>().emission;
        emit.rateOverTime = 0; 
        //파티클의 생존시간은 2초: 열쇠가 사라지는 즉시 emit을 0으로 만들면 2초 뒤 모든 파티클이 사라지는 동시에 파티클시스템은 비활성화됨

        yield return new WaitForSeconds(2f);
        Destroy(gameObject); //2초 뒤 파티클시스템 전부 끝나면 비활성화 
    }  
    
    IEnumerator lightFade()
    {
        for(int index=20; index>=1; index--)
        {
            float colorValue = 0.05f * index;

            keyLight.GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>().color = new Color(colorValue, colorValue, colorValue);
            yield return new WaitForSeconds(0.05f); //1초 뒤 파티클 완전히 투명화 됨
        }
    }
}
