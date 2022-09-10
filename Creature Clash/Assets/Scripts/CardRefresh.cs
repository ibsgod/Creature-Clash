using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardRefresh : MonoBehaviour
{
    bool click = false;
    void Update() {

        click = Input.GetMouseButtonDown(0);
        Vector2 mousepos = (Vector2)Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y));
        if (click && GetComponent<Collider2D>().OverlapPoint(mousepos)) {
            Game.instance.refreshCards();
        }
    }
}
