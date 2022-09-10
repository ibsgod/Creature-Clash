using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Egg : Ball
{
    // Start is called before the first frame update


    public override void move() {

    }

    [PunRPC]
    public override void getHit(float dmg) {
        // Debug.LogError("got hit");
        hp = Mathf.Min(maxhp, Mathf.Max(0, hp - dmg));
        transform.Find("HealthBar").transform.Find("Bar").transform.localScale = new Vector3(hp / maxhp, 1, 1);
        if (hp <= 0 && pid == PhotonNetwork.LocalPlayer.ActorNumber) {
            Game.instance.lose();
        }
    }
}
