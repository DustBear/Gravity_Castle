using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Key : MonoBehaviour
{
    [SerializeField] int stageNum;
    public int achievementNum;

    public Vector2 respawnPos; //�÷��̾ Ű�� �԰� ���� �������Ǵ� ��ġ    
    public Vector2 respawnDir; //�÷��̾ Ű�� �԰� ���� �������Ǵ� �߷¹���

    Transform player;
    SpriteRenderer sprite;

    [SerializeField] float wavePeriod;
    [SerializeField] float waveSize;

    public ParticleSystem rightBurst;
    public ParticleSystem leftBurst;
    public GameObject keyLight; //���谡 ������� ���� ���� ������ ������ ����
    public GameObject keyParticles;

    float initialXPos;
    float initialYPos;

    AudioSource sound;
    void Awake()
    {
        player = GameObject.FindWithTag("Player").transform;
        sprite = GetComponent<SpriteRenderer>();
        sound = GetComponent<AudioSource>();

        if (GameManager.instance.gameData.curAchievementNum >= achievementNum) //�̹� ���� ������ �����ؾ� �� 
        {
            Destroy(gameObject);
        }

        respawnPos = transform.position - transform.up; //key�� keystone���� ���� ���� �����Ƿ� ��ǥ���� 1 �� ��ġ�� �÷��̾� ��ȯ 
    }

    void Start()
    {
        initialXPos = transform.position.x;
        initialYPos = transform.position.y;
    }

    private void FixedUpdate()
    {
        sinMove(); //����� ������ ���� �� �� �Ʒ��� ������ ��
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

        sprite.color = new Color(1, 1, 1, 0); //��� �����ϰ� ����� 
        StartCoroutine("lightFade");

        var emit = keyParticles.GetComponent<ParticleSystem>().emission;
        emit.rateOverTime = 0; 
        //��ƼŬ�� �����ð��� 2��: ���谡 ������� ��� emit�� 0���� ����� 2�� �� ��� ��ƼŬ�� ������� ���ÿ� ��ƼŬ�ý����� ��Ȱ��ȭ��

        yield return new WaitForSeconds(2f);
        Destroy(gameObject); //2�� �� ��ƼŬ�ý��� ���� ������ ��Ȱ��ȭ 
    }  
    
    IEnumerator lightFade()
    {
        for(int index=20; index>=1; index--)
        {
            float colorValue = 0.05f * index;

            keyLight.GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>().color = new Color(colorValue, colorValue, colorValue);
            yield return new WaitForSeconds(0.05f); //1�� �� ��ƼŬ ������ ����ȭ ��
        }
    }
}
