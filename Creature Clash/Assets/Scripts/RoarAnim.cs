using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoarAnim : MonoBehaviour
{
    // Start is called before the first frame update
    float startTime = 0;
    SpriteRenderer sr;
    float animTime = 1;
    void Start()
    {
        startTime = Time.time;
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - startTime < animTime) {
            transform.localScale = new Vector2(1 + Mathf.Sqrt(Time.time - startTime) * Mathf.Sqrt(animTime), 1 + Mathf.Sqrt(Time.time - startTime) * Mathf.Sqrt(animTime));
            sr.color = new Color(255, 255, 255, Mathf.Sqrt(animTime - (Time.time - startTime)) / Mathf.Sqrt(animTime));
        }
        else {
            Destroy(this.gameObject);
        }
    }
}
