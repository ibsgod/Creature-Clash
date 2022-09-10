using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;

public class Ant : Ball
{
    public override void changeTurnExtra() {
        if (PhotonNetwork.LocalPlayer.ActorNumber == pid) {
            Game.instance.mana += 1;
        }
    }

    public override void beforeMove() {
        int fact = 1;
        foreach (GameObject g in Game.instance.balls) {
            if (g != this.gameObject && g.GetComponent<Ball>().GetType() == Type.GetType("Ant") && pid == g.GetComponent<Ball>().pid) {
                fact *= 3;
            }
        }
        atk = Info.stats["Ant"]["atk"] * fact;
    }



    




}
