using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;



public class Rabbit : Ball
{
    bool arching = false;
    GameObject archarrow;
    public float archdmg = 40;

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
        if (arching && Game.instance.myTurn) { 
            Vector2 currpos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y));
            Vector2 diff = currpos - startDrag;
            diff = new Vector2(diff.x, diff.y);
            angle = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg - 90;
        }
        if (arching && Input.GetMouseButtonDown(0) && !coll.OverlapPoint(mousepos)) {
            Game.instance.pv.RPC("playSound", RpcTarget.All, "lunge");
            Game.instance.mana -= 2;
            Game.instance.moved = true;
            Game.instance.pauseStart = Time.time;
            click = false;
            archarrow = (GameObject) PhotonNetwork.Instantiate("archarrow", transform.position, transform.rotation);
            archarrow.GetComponent<Carrot>().owner = this.gameObject;
            archarrow.transform.eulerAngles = new Vector3(0, 0, angle);
            Game.instance.objects.Add(archarrow);
            angle += 90;
            archarrow.GetComponent<Rigidbody2D>().velocity = new Vector2(5 * Mathf.Cos(angle * Mathf.Deg2Rad), 5 * Mathf.Sin(angle * Mathf.Deg2Rad));
            arching = false;
            GameObject.Find("aimicon").GetComponent<SpriteRenderer>().enabled = false;
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
            angle = 0;
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
