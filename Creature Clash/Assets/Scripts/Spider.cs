using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class Spider : Ball
{
    public bool revealed = false;
    bool arching = false;

    public override void Init()
    {
        base.Init();
        if (!revealed && PhotonNetwork.LocalPlayer.ActorNumber != pid) {
            sr.enabled = false;   
            transform.Find("HealthBar").transform.Find("Bar").transform.Find("BarSprite").GetComponent<SpriteRenderer>().enabled = false;
            transform.Find("HealthBar").transform.Find("Background").GetComponent<SpriteRenderer>().enabled = false;
            transform.Find("TeamCircle").GetComponent<SpriteRenderer>().enabled = false;
        }
    }
    
    [PunRPC]
    public override void getHit(float dmg) {
        // Debug.LogError("got hit");
        if (!revealed) {
            reveal();
        }
        hp = Mathf.Min(maxhp, Mathf.Max(0, hp - dmg));
        transform.Find("HealthBar").transform.Find("Bar").transform.localScale = new Vector3(hp / maxhp, 1, 1);
        if (hp <= 0) {
            // Debug.LogError("hi");
            Game.instance.pv.RPC("playSound", RpcTarget.All, "die");
            pv.RPC("removeBall", RpcTarget.All);
            if (pv.IsMine) {
                // Debug.LogError("bye");
                PhotonNetwork.Destroy(pv);
            }
        }
    }

    public void reveal() {
        revealed = true;
        sr.enabled = true;
        transform.Find("HealthBar").transform.Find("Bar").transform.Find("BarSprite").GetComponent<SpriteRenderer>().enabled = true;
        transform.Find("HealthBar").transform.Find("Background").GetComponent<SpriteRenderer>().enabled = true;
        transform.Find("TeamCircle").GetComponent<SpriteRenderer>().enabled = true;
        audios.PlayOneShot(Resources.Load("Sounds/reveal") as AudioClip);
    }

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
            GameObject web = (GameObject) PhotonNetwork.Instantiate("web", mousepos, transform.rotation);
            web.GetComponent<WebStuff>().setOwner(pv.ViewID);
            Game.instance.callAddObject(web.GetPhotonView().ViewID);
            web.transform.eulerAngles = new Vector3(0, 0, StaticRandom.Instance.Next(0, 359));
            arching = false;
            GameObject.Find("aimicon").GetComponent<SpriteRenderer>().enabled = false;
            audios.PlayOneShot(Game.instance.place);
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

    public override void extraHit(GameObject collball)
    {
        Ball scr = collball.GetComponent<Ball>();
        foreach (GameObject o in Game.instance.objects) {
            if (o.tag == "web") {
                if (scr.coll.IsTouching(o.GetComponent<PolygonCollider2D>())) {
                    scr.pv.RPC("getHit", RpcTarget.All, atk);
                }
            }
        }
    }

}
