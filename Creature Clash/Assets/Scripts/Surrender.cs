using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Surrender : MonoBehaviour
{
    public float clickStart = 0;
    public Collider2D coll;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mousepos = (Vector2)Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y));
        if (Input.GetMouseButtonDown(0) && coll.OverlapPoint(mousepos)) {
            clickStart = Time.time;
        }
        if (!coll.OverlapPoint(mousepos)) {
            clickStart = -1;
        }

        if (!Game.instance.gameOver && Time.time - clickStart > 2 && clickStart != -1) {
            Game.instance.lose();
        }   
    }


     
}
