using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DamStuff : MonoBehaviour
{
    public Collider2D coll;
    void LateUpdate() {
        foreach (GameObject b in Game.instance.balls) {
            if (b == null) {
                continue;
            }
            if (b.GetComponent<Ball>().coll.IsTouching(coll)){
                Game.instance.pv.RPC("playSound", RpcTarget.All, "bump");
                Game.instance.objects.Remove(this.gameObject);
                PhotonNetwork.Destroy(this.gameObject);
                break;
            }
        }
    }

}
