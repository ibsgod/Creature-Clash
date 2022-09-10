using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Crocodile : Ball
{
    bool arching = false;
    public bool dashing = false;
    public bool dashingback = false;
    Vector2 dashvel;
    Ball draggee;
    Vector2 startdash;
    const float dashrange = 5;
    GameObject rc;
    GameObject currpond;
    Vector2 startPos;
    public override void beforeMove()
    {
        if (transform.position.x < Game.instance.leftwall.transform.position.x || 
        transform.position.x > Game.instance.rightwall.transform.position.x ||
        transform.position.y < Game.instance.bottomwall.transform.position.y ||
        transform.position.y > Game.instance.topwall.transform.position.y) {
            transform.position = startPos;
            dashing = false;
            rb.velocity = Vector2.zero;
            if (draggee != null) {
                draggee.rb.velocity *= 0;
                draggee.rb.isKinematic = false;
            }
            draggee = null;
            rb.velocity *= 0;
            rb.isKinematic = false;
            dashingback = false;
            atk = Info.stats[GetType().Name]["atk"];
            Destroy(rc);
            rc = null;

        }
        currpond = null;
        foreach (GameObject p in GameObject.FindGameObjectsWithTag("pond")) {
            if (coll.IsTouching(p.GetComponent<PolygonCollider2D>())) {
                currpond = p;
                break;
            }
        }
        
        if (dashing) {
            bool ooga = false;
            // print("sdfihsdf");
            foreach (GameObject b in Game.instance.balls) {
                Ball scr = b.GetComponent<Ball>();
                if (scr.coll.IsTouching(coll) && scr.pid != pid) {
                    // print("bruhruhrhr");
                    scr.rb.isKinematic = true;
                    scr.rb.useFullKinematicContacts = true;
                    scr.rb.velocity = -dashvel;
                    rb.velocity = -dashvel;
                    dashing = false;
                    dashingback = true;
                    draggee = scr;
                    ooga = true;
                    break;
                }
            }
            if (!ooga) {
                foreach (GameObject p in GameObject.FindGameObjectsWithTag("wall")) {
                    if (rb.velocity == dashvel && coll.IsTouching(p.GetComponent<BoxCollider2D>())) {
                        rb.velocity = -dashvel;
                        dashing = false;
                        dashingback = true;
                        ooga = true;
                        break;
                    }
                }   
            }
            if (!ooga) {
                if (!coll.IsTouching(rc.GetComponent<CircleCollider2D>())) {
                    // print("hihihih");
                    rb.velocity = -dashvel;
                    dashing = false;
                    dashingback = true;
                    ooga = true;   
                }
            }
            if (ooga) {
                Destroy(rc);
                rc = (GameObject) GameObject.Instantiate(Resources.Load("RangeCheck"), startdash, transform.rotation);
                rc.transform.localScale = new Vector2(0.1f, 0.1f);

            }
        }
        if (dashingback) {
            // print("sdfihhhhhhhsdf");
            if (rc.GetComponent<CircleCollider2D>().OverlapPoint(transform.position)) {
                // print("Sdfsfsefesfe");
                if (draggee != null) {
                    draggee.rb.velocity *= 0;
                    draggee.rb.isKinematic = false;
                }
                draggee = null;
                rb.velocity *= 0;
                rb.isKinematic = false;
                dashingback = false;
                atk = Info.stats[GetType().Name]["atk"];
                Destroy(rc);
                rc = null;
            }

        }
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
            angle = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        }
        if (arching && Input.GetMouseButtonDown(0) && !coll.OverlapPoint(mousepos)) {
            Game.instance.mana -= 2;
            Game.instance.moved = true;
            Game.instance.pauseStart = Time.time;
            click = false;
            rb.isKinematic = true;
            rb.useFullKinematicContacts = true;
            dashvel = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * 10;
            startdash = transform.position;
            GetComponent<Rigidbody2D>().velocity = dashvel;
            arching = false;
            GameObject.Find("aimicon").GetComponent<SpriteRenderer>().enabled = false;
            dashing = true;
            rc.GetComponent<SpriteRenderer>().enabled = false;
            atk = 0;
            audios.PlayOneShot(Resources.Load("Sounds/lunge") as AudioClip);
        }
        if (!Game.instance.myTurn) {
            if (rc != null) {
                Destroy(rc);
                rc = null;
            }
        }
    }

    public override void doubleClick() {
        if (arching) {
            arching = false;
            canMove = true;
            GameObject.Find("aimicon").GetComponent<SpriteRenderer>().enabled = false;
            Destroy(rc);
            rc = null;
            atk = Info.stats[GetType().Name]["atk"];
            return;
        }        
        if (Game.instance.mana >= 2) {
            if (currpond != null) {
                startPos = transform.position;
                GameObject.Find("aimicon").GetComponent<SpriteRenderer>().enabled = true;
                rc = (GameObject) GameObject.Instantiate(Resources.Load("RangeCheck"), transform.position, transform.rotation);
                rc.transform.localScale = new Vector2 (dashrange, dashrange);
                arching = true;
                canMove = false;
                startDrag = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y));
                angle = 0;
            }
            else {
                audios.PlayOneShot(Game.instance.error);
                Game.instance.createError("Not in pond!");
            }
        }
        else {
            audios.PlayOneShot(Game.instance.error);
            Game.instance.createError("Not enough mana!");
        }
    }
    public override void slow(float coef) {
        if (dashing || dashingback) {
            return;
        }
        rb.velocity /= coef;
    }

    public override void pondSlow()
    {
        return;
    }
    public override void changeTurnExtra() {
        arching = false;
        canMove = true;
        GameObject.Find("aimicon").GetComponent<SpriteRenderer>().enabled = false;
    }
}
