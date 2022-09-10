using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class Cow : Ball
{

    public override void extraHit(GameObject collball) {
        if (collball.GetComponent<Ball>().pid == pid) {
            collball.GetComponent<Ball>().pv.RPC("updateCC", RpcTarget.AllBuffered, new string[] {"heal", "3"});
        }
    }


}
