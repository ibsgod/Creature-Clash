using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Carrot : MonoBehaviour
{
    public GameObject owner;

    void OnTriggerEnter2D(Collider2D coll) {
        if (owner == null) {
            return;
        } 
        Ball collscr = coll.gameObject.GetComponent<Ball>();
        Rabbit scr = owner.GetComponent<Rabbit>();
        if (coll.gameObject.tag == "wall") {
            PhotonNetwork.Destroy(gameObject);
            Game.instance.objects.Remove(this.gameObject);
        }
        if (coll.gameObject.tag == "dam") {
            Game.instance.pv.RPC("playSound", RpcTarget.All, "bump");
            Game.instance.objects.Remove(coll.gameObject);
            PhotonNetwork.Destroy(coll.gameObject);
            PhotonNetwork.Destroy(gameObject);
            Game.instance.objects.Remove(this.gameObject);
        }
        if (owner != null && coll != null && coll.gameObject.tag == "Player" && collscr.pid != scr.pid) {
            // Debug.Log("hitcar");
            collscr.pv.RPC("getHit", RpcTarget.All, scr.archdmg);
            scr.spd += 20;
            PhotonNetwork.Destroy(gameObject);
            Game.instance.objects.Remove(this.gameObject);
        }
    }
}
