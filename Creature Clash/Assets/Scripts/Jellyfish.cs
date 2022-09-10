using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Jellyfish : Ball
{
    public override void extraHit(GameObject collball) {
        if (collball.GetComponent<Ball>().pid != pid) {
            collball.GetComponent<Ball>().pv.RPC("updateCC", RpcTarget.AllBuffered, new string[] {"paralyze", "3"});
        }
    }
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
