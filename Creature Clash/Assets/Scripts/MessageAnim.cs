using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageAnim : MonoBehaviour
{
    float startTime;
    float animTime = 3;
    public TextMesh tm;
    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - startTime < animTime) {
            transform.position = new Vector2 (0, 1 + (Time.time - startTime) / animTime * 3);
            tm.color = new Color(tm.color.r, tm.color.g, tm.color.b, (animTime - (Time.time - startTime)) / animTime);
        }
        else {
            Destroy(this.gameObject);
        }
    }
}
