using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class knife : MonoBehaviour
{
    public float waveSpeed;
    public ParticleSystem ps;

    Vector3 initialPos;
    void Start()
    {
        initialPos = transform.position;
    }

    void Update()
    {
        if(transform.up == new Vector3(0,1,0) || transform.up == new Vector3(0, -1, 0))
        {
            transform.position = new Vector3(transform.position.x, initialPos.y + 0.3f * Mathf.Sin(waveSpeed * Time.time), 0);
        }
        else
        {
            transform.position = new Vector3(initialPos.x + 0.3f * Mathf.Sin(waveSpeed * Time.time), transform.position.y, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            ps.Play();
            Invoke("deactive", 1f);
        }
    }

    void deactive()
    {
        gameObject.SetActive(false);
    }
}
