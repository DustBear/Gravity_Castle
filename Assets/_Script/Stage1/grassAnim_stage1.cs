using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class grassAnim_stage1 : MonoBehaviour
{
    public Sprite[] spriteGroup;
    SpriteRenderer spr;

    GameObject playerObj;

    bool isAnimCor;
    AudioSource sound;

    public AudioClip[] grassSounds;
    void Start()
    {
        spr = GetComponent<SpriteRenderer>();
        sound = GetComponent<AudioSource>();

        spr.sprite = spriteGroup[0];
        isAnimCor = false;
        playerObj = null;
    }

    void Update()
    {
        if(playerObj != null)
        {
            if(playerObj.GetComponent<Rigidbody2D>().velocity.magnitude > 0.1f && !isAnimCor)
            {
                StartCoroutine(grassAnim());
            }
        }
    }

    float frameDelay = 0.05f;
    IEnumerator grassAnim()
    {
        isAnimCor = true;
        grassSound();
        for(int index=0; index<8; index++)
        {
            spr.sprite = spriteGroup[index%4];
            yield return new WaitForSeconds(frameDelay);

            if(playerObj == null) //�÷��̾ �ݶ��̴��� ������ 
            {               
                spr.sprite = spriteGroup[0]; //�ٽ� ���� ��������Ʈ�� ���ƿ� 
                isAnimCor = false;

                yield break;
            }
        }

        spr.sprite = spriteGroup[0]; //�ٽ� ���� ��������Ʈ�� ���ƿ� 
        isAnimCor = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            playerObj = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            playerObj = null;
        }
    }

    void grassSound()
    {
        int grassIndex = Random.Range(0, grassSounds.Length);
        if (sound.isPlaying) sound.Stop();

        sound.clip = grassSounds[grassIndex];
        sound.volume = GameManager.instance.optionSettingData.masterVolume_setting * GameManager.instance.optionSettingData.effectVolume_setting;
        sound.Play();
    }
}
