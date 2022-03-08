using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonCollider : MonoBehaviour
{
    [SerializeField] GameObject anotherCollider1;
    [SerializeField] GameObject anotherCollider2;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other != null && other.CompareTag("Projectile"))
        {
            anotherCollider1.SetActive(true);
            anotherCollider2.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
