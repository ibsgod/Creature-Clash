using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class Beaver : Ball
{
    bool arching = false;

    public override void beforeMove()
    {
        if (canMove) {
            return;
        }
        if (!arching) {
            canMove = true;
            return;
        }
        Vector2 mousepos = (Vector2)Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y));
        Vector2 currpos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y));
        if (arching && Input.GetMouseButtonDown(0) && !coll.OverlapPoint(mousepos)) {
            Game.instance.mana -= 2;
            Game.instance.moved = true;
            Game.instance.pauseStart = Time.time;
            click = false;
            GameObject dam = (GameObject) PhotonNetwork.Instantiate("dam", mousepos, transform.rotation);
            Game.instance.callAddObject(dam.GetPhotonView().ViewID);
            arching = false;
            GameObject.Find("aimicon").GetComponent<SpriteRenderer>().enabled = false;
            Game.instance.pv.RPC("playSound", RpcTarget.All, "place");
        }
    }

    public override void doubleClick() {
        if (arching) {
            arching = false;
            canMove = true;
            GameObject.Find("aimicon").GetComponent<SpriteRenderer>().enabled = false;
            return;
        }       
        if (Game.instance.mana >= 2) { 
            GameObject.Find("aimicon").GetComponent<SpriteRenderer>().enabled = true;
            arching = true;
            canMove = false;
            startDrag = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y));
        }
        else {
            audios.PlayOneShot(Game.instance.error);
            Game.instance.createError("Not enough mana!");
        }
    }

    public override void changeTurnExtra() {
        arching = false;
        canMove = true;
        GameObject.Find("aimicon").GetComponent<SpriteRenderer>().enabled = false;
    }


}
