using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Snake : Ball
{
    public override void extraHit(GameObject collball) {
        if (collball.GetComponent<Ball>().pid != pid) {
            collball.GetComponent<Ball>().pv.RPC("updateCC", RpcTarget.AllBuffered, new string[] {"poison", "3"});
        }
    }



}
