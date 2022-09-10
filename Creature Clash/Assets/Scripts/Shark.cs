using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class Shark : Ball
{

    public override void pondSlow()
    {
        return;
    }
    public override void changeTurnExtra() {
        bool yes = false;
        foreach (GameObject p in GameObject.FindGameObjectsWithTag("pond")) {
            if (coll.IsTouching(p.GetComponent<PolygonCollider2D>())) {
                yes = true;
                break;
            }
        }
        if (!yes) {
            getHit(3f);
        }
    }


}
