using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyBox : MonoBehaviour
{
    [SerializeField] Key key;
    [SerializeField] Animator boxHeadAnimator;
    Transform player;
    Animator boxBodyAnimator;
    bool isKeyBoxOpened;

    void Awake()
    {
        boxBodyAnimator = GetComponent<Animator>();
        player = GameObject.FindWithTag("Player").transform;
    }

    void Start()
    {
        // If player have already obtained the key, start scene with the keybox open
        if (GameManager.instance.gameData.curAchievementNum >= key.achievementNum) {
            boxBodyAnimator.SetBool("getKey", true);
            boxHeadAnimator.SetBool("getKey", true);
            isKeyBoxOpened = true;
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (!isKeyBoxOpened && other.CompareTag("Player") && transform.eulerAngles.z == player.eulerAngles.z)
        {
            isKeyBoxOpened = true;
            boxBodyAnimator.SetBool("allowKey", true);
            boxHeadAnimator.SetBool("allowKey", true);
            Invoke("ActivateKey", 0.9f);
        }
    }

    void ActivateKey()
    {
        key.gameObject.SetActive(true);
    }
}
