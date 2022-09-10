using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class Wolf : Ball
{
    GameObject rc;
    public void Start() {
        rc = (GameObject) GameObject.Instantiate(Resources.Load("RangeCheck"), transform.position, transform.rotation, transform);
        rc.GetComponent<SpriteRenderer>().enabled = false;
        rc.transform.localScale = new Vector2(4, 4);
    }

    public override void beforeMove()
    {
        int debuff = 0;
        foreach (GameObject ball in Game.instance.balls) {
            if (ball.GetComponent<Ball>().pid == pid && ball.GetComponent<Ball>().coll.IsTouching(rc.GetComponent<Collider2D>())) {
                debuff += 1;
            }
        }
        atk = Info.stats["Wolf"]["atk"] - debuff;
    }


}
