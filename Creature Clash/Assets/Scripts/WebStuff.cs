using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class WebStuff : MonoBehaviour
{
    public GameObject owner;
    bool revealed = false;
    bool hideStart = false;

    void Awake() {
        GetComponent<SpriteRenderer>().enabled = false;
    }
    void Update() {
        if (owner == null) {
            return;
        }
        if (!hideStart && PhotonNetwork.LocalPlayer.ActorNumber == owner.GetComponent<Ball>().pid) {
            GetComponent<SpriteRenderer>().enabled = true;
            hideStart = true;
        }
        foreach (GameObject b in Game.instance.balls) {
            if (b == null) {
                continue;
            }
            Ball collscr = b.GetComponent<Ball>();
            if (collscr.pid == owner.GetComponent<Ball>().pid) {
                continue;
            }
            if (collscr.coll.IsTouching(GetComponent<PolygonCollider2D>())) {
                if (!revealed) {
                    reveal();
                }
                collscr.slow(0.997f);
            }
        }
    }

    public void reveal() {
        revealed = true;
        GetComponent<SpriteRenderer>().enabled = true;
        Game.instance.audios.PlayOneShot(Resources.Load("Sounds/reveal") as AudioClip);
    }

    public void setOwner(int pvid) {
        gameObject.GetPhotonView().RPC("setOwner2", RpcTarget.AllBuffered, pvid);
    }

    [PunRPC] 
    public void setOwner2(int pvid) {
        owner = PhotonView.Find(pvid).gameObject;
    }
}
