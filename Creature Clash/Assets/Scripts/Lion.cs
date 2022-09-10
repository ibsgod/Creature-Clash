using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class Lion : Ball
{
    GameObject rc;
    AudioClip roar;
    public HashSet<GameObject> feared = new HashSet<GameObject>();

    public void Start() {
        rc = (GameObject) GameObject.Instantiate(Resources.Load("RangeCheck"), transform.position, transform.rotation, transform);
        rc.GetComponent<SpriteRenderer>().enabled = false;
        rc.transform.localScale = new Vector2(4, 4);
        roar = Resources.Load("Sounds/roar") as AudioClip;
    }
    public override void beforeMove()
    {
        if (!Game.instance.moved) {
            canMove = true;
        }
        foreach (GameObject g in Game.instance.balls) {
            Ball scr = g.GetComponent<Ball>();
            if (scr.pid != pid && scr.coll.IsTouching(rc.GetComponent<Collider2D>())) {
                if (!feared.Contains(g) && scr.GetType().ToString() != "Egg") {
                    feared.Add(g);
                    scr.atk /= 2.0f;
                }
            }
            else if (feared.Contains(g)) {            
                feared.Remove(g);
                scr.atk *= 2;
            }
        }
    }

    public override void doubleClick() {
        Game.instance.moved = true;
        Game.instance.pauseStart = Time.time;
        canMove = false;
        click = false;
        bool yes = false;
        if (Game.instance.mana >= 2) {
            foreach (GameObject g in Game.instance.balls) {
                if (g.GetComponent<Ball>().pid != pid && feared.Contains(g)) {
                    yes = true;
                    g.GetComponent<Rigidbody2D>().AddForce((g.transform.position - transform.position).normalized * 40);
                }
            }
            if (yes) {
                Game.instance.pv.RPC("playSound", RpcTarget.All, "roar");
                Game.instance.mana -= 2;
                GameObject.Instantiate(Resources.Load("roar"), transform.position, transform.rotation);
            }
            else {
                audios.PlayOneShot(Game.instance.error);
                Game.instance.createError("No enemies in range!");
            }
        }
        else {
            audios.PlayOneShot(Game.instance.error);
            Game.instance.createError("Not enough mana!");
        }
    }



}
