using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class collecting : MonoBehaviour
{
    GameObject cameraObj;
    MainCamera cameraScript;
    public ParticleSystem part;
    public ParticleSystem idleParticle;

    public Sprite[] collectionSprite;
    SpriteRenderer spr;

    bool isParticlePlayed;

    public AudioSource sound;
    public AudioSource loopSound;

    public AudioClip collect_normal;
    public AudioClip collect_abnormal;
    public AudioClip ambience;

    [SerializeField] int stageNum; //������ �� ��° ������������ 
    [SerializeField] int collectionNum; //�� ��° �����������

    public int colNumCal;

    float timer = 0f;
   
    private void Awake()
    {
        cameraObj = GameObject.FindWithTag("MainCamera");
        cameraScript = cameraObj.GetComponent<MainCamera>();
        colNumCal = GameManager.instance.collectionNumCalculate(new Vector2(stageNum, collectionNum));
        spr = GetComponent<SpriteRenderer>();
    }
    void Start()
    {      
        if (GameManager.instance.gameData.collectionUnlock[colNumCal]) //���� �̹� ���� ��������̸� 
        {
            gameObject.SetActive(false);
        }
        else
        {
            isParticlePlayed = false;
            loopSound.clip = ambience;
            loopSound.Play();
        }
    }

    
    void Update()
    {
        timer += Time.deltaTime;
        if(timer >= 1.2f)
        {
            StartCoroutine(collectionAnim());
            timer = 0f;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {            
            if(collision.transform.up == transform.up)
            {
                if (!isParticlePlayed)
                {
                    part.Play();
                    GetComponent<AudioSource>().Play();
                    sound.PlayOneShot(collect_normal);

                    UIManager.instance.cameraShake(0.5f, 0.3f);
                    GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0); //����ȭ 
                    var em = idleParticle.emission;
                    em.rateOverTime = 0f; //Ž�谡���� �԰� ���� ��ƼŬ ���� 

                    UIManager.instance.collectionAlarm(1); //Ž�谡���ڸ� �����ߴٴ� �޽��� ���  //Ž�谡���� Ȯ���ߴٴ� �޽��� ��� 
                    collectionSave(); //������ ���� 

                    isParticlePlayed = true;
                }
               
                Invoke("deActive", 2f); //2�� �� �����
            }
            else
            {
                sound.PlayOneShot(collect_abnormal);
            }
        }
    }

    void deActive()
    {
        gameObject.SetActive(false);
    }

    void collectionSave()
    {
        GameManager.instance.gameData.collectionTmp.Add(colNumCal); //�ӽ� ����ҿ� ������ ������� �ѹ��� �Է� 

        //GameData �� ������ ���� 
        string ToJsonData = JsonUtility.ToJson(GameManager.instance.gameData);
        string filePath = Application.persistentDataPath + GameManager.instance.gameDataFileNames[GameManager.instance.curSaveFileNum];
        File.WriteAllText(filePath, ToJsonData);
    }

    IEnumerator collectionAnim()
    {
        var waitFrame = new WaitForSeconds(0.1f);
        for (int index=0; index<collectionSprite.Length; index++)
        {
            spr.sprite = collectionSprite[index];
            yield return waitFrame;
        }
        spr.sprite = collectionSprite[0];
    }
}
