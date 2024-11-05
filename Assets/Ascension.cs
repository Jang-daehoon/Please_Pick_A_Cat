using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ascension : MonoBehaviour
{
    public float Speed;
    public bool isCleanerConnet;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("GameCleaner"))
        {
            isCleanerConnet = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(isCleanerConnet == false)
        {
            transform.Translate(Vector2.up * Speed * Time.deltaTime);
            Destroy(gameObject, 1f);
        }
        else if(isCleanerConnet == true)
        {
            Destroy(gameObject);
        }
    }
}
