using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyBox : MonoBehaviour
{
    [SerializeField] Key key;
    [SerializeField] Animator boxHeadAnimator;
    Animator boxBodyAnimator;
    bool isCollidePlayer;

    void Awake()
    {
        boxBodyAnimator = GetComponent<Animator>();
    }

    void Start()
    {
        // If player have already obtained the key, start scene with the keybox open
        if (GameManager.instance.curAchievementNum >= key.achievementNum) {
            boxBodyAnimator.SetBool("getKey", true);
            boxHeadAnimator.SetBool("getKey", true);
            this.enabled = false;
        }
        else {
            StartCoroutine(ActivateKey());
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player"))
        {
            isCollidePlayer = true;
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Player"))
        {
            isCollidePlayer = false;
        }
    }

    IEnumerator ActivateKey()
    {
        // Open keybox
        while (!isCollidePlayer || Vector2.Distance(transform.up, -Physics2D.gravity.normalized) > 0.1f)
        {
            yield return null;
        }
        boxBodyAnimator.SetBool("allowKey", true);
        boxHeadAnimator.SetBool("allowKey", true);
        yield return new WaitForSeconds(0.9f);
        key.gameObject.SetActive(true);
        this.enabled = false;
    }
}
