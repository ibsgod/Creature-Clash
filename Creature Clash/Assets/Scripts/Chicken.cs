using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class Chicken : Ball
{

    public override void beforeMove()
    {
        atk = hp;
    }
    
    [PunRPC]
    public override void getHit(float dmg) {
        hp = Mathf.Min(maxhp, Mathf.Max(0, hp - dmg));
        transform.Find("HealthBar").transform.Find("Bar").transform.localScale = new Vector3(hp / maxhp, 1, 1);
    }


}
