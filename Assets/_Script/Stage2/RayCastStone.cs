using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCastStone : MonoBehaviour
{
    void Update()
    {
        RaycastHit2D rayHit = Physics2D.BoxCast(transform.position, new Vector2(0.6f, 0.1f), transform.eulerAngles.z, transform.up, 0.8f, 1 << 15);
        if (rayHit.collider != null)
        {
            GameManager.instance.shouldStartAtSavePoint = true;
            UIManager.instance.FadeOut();
        }
    }
}
